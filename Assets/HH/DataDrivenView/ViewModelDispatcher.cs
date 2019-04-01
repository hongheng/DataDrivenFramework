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
        void StartDispatch(ViewModelQueue qViewModel, Func<object, float> runViewModel);
        void Reset();
    }

    public class ViewModelDispatcher : IViewModelDispatcher
    {
        MonoBehaviour monoBehaviour;
        List<Coroutine> dispatching;

        public ViewModelDispatcher(MonoBehaviour monoBehaviour) {
            this.monoBehaviour = monoBehaviour;
            dispatching = new List<Coroutine>();
        }

        public void StartDispatch(ViewModelQueue qViewModel, Func<object, float> runViewModel) {
            dispatching.Add(monoBehaviour.StartCoroutine(Dispatch(qViewModel, runViewModel)));
        }

        public void Reset() {
            dispatching.ForEach(cor => monoBehaviour.StopCoroutine(cor));
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