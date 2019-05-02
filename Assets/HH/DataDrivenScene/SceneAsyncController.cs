using HH.DataDrivenScene.ViewCtrler;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HH.DataDrivenScene
{
    public abstract class SceneAsyncController : SceneSyncController
    {
        protected ModelQueueUpdater asyncViews;

        protected SceneAsyncController(MonoBehaviour monoBehaviour) {
            asyncViews = new ModelQueueUpdater(monoBehaviour);
            InitView();
        }

        public override void InitViewController(IModelingViewController viewController) {
            base.InitViewController(viewController);
            asyncViews.SetParent(viewController);
        }

        public override void OnSceneLoaded(Scene scene, bool selfLoaded) {
            if (selfLoaded) {
                asyncViews.StartDispatch();
            }
        }

        public override void OnSceneUnloaded(Scene scene, bool selfUnloaded) {
            if (selfUnloaded) {
                asyncViews.StopDispatch();
            }
        }
    }
}