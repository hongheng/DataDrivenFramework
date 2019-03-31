using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace HH.DataDrivenFramework
{
    public interface ISceneController
    {
        ViewModelQueue QueueViewModel { get; }
        void OnSceneLoaded(Scene scene);
        void OnSceneUnloaded(Scene scene);
    }

    public abstract class SceneController : ISceneController
    {
        ViewModelQueue queueView = new ViewModelQueue();

        public ViewModelQueue QueueViewModel => queueView;
        
        public virtual void OnSceneLoaded(Scene scene) {
        }

        public virtual void OnSceneUnloaded(Scene scene) {
        }

        protected void PushToView(object model) {
            queueView.Enqueue(new ViewModel { viewData = model });
        }
    }
}