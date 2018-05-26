using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HH.DataDrivenFramework.Sample {

    public class Bootstrap : SingleTonMonoBehaviour<Bootstrap> {

        protected override void UniqueAwake() {
            Debug.Log("Bootstrap UniqueAwake");
            var hub = SceneControllerHub.Instance;
            hub.RegisterCtrler(
                new SceneType[] {
                    SceneType.SampleInitScene,
                    SceneType.SampleGameScene,
                },
                s => new GameController()
            );
        }
    }
}