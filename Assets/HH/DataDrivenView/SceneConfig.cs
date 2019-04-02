using UnityEngine;
using System.Collections;
using System;

namespace HH.DataDrivenScene
{
    public class SceneConfig
    {
        /// <summary>
        /// Element 0 is main scene, others is subscene.
        /// </summary>
        public int[] scenesBuildIdx;
        public Func<ISceneController> GetCtrler;
    }
}