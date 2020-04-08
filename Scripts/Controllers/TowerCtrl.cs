using System.Collections.Generic;
using UnityEngine;
using Byjus.Gamepod.TowerPower.Views;
using Byjus.Gamepod.TowerPower.Verticals;

namespace Byjus.Gamepod.TowerPower.Controllers {
    public class TowerCtrl : ITowerCtrl {
        public ITowerView view;
        public ITowerParent parent;

        Tower mModel;

        public int Id => mModel.id;
        public TowerType Type => mModel.type;
        public Vector2Int UnitSize => mModel.unitSize;
        public Vector2Int UnitPostion => mModel.unitPosition;

        public void Init(Tower mModel) {
            this.mModel = mModel;
            Fire();
        }

        void Fire() {
            var targets = parent.GetInRangeTargetsForTower(this);
            var furthest = GetFurthestTarget(targets);

            if (furthest == null) {
                Debug.LogError("No targets to shoot at");
                return;
            }

            view.FireBullet(furthest.GetCurrentPosition(), () => {
                furthest.TakeDamage(mModel.damage);
            });

            view.Wait(mModel.timeBetweenShots, () => {
                Fire();
            });
        }

        ITowerTarget GetFurthestTarget(List<ITowerTarget> targets) {
            ITowerTarget ret = null;
            float maxDist = 0;

            foreach (var t in targets) {
                var dist = t.GetDistanceCovered();
                if (dist > maxDist) {
                    ret = t;
                    maxDist = dist;
                }
            }

            return ret;
        }

        public Rect GetRange(Vector2 tileSize) {
            var bottomLeft = new Vector2(
                mModel.position.x - (mModel.unitSize.x / 2 + mModel.unitRange) * tileSize.x,
                mModel.position.y - (mModel.unitSize.y / 2 + mModel.unitRange) * tileSize.y);

            var size = new Vector2(
                (mModel.unitSize.x + mModel.unitRange) * tileSize.x,
                (mModel.unitSize.y + mModel.unitRange) * tileSize.y);

            return new Rect(bottomLeft, size);
        }

        public void DestroySelf() {
            view.DestroySelf();
        }
    }

    public interface ITowerCtrl {
        void Init(Tower tower);
        int Id { get; }
        TowerType Type { get; }
        Vector2Int UnitSize { get; }
        Vector2Int UnitPostion { get; }
        Rect GetRange(Vector2 tileSize);
        void DestroySelf();
    }

    public interface ITowerParent {
        List<ITowerTarget> GetInRangeTargetsForTower(ITowerCtrl t);
    }

    public interface ITowerTarget {
        void TakeDamage(float damage);
        float GetDistanceCovered();
        Vector3 GetCurrentPosition();
    }
}