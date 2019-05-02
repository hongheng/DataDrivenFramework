using HH.DataDrivenScene.View;
using HH.DataDrivenScene.ViewCtrler;
using System;
using UnityEngine;

namespace HH.DataDrivenScene
{
    public abstract class ModelingView<T> : MonoBehaviour, IModelingView
    {
        [SerializeField] bool autoRegister = true;
        protected T currentModel;

        bool IModelingView.AutoRegister => autoRegister;

        Type IModelingView.ModelType => typeof(T);

        void IModelingViewController.UpdateView(ModelingViewUpdateTask data) {
            UpdateView((T)data.model, data.onUpdateDone);
        }

        public virtual void UpdateView(T model, OnUpdateDone onUpdateDone = null) {
            currentModel = model;
            Update(model);
            onUpdateDone?.Invoke(model, null);
        }

        protected abstract void Update(T model);
    }
}
