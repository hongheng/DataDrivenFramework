using HH.DataDrivenScene.SceneCtrler;
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

        public int MainSceneIdx {
            get {
                return 0;
            }
        }

        public int MainSceneBuildIdx {
            get {
                if (scenesBuildIdx != null && scenesBuildIdx.Length > MainSceneIdx) {
                    return scenesBuildIdx[MainSceneIdx];
                }
                return -1;
            }
        }
    }
}