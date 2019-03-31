using System;
using System.Collections;
using System.Collections.Generic;

namespace HH.DataDrivenFramework
{
    public class ViewModel
    {
        public object viewData;
    }

    public class ViewModelQueue : Queue<ViewModel> { }

    public class ViewModelDispatcher
    {
        public void StartDispatch(ViewModelQueue qViewModel, Func<object, float> runViewModel) {
            
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