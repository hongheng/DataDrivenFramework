using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using System.Linq;

namespace HH.DataDrivenFramework
{
    public class SceneLoader
    {
        public class Config
        {
            /// <summary>
            /// Element 0 is main scene, others is subscene.
            /// </summary>
            public int[] scenesBuildIdx;
            public Func<ISceneController> GetCtrler;
        }

        struct RuntimeData
        {
            public Config cfg;
            public ISceneController ctrler;
            public HashSet<int> scenesNeedLoaded;
        }

        List<RuntimeData> currentScene = new List<RuntimeData>();
        IViewModelDispatcher dispatcher;
        DataDrivenViewHub viewHub;

        public SceneLoader(MonoBehaviour monoBehaviour) : this(new ViewModelDispatcher(monoBehaviour)) {}

        public SceneLoader(IViewModelDispatcher viewModelDispatcher) {
            dispatcher = viewModelDispatcher;
            viewHub = new DataDrivenViewHub();
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
            SceneManager.activeSceneChanged += OnActiveSceneChanged;
        }

        public void LoadScene(Config cfg, LoadSceneMode mode = LoadSceneMode.Single) {
            IEnumerable<int> scenes = cfg.scenesBuildIdx;
            if (mode == LoadSceneMode.Single) {
                dispatcher.Reset();
                currentScene.Clear();
            } else {
                // 避免重复加载子场景
                scenes = scenes.Where(s => !SceneManager.GetSceneByBuildIndex(s).isLoaded);
            }
            currentScene.Add(new RuntimeData {
                cfg = cfg,
                ctrler = cfg.GetCtrler(),
                scenesNeedLoaded = new HashSet<int>(scenes),
            });
            for (int i = 0; i < cfg.scenesBuildIdx.Length; i++) {
                SceneManager.LoadScene(cfg.scenesBuildIdx[i], i == 0 ? mode : LoadSceneMode.Additive);
            }
        }

        public void UnloadAdditiveScene(Config cfg) {
            var idx = currentScene.FindIndex(s => s.cfg == cfg);
            if (idx < 0) {
                return;
            }
            var needUnload = currentScene[idx];
            currentScene.Remove(needUnload);
            dispatcher.StopDispatch(needUnload.ctrler);
            foreach (var buildIdx in needUnload.cfg.scenesBuildIdx) {
                // TODO: 避免卸载同时被其他场景依赖的子场景。
                SceneManager.UnloadSceneAsync(buildIdx, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);
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
            foreach (var sceneData in currentScene) {
                sceneData.ctrler.OnSceneLoaded(scene);
                if (sceneData.scenesNeedLoaded.Remove(scene.buildIndex)) {
                    if (sceneData.scenesNeedLoaded.Count == 0) {
                        dispatcher.StartDispatch(sceneData.ctrler, sceneData.ctrler.QueueViewModel, viewHub.Dispatch);
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
            foreach (var sceneData in currentScene) {
                sceneData.ctrler.OnSceneUnloaded(scene);
            }
        }

        #endregion
    }
}