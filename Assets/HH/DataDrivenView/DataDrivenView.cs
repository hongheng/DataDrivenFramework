using System;
using UnityEngine;

namespace HH.DataDrivenFramework
{
    public interface IDataDrivenView
    {
        bool AutoRegister { get; }
        Type DataType { get; }
        float UpdateView(object model);
    }

    public abstract class DataDrivenView<T> : MonoBehaviour, IDataDrivenView
    {
        [SerializeField] bool autoRegister;
        [SerializeField] float blockTime;

        public Type DataType => typeof(T);

        public bool AutoRegister => autoRegister;

        public float UpdateView(object model) {
            var block = blockTime;
            UpdateView((T)model, ref block);
            return block;
        }

        protected abstract void UpdateView(T model, ref float blockTime);
    }
}
