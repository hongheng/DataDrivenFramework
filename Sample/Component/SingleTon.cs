using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HH {

    public abstract class SingleTon<T> where T : class, new() {

        static T instance;

        public static T Instance {
            get {
                if (instance == null) {
                    instance = new T();
                }
                return instance;
            }
        }
    }

    public abstract class SingleTonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour {

        protected static T instance;

        public static T InstanceNow {
            get {
                if (instance == null) {
                    instance = (T)FindObjectOfType(typeof(T));
                    if (instance != null) {
                        DontDestroyOnLoad(instance);
                    }
                }
                return instance;
            }
        }

        public static T GetOrCreateInstance(string prefabName) {
            if (InstanceNow == null) {
                var prefab = Resources.Load(prefabName);
                var go = Instantiate(prefab) as GameObject;
                instance = go.GetComponent<T>();
                DontDestroyOnLoad(instance);
            }
            return instance;
        }

        private void Awake() {
            if (instance == null) {
                instance = this as T;
            }
            if (instance == this) {
                DontDestroyOnLoad(instance);
                UniqueAwake();
            } else {
                Destroy(this);
            }
        }

        protected virtual void UniqueAwake() { }
    }
}
