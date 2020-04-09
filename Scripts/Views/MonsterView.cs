using UnityEngine;
using System.Collections;
using System;
using Byjus.Gamepod.TowerPower.Controllers;
using DG.Tweening;

namespace Byjus.Gamepod.TowerPower.Views {
    public class MonsterView : BaseView, IMonsterView {
        public IMonsterCtrl ctrl;
        [SerializeField] HealthBarView healthBar;

        public void MoveTo(Vector2 point, Action onDone) {
            transform.DOMove(point, 2.5f).OnComplete(() => { onDone(); });
        }

        public void UpdateHealth(float healthPercent, bool showDanger) {
            healthBar.UpdateHealth(healthPercent, showDanger);
        }

        public void DestroySelf() {
            Destroy(gameObject);
        }
    }

    public interface IMonsterView : IBaseView {
        void MoveTo(Vector2 point, Action onDone);
        void UpdateHealth(float healthPercent, bool showDanger);
        void DestroySelf();
    }
}