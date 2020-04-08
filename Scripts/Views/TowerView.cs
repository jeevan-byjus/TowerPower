using UnityEngine;
using System.Collections;
using System;
using Byjus.Gamepod.TowerPower.Controllers;

namespace Byjus.Gamepod.TowerPower.Views {
    public class TowerView : BaseView, ITowerView {
        public ITowerCtrl ctrl;

        [SerializeField] GameObject bulletPrefab;

        public void FireBullet(Vector3 toPos, Action onDone) {
            // will change to DoTween
            StartCoroutine(FireBulletAsync(toPos, onDone));
        }

        IEnumerator FireBulletAsync(Vector3 toPos, Action onDone) {
            var bullet = Instantiate(bulletPrefab, transform);

            yield return new WaitForSeconds(0.5f);

            bullet.transform.position += new Vector3(toPos.x - transform.position.x / 2, toPos.y - transform.position.y / 2);

            yield return new WaitForSeconds(0.5f);

            bullet.transform.position = toPos;

            yield return new WaitForSeconds(0.2f);

            Destroy(bullet);

            onDone();
        }

        public void DestroySelf() {
            Destroy(gameObject);
        }
    }

    public interface ITowerView : IBaseView {
        void FireBullet(Vector3 toPos, Action onDone);
        void DestroySelf();
    }
}