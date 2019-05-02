using HH.DataDrivenScene.SceneCtrler;
using HH.DataDrivenScene.View;
using HH.DataDrivenScene.ViewCtrler;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HH.DataDrivenScene
{
    public class SceneLoader
    {
        static SceneLoader instance;

        public static SceneLoader Instance {
            get {
                if (instance == null) {
                    instance = new SceneLoader();
                }
                return instance;
            }
        }

        struct RuntimeData
        {
            public SceneConfig cfg;
            public ISceneController ctrler;
            public HashSet<int> scenesWaitToLoaded;
        }

        List<RuntimeData> runtimeDatas;
        int[] sceneNeeded;
        ModelingViewHub viewHub;
        
        SceneLoader() {
            runtimeDatas = new List<RuntimeData>();
            sceneNeeded = new int[SceneManager.sceneCountInBuildSettings];
            viewHub = new ModelingViewHub();
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
            SceneManager.activeSceneChanged += OnActiveSceneChanged;
        }

        public void LoadScene(SceneConfig cfg, LoadSceneMode mode = LoadSceneMode.Single) {
            if (mode == LoadSceneMode.Single) {
                runtimeDatas.Clear();
                Array.Clear(sceneNeeded, 0, sceneNeeded.Length);
            }
            var scenesWaitToLoaded = new HashSet<int>();
            for (int i = 0; i < cfg.scenesBuildIdx.Length; i++) {
                var buildIdx = cfg.scenesBuildIdx[i];
                if (sceneNeeded[buildIdx] == 0) {
                    scenesWaitToLoaded.Add(buildIdx);
                    SceneManager.LoadScene(buildIdx, i == cfg.MainSceneIdx ? mode : LoadSceneMode.Additive);
                } else if (!SceneManager.GetSceneByBuildIndex(buildIdx).isLoaded) {
                    scenesWaitToLoaded.Add(buildIdx);
                }
                sceneNeeded[buildIdx]++;
            }
            var ctrler = cfg.GetCtrler();
            ctrler.InitViewController(viewHub);
            runtimeDatas.Add(new RuntimeData {
                cfg = cfg,
                ctrler = ctrler,
                scenesWaitToLoaded = scenesWaitToLoaded,
            });
            if (scenesWaitToLoaded.Count == 0) {
                ctrler.OnSceneLoaded(SceneManager.GetSceneByBuildIndex(cfg.MainSceneBuildIdx), true);
            }
        }

        public void UnloadAdditiveScene(SceneConfig cfg) {
            var idx = runtimeDatas.FindIndex(s => s.cfg == cfg);
            if (idx < 0) {
                return;
            }
            var needUnload = runtimeDatas[idx];
            runtimeDatas.Remove(needUnload);
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
                foreach (var view in root.GetComponentsInChildren<IModelingView>(true)) {
                    Debug.LogFormat("FindAndRegisterViewsInScene, {0}, {1}", view, view.AutoRegister);
                    if (view.AutoRegister) {
                        viewHub.RegiterView(view);
                    }
                }
            }
            foreach (var runtime in runtimeDatas) {
                var isNowAllLoaded = false;
                var waitTo = runtime.scenesWaitToLoaded;
                if (waitTo.Remove(scene.buildIndex)) {
                    isNowAllLoaded = waitTo.Count == 0;
                }
                runtime.ctrler.OnSceneLoaded(scene, isNowAllLoaded);
            }
        }

        void OnSceneUnloaded(Scene scene) {
            Debug.LogFormat("OnSceneUnloaded, {0}, {1}", scene.name, scene.buildIndex);
            viewHub.UnRegisterView(view => {
                var rm = view == null;
                if (rm) {
                    Debug.LogFormat("FindAndUnRegisterViewsInScene, {0}", view);
                }
                return rm;
            });
            foreach (var runtime in runtimeDatas) {
                var isNowUnloaded = runtime.cfg.MainSceneBuildIdx == scene.buildIndex;
                runtime.ctrler.OnSceneUnloaded(scene, isNowUnloaded);
            }
        }

        #endregion
    }
}