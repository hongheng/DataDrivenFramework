using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HH.DataDrivenFramework.Sample {

    public class MonoBehaviourHook : SingleTonMonoBehaviour<MonoBehaviourHook> {

        public static MonoBehaviourHook Instance {
            get { return GetOrCreateInstance("MonoBehaviourHook"); }
        }
    }
}