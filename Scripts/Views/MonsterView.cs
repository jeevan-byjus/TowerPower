using UnityEngine;
using System.Collections;
using System;
using Byjus.Gamepod.TowerPower.Controllers;
using DG.Tweening;

namespace Byjus.Gamepod.TowerPower.Views {
    public class MonsterView : BaseView, IMonsterView {
        public IMonsterCtrl ctrl;

        public void MoveTo(Vector2 point, Action onDone) {
            transform.DOMove(point, 0.5f).OnComplete(() => { onDone(); });
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