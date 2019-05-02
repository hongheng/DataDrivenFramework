using HH.DataDrivenScene.ViewCtrler;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace HH.DataDrivenScene.SceneCtrler
{
    public interface ISceneController
    {
        void InitViewController(IModelingViewController viewController);
        void OnSceneLoaded(Scene scene, bool selfLoaded);
        void OnSceneUnloaded(Scene scene, bool selfUnloaded);
    }
}