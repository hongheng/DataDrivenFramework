using HH.DataDrivenScene.View;
using HH.DataDrivenScene.ViewCtrler;
using System;

namespace HH.DataDrivenScene
{
    public abstract class ModelingAsyncView<T> : ModelingView<T>, INeedNotifyUpdateDone
    {
        OnUpdateDone onUpdateDone;
        
        public override void UpdateView(T model, OnUpdateDone onUpdateDone = null) {
            this.onUpdateDone?.Invoke(currentModel, new UpdateCancelled());
            this.onUpdateDone = onUpdateDone;
            currentModel = model;
            Update(model, this);
        }

        protected override void Update(T model) { }
        protected abstract void Update(T model, INeedNotifyUpdateDone notifier);

        public void NotifyUpdateDone(Exception exception) {
            onUpdateDone?.Invoke(currentModel, exception);
            onUpdateDone = null;
        }
    }
}
