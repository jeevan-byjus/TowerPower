using UnityEngine;
using System.Collections;
using System;
using Byjus.Gamepod.TowerPower.Controllers;

namespace Byjus.Gamepod.TowerPower.Views {
    public class MonsterView : BaseView, IMonsterView {
        public IMonsterCtrl ctrl;

        public void MoveTo(Vector2 point, Action onDone) {
            transform.position = point;
            onDone();
        }

        public void DestroySelf() {
            Destroy(gameObject);
        }
    }

    public interface IMonsterView : IBaseView {
        void MoveTo(Vector2 point, Action onDone);
        void DestroySelf();
    }
}