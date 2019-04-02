using System;
using UnityEngine;

namespace HH.DataDrivenScene
{
    public interface IDataDrivenView
    {
        bool AutoRegister { get; }
        Type DataType { get; }
        float UpdateView(object data);
    }

    public abstract class DataDrivenView<T> : MonoBehaviour, IDataDrivenView
    {
        [SerializeField] bool autoRegister;

        public Type DataType => typeof(T);

        public bool AutoRegister => autoRegister;

        public float UpdateView(object data) {
            float block = 0;
            UpdateView((T)data, ref block);
            return block;
        }

        protected abstract void UpdateView(T data, ref float blockTime);
    }
}
