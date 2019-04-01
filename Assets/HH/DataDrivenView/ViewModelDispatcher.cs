using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HH.DataDrivenFramework
{
    public class ViewModel
    {
        public object viewData;
    }

    public class ViewModelQueue : Queue<ViewModel> { }

    public interface IViewModelDispatcher
    {
        void StartDispatch(object taskId, ViewModelQueue qViewModel, Func<object, float> runViewModel);
        void StopDispatch(object taskId);
        void Reset();
    }

    public class ViewModelDispatcher : IViewModelDispatcher
    {
        MonoBehaviour monoBehaviour;
        Dictionary<object, Coroutine> dispatching;

        public ViewModelDispatcher(MonoBehaviour monoBehaviour) {
            this.monoBehaviour = monoBehaviour;
            dispatching = new Dictionary<object, Coroutine>();
        }

        public void StartDispatch(object taskId, ViewModelQueue qViewModel, Func<object, float> runViewModel) {
            dispatching.Add(taskId, monoBehaviour.StartCoroutine(Dispatch(qViewModel, runViewModel)));
        }

        public void StopDispatch(object taskId) {
            if (dispatching.ContainsKey(taskId)) {
                monoBehaviour.StopCoroutine(dispatching[taskId]);
                dispatching.Remove(taskId);
            }
        }

        public void Reset() {
            foreach (var taskId in dispatching.Keys) {
                StopDispatch(taskId);
            }
        }

        IEnumerator Dispatch(ViewModelQueue qViewModel, Func<object, float> runViewModel) {
            while (true) {
                while (qViewModel.Count > 0) {
                    var model = qViewModel.Dequeue();
                    runViewModel(model.viewData);
                }
                yield return null;
            }
        }
    }
}