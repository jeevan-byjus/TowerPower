using System.Collections.Generic;
using Byjus.Gamepod.TowerPower.Views;
using UnityEngine;

namespace Byjus.Gamepod.TowerPower.Controllers {
    public class MonsterCtrl : IMonsterCtrl, ITowerTarget {
        public IMonsterView view;
        public IMonsterParent parent;

        Monster mModel;
        List<Vector2> mPath;
        int currPathInd;

        public void Init(Monster mModel, List<Vector2> path) {
            this.mModel = mModel;
            this.mPath = path;

            currPathInd = 0;
            MoveOnPath();
        }

        public void Destroy() {
            view.DestroySelf();
        }

        void MoveOnPath() {
            if (currPathInd == mPath.Count) {
                parent.OnMonsterEndOfPath(this);
                return;
            }

            view.MoveTo(mPath[currPathInd], () => {
                view.Wait(2.0f, () => {
                    currPathInd++;
                    MoveOnPath();
                });
            });
        }

        public void TakeDamage(float damage) {
            mModel.value -= damage;
            Debug.LogError("Taking damage for monster: "+ mModel.id + " : " + damage + ", after damage value: " + mModel.value);

            if (mModel.value <= 0) {
                parent.OnMonsterDestroyed(this);
            }
        }

        public float GetDistanceCovered() {
            return currPathInd;
        }

        public Vector3 GetCurrentPosition() {
            if (currPathInd == mPath.Count) {
                throw new System.Exception("Monster finished path before targetting");
            }
            return mPath[currPathInd];
        }
    }

    public interface IMonsterCtrl {
        void Init(Monster mModel, List<Vector2> path);
        void Destroy();
    }

    public interface IMonsterParent {
        void OnMonsterEndOfPath(IMonsterCtrl m);
        void OnMonsterDestroyed(IMonsterCtrl m);
    }

    public class Monster {
        public int id;
        public MonsterType type;
        public float value;
    }

    public enum MonsterType {
        TYPE_A,
        TYPE_B
    }
}