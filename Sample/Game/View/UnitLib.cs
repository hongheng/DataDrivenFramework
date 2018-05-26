using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HH.DataDrivenFramework.Sample {

    public class UnitLib : DataDrivenView<UnitLibCmd> {

        [SerializeField] UnitView[] unitPref;
        [SerializeField] Vector3 enemyOffsetRandom;
        [SerializeField] Vector3 enemyOffsetFixed;

        UnitView player;
        Dictionary<UnitId, UnitView> units = new Dictionary<UnitId, UnitView>();

        protected override void UpdateView(UnitLibCmd model, ref float blockTime) {
            var id = model.id;
            if (model.type == UnitLibCmdType.Destroy) {
                if (id != null) {
                    Destroy(GetUnit(id).gameObject);
                    units.Remove(id);
                } else {
                    foreach (var v in units.Values) {
                        Destroy(v.gameObject);
                    }
                    units.Clear();
                }
                return;
            }
            var unit = Instantiate(unitPref[(int)id.type], transform);
            unit.gameObject.SetActive(true);
            unit.id = id;
            switch (id.type) {
            case UnitType.Player:
                player = unit;
                units.Add(id, unit);
                break;
            case UnitType.Enemy:
                unit.transform.position = player.transform.position
                    + new Vector3(Random.Range(-enemyOffsetRandom.x, enemyOffsetRandom.x),
                                  Random.Range(-enemyOffsetRandom.y, enemyOffsetRandom.y),
                                  0)
                    + enemyOffsetFixed;
                units.Add(id, unit);
                break;
            default:
                break;
            }
        }

        public UnitView GetUnit(UnitId id) {
            if (id == player.id) {
                return player;
            }
            UnitView view;
            units.TryGetValue(id, out view);
            return view;
        }
    }
}