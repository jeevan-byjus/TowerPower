using System.Collections.Generic;
using Byjus.Gamepod.TowerPower.Views;
using Byjus.Gamepod.TowerPower.Util;
using UnityEngine;

namespace Byjus.Gamepod.TowerPower.Controllers {
    public class MonsterCtrl : IMonsterCtrl, ITowerTarget {
        public IMonsterView view;
        public IMonsterParent parent;

        Monster mModel;
        List<Vector2> mPath;
        int currPathInd;

        public int Value { get { return mModel.maxHealth; } }

        public void Init(Monster mModel, List<Vector2> path) {
            this.mModel = mModel;
            this.mPath = path;

            view.UpdateHealth(1, false);
            currPathInd = 1;
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
                currPathInd++;
                MoveOnPath();

            });
        }

        public void TakeDamage(int damage) {
            mModel.currHealth -= damage;
            var dPercent = Mathf.Clamp01((float) mModel.currHealth / mModel.maxHealth);
            Debug.LogError("Taking damage: " + damage + ", curHealth: " + mModel.currHealth + ", percent: " + dPercent);
            view.UpdateHealth(dPercent, dPercent < Constants.MONSTER_DANGER_HEALTH_THRESHOLD);

            if (mModel.currHealth <= 0) {
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
        int Value { get; }
        void Destroy();
    }

    public interface IMonsterParent {
        void OnMonsterEndOfPath(IMonsterCtrl m);
        void OnMonsterDestroyed(IMonsterCtrl m);
    }

    public class Monster {
        public int id;
        public MonsterType type;
        public int maxHealth;

        public int currHealth;
    }

    public enum MonsterType {
        TYPE_A,
        TYPE_B
    }
}