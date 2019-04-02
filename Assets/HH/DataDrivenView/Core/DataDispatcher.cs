using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HH.DataDrivenScene.Core
{
    public class DispatchData
    {
        public object viewData;
    }

    public class DispatchQueue : Queue<DispatchData> { }

    public interface IDataDispatcher
    {
        void StartDispatch(object taskId, DispatchQueue dispatchQueue, Func<object, float> runViewModel);
        void StopDispatch(object taskId);
        void Reset();
    }

    public class DataDispatcher : IDataDispatcher
    {
        MonoBehaviour monoBehaviour;
        Dictionary<object, Coroutine> dispatching;

        public DataDispatcher(MonoBehaviour monoBehaviour) {
            this.monoBehaviour = monoBehaviour;
            dispatching = new Dictionary<object, Coroutine>();
        }

        public void StartDispatch(object taskId, DispatchQueue dispatchQueue, Func<object, float> runViewModel) {
            dispatching.Add(taskId, monoBehaviour.StartCoroutine(Dispatch(dispatchQueue, runViewModel)));
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

        IEnumerator Dispatch(DispatchQueue qViewModel, Func<object, float> runViewModel) {
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