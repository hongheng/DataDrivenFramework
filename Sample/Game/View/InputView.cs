using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace HH.DataDrivenFramework.Sample {

    public class InputView : DataDrivenView<InputModel> {

        InputModel input;
        float z;

        protected override void UpdateView(InputModel model, ref float blockTime) {
            input = model;
        }

        private void Start() {
            z = Camera.main.WorldToScreenPoint(transform.position).z;
        }

        private void Update() {
            if (input == null || !Input.GetMouseButtonUp(0)) {
                return;
            }
            if (input.OnClickSpace != null) {
                var pos = Input.mousePosition;
                pos.z = z;
                input.OnClickSpace(Camera.main.ScreenToWorldPoint(pos));
            }
        }
    }

}