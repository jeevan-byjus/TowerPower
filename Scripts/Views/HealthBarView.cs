using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Byjus.Gamepod.TowerPower.Views {
    public class HealthBarView : MonoBehaviour {
        [SerializeField] SpriteRenderer bg;
        [SerializeField] SpriteRenderer health;
        [SerializeField] Color okHealthColor;
        [SerializeField] Color dangerHealthColor;

        public void UpdateHealth(float healthPercent, bool showDanger) {
            var newSize = healthPercent * bg.size.x;
            var diff = newSize - health.size.x;

            Debug.LogError("newSize: " + newSize + ", diff: " + diff);

            health.size += new Vector2(diff, 0);
            health.transform.position += new Vector3(diff / 2, 0);
            health.color = showDanger ? dangerHealthColor : okHealthColor;
        }
    }
}
