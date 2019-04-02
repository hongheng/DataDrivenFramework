using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using HH.DataDrivenScene.Core;

namespace HH.DataDrivenScene
{
    public interface ISceneController
    {
        DispatchQueue QueueToDispatch { get; }
        void OnSceneLoaded(Scene scene);
        void OnSceneUnloaded(Scene scene);
    }

    public abstract class SceneController : ISceneController
    {
        DispatchQueue queueToDispatch = new DispatchQueue();

        public DispatchQueue QueueToDispatch => queueToDispatch;
        
        public virtual void OnSceneLoaded(Scene scene) {
        }

        public virtual void OnSceneUnloaded(Scene scene) {
        }

        protected void PushToView(object model) {
            queueToDispatch.Enqueue(new DispatchData { viewData = model });
        }
    }
}