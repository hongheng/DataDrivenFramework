using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HH.DataDrivenFramework.Sample {

    public class UnitCollision : DataDrivenView<CollideTrigger> {

        public UnitLib lib;

        protected override void UpdateView(CollideTrigger model, ref float blockTime) {
            var unit = lib.GetUnit(model.id);
            if (unit == null) {
                Debug.LogWarning("unit is miss: " + model);
                return;
            }
            unit.onCollision = model.onCollision;
        }
    }

}