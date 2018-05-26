using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

namespace HH.DataDrivenFramework.Sample {

    public static class SceneLoader {

        public static void Load(SceneType scene) {
            SceneManager.LoadScene(scene.ToString());
        }
    }
}