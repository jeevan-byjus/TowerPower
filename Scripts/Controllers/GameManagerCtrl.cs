using UnityEngine;
using Byjus.Gamepod.TowerPower.Views;
using Byjus.Gamepod.TowerPower.Verticals;
using System.Collections.Generic;

namespace Byjus.Gamepod.TowerPower.Controllers {
    public class GameManagerCtrl : IGameManagerCtrl, IExtInputListener, IMonsterParent, ITowerParent {
        public IGameManagerView view;

        public Vector2 startPoint { get; private set; }
        public Vector2 tileSize { get; private set; }
        public Vector2 entry { get; private set; }
        public Vector2 exit { get; private set; }
        public List<Vector2> gamePath { get; private set; }

        List<List<CellType>> cells;
        List<IMonsterCtrl> monsterCtrls;
        List<ITowerCtrl> towerCtrls;

        public void Init() {
            startPoint = new Vector2(-13.5f, 13.5f);
            tileSize = new Vector2(3, 3);
            entry = WorldPos(new Vector2Int(0, 1));
            exit = WorldPos(new Vector2Int(9, 8));

            string levelS = "0 0 0 0 0 0 0 0 0 0\n" +
                "7 1 1 5 0 0 0 0 0 0\n" +
                "0 0 0 4 1 1 1 5 0 0\n" +
                "0 0 0 0 0 0 0 2 0 0\n" +
                "0 0 0 0 0 0 0 2 0 0\n" +
                "0 6 1 1 1 1 1 3 0 0\n" +
                "0 2 0 0 0 0 0 0 0 0\n" +
                "0 4 1 5 0 0 0 0 0 0\n" +
                "0 0 0 4 1 1 1 1 1 8\n" +
                "0 0 0 0 0 0 0 0 0 0";

            string monstersS = "1 0 125\n" +
                "2 1 215\n" +
                "3 1 100\n" +
                "4 0 80\n" +
                "5 1 400";

            cells = ParseLevel(levelS);
            view.DrawLevel(cells, () => { });
            gamePath = new List<Vector2>();
            gamePath = ParsePath(cells, new Vector2Int(0, 1), new Vector2Int(9, 8));

            towerCtrls = new List<ITowerCtrl>();
            monsterCtrls = new List<IMonsterCtrl>();
            var monsters = ParseMonsters(monstersS);
            MonsterSpawnLoop(monsters);
        }

        List<List<CellType>> ParseLevel(string s) {
            var parts = s.Split('\n');
            var ret = new List<List<CellType>>();
            foreach (var part in parts) {
                var add = new List<CellType>();
                var cs = part.Split(' ');
                foreach (var c in cs) {
                    add.Add((CellType) int.Parse(c));
                }

                ret.Add(add);
            }

            return ret;
        }

        List<Monster> ParseMonsters(string s) {
            var parts = s.Split('\n');
            var ret = new List<Monster>();
            foreach (var part in parts) {
                var props = part.Split(' ');
                var m = new Monster {
                    id = int.Parse(props[0]),
                    type = (MonsterType) int.Parse(props[1]),
                    value = int.Parse(props[2])
                };
                ret.Add(m);
            }

            return ret;
        }

        // assume only one distinct path, so, no two adjacency
        // will go in a loop otherwise
        List<Vector2> ParsePath(List<List<CellType>> cells, Vector2Int entryPos, Vector2Int exitPos) {
            var ret = new List<Vector2>();

            var addX = new int[] { 1, 0, -1, 0 };
            var addY = new int[] { 0, 1, 0, -1 };

            Vector2Int prev = new Vector2Int(-1, -1);

            Vector2Int curr = entryPos;
            while (curr != exitPos) {
                ret.Add(WorldPos(curr));

                int nx = 0, ny = 0;
                bool found = false;
                for (int i = 0; i < 4; i++) {
                    nx = curr.x + addX[i];
                    ny = curr.y + addY[i];

                    if (nx >= 0 && nx < cells[0].Count &&
                        ny >= 0 && ny < cells.Count &&
                        (nx != prev.x || ny != prev.y) &&
                        cells[ny][nx] != CellType.LAND) {

                        found = true;
                        prev = curr;
                        curr = new Vector2Int(nx, ny);
                        break;
                    }
                }

                if (!found) {
                    throw new System.Exception("No adjacent cell found");
                }
            }
            ret.Add(WorldPos(exitPos));

            return ret;
        }

        Vector2 WorldPos(Vector2Int gridPos) {
            return startPoint + new Vector2(gridPos.x * tileSize.x, -gridPos.y * tileSize.y);
        }

        void MonsterSpawnLoop(List<Monster> monsters) {
            for (int i = 0; i < monsters.Count; i++) {
                WaitAndCreateMonster(i * 2, monsters[i]);
            }
        }

        void WaitAndCreateMonster(float time, Monster m) {
            view.Wait(time, () => {
                view.CreateMonster(m, mv => {
                    var ctrl = CreateMonsterCtrl(mv);
                    ctrl.Init(m, gamePath);
                    monsterCtrls.Add(ctrl);
                });
            });
        }

        IMonsterCtrl CreateMonsterCtrl(MonsterView mView) {
            var ctrl = new MonsterCtrl();
            mView.ctrl = ctrl;
            ctrl.view = mView;
            ctrl.parent = this;
            return ctrl;
        }

        ITowerCtrl CreateTowerCtrl(TowerView tView) {
            var ctrl = new TowerCtrl();
            tView.ctrl = ctrl;
            ctrl.view = tView;
            ctrl.parent = this;
            return ctrl;
        }

        public void OnMonsterEndOfPath(IMonsterCtrl m) {
            // reduce HP if any
            DestroyMonster(m);
        }

        void DestroyMonster(IMonsterCtrl m) {
            if (monsterCtrls.Contains(m)) {
                monsterCtrls.Remove(m);
            } else {
                throw new System.Exception("Trying to remove invalid monster on completion of path");
            }
            m.Destroy();
        }

        public void OnInputStart() {

        }

        public void OnTowerAdded(Tower t) {
            // validate wether this tower can be added, i.e. is there another tower at it's place or maybe it's not a valid path
            // or maybe allowed capacity for towers or something
            // also change the position to match the centre of a tile
            // mark off grid locations to occupied
            view.CreateTower(t, tv => {
                var ctrl = CreateTowerCtrl(tv);
                ctrl.Init(t);
                towerCtrls.Add(ctrl);
            });
        }

        public void OnTowerRemoved(int towerId, TowerType type) {
            var tower = towerCtrls.Find(x => x.Id == towerId && x.Type == type);
            if (tower == null) {
                throw new System.Exception("No tower to remove of id: " + towerId + ", type: " + type);
            }

            tower.DestroySelf();
            towerCtrls.Remove(tower);
        }

        public void OnInputEnd() {

        }

        public void OnMonsterDestroyed(IMonsterCtrl m) {
            // destroyed by user, give points maybe
            DestroyMonster(m);
        }

        public List<ITowerTarget> GetInRangeTargetsForTower(ITowerCtrl towerCtrl) {
            // find targets in this range.. how?
            // ask the tower for dimensions
            // get monster's current positions and determine if it is inside this range
            var rect = towerCtrl.GetRange(tileSize);
            var targets = new List<ITowerTarget>();
            foreach (var monster in monsterCtrls) {
                var target = (ITowerTarget) monster;
                if (rect.Contains(target.GetCurrentPosition())) {
                    targets.Add(target);
                }
            }

            return targets;
        }
    }

    public interface IGameManagerCtrl {
        void Init();
        Vector2 startPoint { get; }
        Vector2 tileSize { get; }
        Vector2 entry { get; }
        Vector2 exit { get; }
    }

    public enum CellType {
        LAND,
        STRAIGHT_H,
        STRAIGHT_V,
        BEND_TL,
        BEND_TR,
        BEND_BL,
        BEND_BR,
        ENTRY,
        EXIT
    }
}