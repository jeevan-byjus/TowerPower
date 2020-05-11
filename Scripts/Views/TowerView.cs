using UnityEngine;
using System.Collections;
using System;
using Byjus.Gamepod.TowerPower.Controllers;
using DG.Tweening;

namespace Byjus.Gamepod.TowerPower.Views {
    public class TowerView : BaseView, ITowerView {
        public ITowerCtrl ctrl;

        [SerializeField] GameObject bulletPrefab;
        [SerializeField] SpriteRenderer towerSprite;
        [SerializeField] Color invalidColor;
        [SerializeField] Color validColor;

        public void FireBullet(Vector3 toPos, Action onDone) {
            // will change to DoTween
            var bullet = Instantiate(bulletPrefab, transform);
            bullet.transform.position = transform.position;
            bullet.transform.DOMove(toPos, 0.2f).OnComplete(() => {
                Destroy(bullet);
                onDone();
            });
        }

        public void MarkValidity(bool validity) {
            towerSprite.color = validity ? validColor : invalidColor;
        }

        public void DestroySelf() {
            Destroy(gameObject);
        }
    }

    public interface ITowerView : IBaseView {
        void FireBullet(Vector3 toPos, Action onDone);
        void MarkValidity(bool validity);
        void DestroySelf();
    }
}