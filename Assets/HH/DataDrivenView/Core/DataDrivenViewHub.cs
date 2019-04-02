using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HH.DataDrivenScene.Core
{
    public class DataDrivenViewHub
    {
        class HandlerTable : Dictionary<IDataDrivenView, Func<object, float>> { }
        class TypeLookup : Dictionary<Type, HandlerTable> { }

        TypeLookup hub = new TypeLookup();

        public void RegiterView(IDataDrivenView view) {
            var type = view.DataType;
            if (!hub.ContainsKey(type)) {
                hub.Add(type, new HandlerTable());
            }
            if (hub[type].ContainsKey(view)) {
                hub[type][view] = view.UpdateView;
            } else {
                hub[type].Add(view, view.UpdateView);
            }
        }

        public void UnRegisterView(Func<Type, IDataDrivenView, bool> needUnregister) {
            foreach (var handlers in hub) {
                foreach (var view in handlers.Value.Keys) {
                    if (needUnregister(handlers.Key, view)) {
                        handlers.Value.Remove(view);
                    }
                }
            }
        }

        public void Clear() {
            hub.Clear();
        }

        public float Dispatch(object data) {
            var type = data.GetType();
            var list = hub.ContainsKey(type) ? hub[type] : null;
            var count = list == null ? 0 : list.Count;
            Debug.LogFormat("Dispatch {0} => {1} view. {2}", type.Name, count, data);
            if (count == 0) {
                return 0;
            }
            var tBlockAll = 0f;
            foreach (var handler in list) {
                var tBlock = 0f;
                try {
                    tBlock = handler.Value(data);
                } catch (Exception ex) {
                    Debug.LogException(ex);
                }
                if (tBlock > tBlockAll) {
                    tBlockAll = tBlock;
                }
            }
            return tBlockAll;
        }
    }
}