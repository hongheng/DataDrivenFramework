using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HH.DataDrivenFramework.Sample {

    public class UnitMovable : DataDrivenView<Movable> {

        public UnitLib lib;

        protected override void UpdateView(Movable model, ref float blockTime) {
            var unit = lib.GetUnit(model.id);
            if (unit == null) {
                Debug.LogWarning("unit is miss: " + model);
                return;
            }
            unit.moveTask = null;
            switch (model.type) {
            case MoveType.ToPosition:
                unit.moveTask = (now, t) => now + Delta(model.targetData - now, model.speed, t);
                break;
            case MoveType.ToTarget:
                var target = lib.GetUnit(model.targetId);
                if (target == null) {
                    Debug.LogWarning("target is miss: " + model);
                    return;
                }
                unit.moveTask = (now, t) =>
                    now + (target == null ? Vector3.zero : Delta(target.transform.position - now, model.speed, t));
                break;
            default:
                break;
            }
        }

        Vector3 Delta(Vector2 distance, float speed, float time) {
            if (Mathf.Abs(distance.x) < 0.01 && Mathf.Abs(distance.y) < 0.01) {
                return Vector3.zero;
            }
            return distance.normalized * speed * time;
        }
    }

}