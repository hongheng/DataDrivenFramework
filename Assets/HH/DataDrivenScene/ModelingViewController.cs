using HH.DataDrivenScene.ViewCtrler;
using System;

namespace HH.DataDrivenScene
{
    public static class ModelingViewController
    {
        public static void UpdatView(this IModelingViewController ctrler, object model) {
            ctrler.UpdateView(new ModelingViewUpdateTask {
                model = model,
                onUpdateDone = null,
            });
        }

        public static void UpdatView<T>(this IModelingViewController ctrler, T model, Action<T, Exception> onUpdateDone = null) {
            OnUpdateDone updateDone = null;
            if (onUpdateDone != null) {
                updateDone = (m, exp) => onUpdateDone((T)m, exp);
            }
            ctrler.UpdateView(new ModelingViewUpdateTask {
                model = model,
                onUpdateDone = updateDone,
            });
        }
    }
}