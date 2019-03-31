using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HH {

    public class ListView<T> where T : MonoBehaviour {

        List<T> list = new List<T>();

        public void Update(Action<T, int> updater, int count = -1, Func<T> creator = null) {
            count = count < 0 ? list.Count : count;
            for (int i = 0; i < count; i++) {
                if (i >= list.Count) {
                    if (creator != null) {
                        list.Add(creator());
                    } else {
                        break;
                    }
                }
                list[i].gameObject.SetActive(true);
                updater(list[i], i);
            }
            for (int i = count; i < list.Count; i++) {
                list[i].gameObject.SetActive(false);
            }
        }
    }
}