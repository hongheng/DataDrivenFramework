using System;
using UnityEngine;

namespace HH.DataDrivenFramework {

    public interface IDataDrivenView {
        Type DataType { get; }
        float UpdateView(object model);
    }

    public abstract class DataDrivenView : MonoBehaviour, IDataDrivenView {
        public abstract Type DataType { get; }
        public abstract float UpdateView(object model);
    }

    public abstract class DataDrivenView<T> : DataDrivenView {

        public float blockTime;

        public override float UpdateView(object model) {
            var block = blockTime;
            UpdateView((T)model, ref block);
            return block;
        }

        public override Type DataType {
            get {
                return typeof(T);
            }
        }

        protected abstract void UpdateView(T model, ref float blockTime);

    }
}
