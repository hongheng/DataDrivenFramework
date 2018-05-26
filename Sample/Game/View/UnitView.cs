using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HH.DataDrivenFramework.Sample {

    public delegate Vector3 MoveTask(Vector3 positionNow, float time);

    public class UnitView : MonoBehaviour {

        public UnitId id;
        public MoveTask moveTask;
        public Action<UnitId, UnitId> onCollision;

        private void Update() {
            if (moveTask != null) {
                transform.position = moveTask(transform.position, Time.deltaTime);
            }
        }

        private void OnCollisionEnter(Collision collision) {
            if (onCollision != null) {
                onCollision(id, collision.gameObject.GetComponent<UnitView>().id);
            }
        }
    }
}