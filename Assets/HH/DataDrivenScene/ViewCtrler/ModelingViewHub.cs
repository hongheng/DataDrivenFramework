using HH.DataDrivenScene.View;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace HH.DataDrivenScene.ViewCtrler
{
    public class ModelingViewHub : IModelingViewController
    {
        class ViewSet : HashSet<IModelingView> { }
        class TypeTable : Dictionary<Type, ViewSet> { }

        TypeTable typeTable = new TypeTable();

        public void RegiterView(IModelingView view) {
            var type = view.ModelType;
            if (typeTable.ContainsKey(type)) {
                typeTable[type].Add(view);
            } else {
                typeTable.Add(type, new ViewSet() { view });
            }
        }

        public void UnRegisterView(Func<IModelingView, bool> needUnregister) {
            foreach (var handlers in typeTable) {
                foreach (var view in handlers.Value) {
                    if (needUnregister(view)) {
                        handlers.Value.Remove(view);
                    }
                }
            }
        }

        public void Clear() {
            typeTable.Clear();
        }

        void IModelingViewController.UpdateView(ModelingViewUpdateTask data) {
            var model = data.model;
            var type = model.GetType();
            var list = typeTable.ContainsKey(type) ? typeTable[type] : null;
            var count = list == null ? 0 : list.Count;
            Debug.LogFormat("Dispatch {0} => {1} view. {2}", type, count, model);
            if (count == 0) {
                data.onUpdateDone?.Invoke(model, new NoViewUpdated());
                return;
            }
            foreach (var handler in list) {
                try {
                    handler.UpdateView(data);
                } catch (Exception ex) {
                    data.onUpdateDone?.Invoke(model, ex);
                }
            }
        }
    }
}