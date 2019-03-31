using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using System.Linq;

namespace HH.DataDrivenFramework
{
    public class SceneLoaderConfig
    {
        public int buildIdx;
        public int[] subScenes;
        public Func<ISceneController> GetCtrler;
    }

    public class SceneLoader
    {
        static SceneLoader instance;
        public static SceneLoader Instance { get { return instance = instance ?? new SceneLoader(); } }

        SceneLoaderConfig curCfg;
        ISceneController curCtrler;
        ViewModelDispatcher dispatcher;
        DataDrivenViewHub viewHub;
        
        SceneLoader() {
            viewHub = new DataDrivenViewHub();
            dispatcher = new ViewModelDispatcher();
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
            SceneManager.activeSceneChanged += OnActiveSceneChanged;
        }

        public void LoadScene(SceneLoaderConfig cfg) {
            curCfg = cfg;
            curCtrler = cfg.GetCtrler();
            SceneManager.LoadScene(cfg.buildIdx);
            if (cfg.subScenes != null) {
                foreach (var subscene in cfg.subScenes) {
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
            if (curCtrler != null) {
                curCtrler.OnSceneLoaded(scene);
                if (curCfg != null && SceneManager.GetSceneByBuildIndex(curCfg.buildIdx).isLoaded) {
                    if (curCfg.subScenes != null) {
                        if (curCfg.subScenes.Select(SceneManager.GetSceneByBuildIndex).All(s => s.isLoaded)) {
                            dispatcher.StartDispatch(curCtrler.QueueViewModel, viewHub.Dispatch);
                        }
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
            if (curCtrler != null) {
                curCtrler.OnSceneUnloaded(scene);
            }
        }

        #endregion
    }
}