using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using System.Linq;

namespace HH.DataDrivenScene.Core
{
    public class DataDrivenSceneHub
    {
        struct RuntimeData
        {
            public SceneConfig cfg;
            public ISceneController ctrler;
            public HashSet<int> scenesWaitToLoaded;
        }

        List<RuntimeData> runtimeData;
        int[] sceneNeeded;
        IDataDispatcher dispatcher;
        DataDrivenViewHub viewHub;

        public DataDrivenSceneHub(MonoBehaviour monoBehaviour) : this(new DataDispatcher(monoBehaviour)) {}

        public DataDrivenSceneHub(IDataDispatcher viewModelDispatcher) {
            runtimeData = new List<RuntimeData>();
            sceneNeeded = new int[SceneManager.sceneCountInBuildSettings];
            dispatcher = viewModelDispatcher;
            viewHub = new DataDrivenViewHub();
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
            SceneManager.activeSceneChanged += OnActiveSceneChanged;
        }

        public void LoadScene(SceneConfig cfg, LoadSceneMode mode = LoadSceneMode.Single) {
            if (mode == LoadSceneMode.Single) {
                dispatcher.Reset();
                runtimeData.Clear();
                Array.Clear(sceneNeeded, 0, sceneNeeded.Length);
            }
            var scenesWaitToLoaded = new HashSet<int>();
            for (int i = 0; i < cfg.scenesBuildIdx.Length; i++) {
                var scene = cfg.scenesBuildIdx[i];
                if (sceneNeeded[scene] == 0) {
                    scenesWaitToLoaded.Add(scene);
                    SceneManager.LoadScene(scene, i == 0 ? mode : LoadSceneMode.Additive);
                } else if (!SceneManager.GetSceneByBuildIndex(scene).isLoaded) {
                    scenesWaitToLoaded.Add(scene);
                }
                sceneNeeded[scene]++;
            }
            runtimeData.Add(new RuntimeData {
                cfg = cfg,
                ctrler = cfg.GetCtrler(),
                scenesWaitToLoaded = scenesWaitToLoaded,
            });
        }

        public void UnloadAdditiveScene(SceneConfig cfg) {
            var idx = runtimeData.FindIndex(s => s.cfg == cfg);
            if (idx < 0) {
                return;
            }
            var needUnload = runtimeData[idx];
            runtimeData.Remove(needUnload);
            dispatcher.StopDispatch(needUnload.ctrler);
            foreach (var buildIdx in needUnload.cfg.scenesBuildIdx) {
                sceneNeeded[buildIdx]--;
                if (sceneNeeded[buildIdx] == 0) {
                    SceneManager.UnloadSceneAsync(buildIdx);
                }
            }
        }

        #region SceneManager listener

        void OnActiveSceneChanged(Scene sceneOld, Scene sceneNew) {
            Debug.LogFormat("SceneManagerActiveSceneChanged, {0}, {1}", sceneOld.buildIndex, sceneNew.buildIndex);
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
            Debug.LogFormat("OnSceneLoaded, {0}, {1}", scene.name, scene.buildIndex);
            foreach (var root in scene.GetRootGameObjects()) {
                foreach (var view in root.GetComponentsInChildren<IDataDrivenView>(true)) {
                    Debug.LogFormat("FindAndRegisterViewsInScene, {0}, {1}", view, view.AutoRegister);
                    if (view.AutoRegister) {
                        viewHub.RegiterView(view);
                    }
                }
            }
            foreach (var sceneData in runtimeData) {
                sceneData.ctrler.OnSceneLoaded(scene);
                if (sceneData.scenesWaitToLoaded.Remove(scene.buildIndex)) {
                    if (sceneData.scenesWaitToLoaded.Count == 0) {
                        dispatcher.StartDispatch(sceneData.ctrler, sceneData.ctrler.QueueToDispatch, viewHub.Dispatch);
                    }
                }
            }
        }

        void OnSceneUnloaded(Scene scene) {
            Debug.LogFormat("OnSceneUnloaded, {0}, {1}", scene.name, scene.buildIndex);
            viewHub.UnRegisterView((type, view) => {
                var rm = view == null;
                if (rm) {
                    Debug.LogFormat("FindAndUnRegisterViewsInScene, {0}, {1}", type, view);
                }
                return rm;
            });
            foreach (var sceneData in runtimeData) {
                sceneData.ctrler.OnSceneUnloaded(scene);
            }
        }

        #endregion
    }
}