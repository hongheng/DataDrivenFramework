using HH.DataDrivenScene.SceneCtrler;
using HH.DataDrivenScene.ViewCtrler;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace HH.DataDrivenScene
{
    public abstract class SceneSyncController : ISceneController
    {
        protected IModelingViewController views;
        
        public virtual void InitViewController(IModelingViewController viewController) {
            views = viewController;
        }

        public virtual void OnSceneLoaded(Scene scene, bool selfLoaded) {
            if (selfLoaded) {
                InitView();
            }
        }

        public virtual void OnSceneUnloaded(Scene scene, bool selfUnloaded) {
        }

        protected abstract void InitView();
    }
}