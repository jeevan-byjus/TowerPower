using System.Collections.Generic;
using Byjus.Gamepod.TowerPower.Views;
using UnityEngine;

namespace Byjus.Gamepod.TowerPower.Controllers {
    public class MonsterCtrl : IMonsterCtrl {
        public IMonsterView view;
        public IMonsterParent parent;

        Monster mModel;
        List<Vector2> mPath;

        public void Init(Monster mModel, List<Vector2> path) {
            this.mModel = mModel;
            this.mPath = path;

            MoveOnPath(0);
        }

        public void Destroy() {
            view.DestroySelf();
        }

        void MoveOnPath(int ind) {
            if (ind == mPath.Count) {
                parent.OnEndOfPath(this);
                return;
            }

            view.MoveTo(mPath[ind], () => {
                view.Wait(0.5f, () => {
                    MoveOnPath(ind + 1);
                });
            });
        }
    }

    public interface IMonsterCtrl {
        void Init(Monster mModel, List<Vector2> path);
        void Destroy();
    }

    public interface IMonsterParent {
        void OnEndOfPath(IMonsterCtrl m);
    }

    public class Monster {
        public int id;
        public MonsterType type;
        public int value;
    }

    public enum MonsterType {
        TYPE_A,
        TYPE_B
    }
}