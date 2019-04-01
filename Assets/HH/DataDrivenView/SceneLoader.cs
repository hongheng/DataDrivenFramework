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
            public int mainSceneBuildIdx;
            public int[] subScenesBuildIdx;
            public Func<ISceneController> GetCtrler;
        }

        struct SceneData
        {
            public Config cfg;
            public ISceneController ctrler;
            public HashSet<int> scenesNeedLoaded;
        }

        List<SceneData> currentScene = new List<SceneData>();
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
            var scenes = new HashSet<int>(cfg.subScenesBuildIdx) { cfg.mainSceneBuildIdx };
            if (mode == LoadSceneMode.Single) {
                dispatcher.Reset();
                currentScene.Clear();
            } else {
                var notLoaded = scenes.Where(s => !SceneManager.GetSceneByBuildIndex(s).isLoaded);
                scenes = new HashSet<int>(notLoaded);
            }
            currentScene.Add(new SceneData {
                cfg = cfg,
                ctrler = cfg.GetCtrler(),
                scenesNeedLoaded = scenes,
            });
            SceneManager.LoadScene(cfg.mainSceneBuildIdx, mode);
            if (cfg.subScenesBuildIdx != null) {
                foreach (var subscene in cfg.subScenesBuildIdx) {
                    SceneManager.LoadScene(subscene, LoadSceneMode.Additive);
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
            foreach (var sceneData in currentScene) {
                sceneData.ctrler.OnSceneLoaded(scene);
                if (sceneData.scenesNeedLoaded.Remove(scene.buildIndex)) {
                    if (sceneData.scenesNeedLoaded.Count == 0) {
                        dispatcher.StartDispatch(sceneData.ctrler.QueueViewModel, viewHub.Dispatch);
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