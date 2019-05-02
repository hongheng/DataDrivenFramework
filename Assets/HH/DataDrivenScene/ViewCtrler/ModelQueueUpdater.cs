using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HH.DataDrivenScene.ViewCtrler
{
    public class ModelQueueUpdater : IModelingViewController
    {
        MonoBehaviour monoBehaviour;
        public IModelingViewController parent;

        Queue<ModelingViewUpdateTask> queue = new Queue<ModelingViewUpdateTask>();
        Coroutine dispatching;

        public ModelQueueUpdater(MonoBehaviour monoBehaviour) {
            this.monoBehaviour = monoBehaviour;
        }

        public void SetParent(IModelingViewController parent) {
            this.parent = parent;
        }

        public void UpdateView(ModelingViewUpdateTask task) {
            queue.Enqueue(task);
        }

        public void StartDispatch() {
            StopDispatch();
            dispatching = monoBehaviour.StartCoroutine(Dispatch());
        }

        public void StopDispatch() {
            if (dispatching != null) {
                monoBehaviour.StopCoroutine(dispatching);
                dispatching = null;
            }
        }
        
        IEnumerator Dispatch() {
            while (true) {
                while (queue.Count > 0 && parent != null) {
                    parent.UpdateView(queue.Dequeue());
                }
                yield return null;
            }
        }
    }
}