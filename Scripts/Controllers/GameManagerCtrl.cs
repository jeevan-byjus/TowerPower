using UnityEngine;
using Byjus.Gamepod.TowerPower.Views;
using Byjus.Gamepod.TowerPower.Verticals;
using Byjus.Gamepod.TowerPower.Util;
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
        int currLife;
        int currCoins;

        public void Init() {
            startPoint = new Vector2(-12.5f, 12.5f);
            tileSize = new Vector2(5, 5);
            entry = WorldPos(new Vector2Int(0, 0));
            exit = WorldPos(new Vector2Int(5, 5));

            string levelS =
                "7 1 5 0 0 0\n" +
                "0 0 4 1 5 0\n" +
                "0 0 0 0 2 0\n" +
                "0 6 1 1 3 0\n" +
                "0 2 0 0 0 0\n" +
                "0 4 1 1 1 8";

            string monstersS = "1 0 154\n" +
                "2 1 250\n" +
                "3 1 123\n" +
                "4 0 290\n" +
                "5 1 500";

            cells = ParseLevel(levelS);
            view.DrawLevel(cells, () => { });
            gamePath = new List<Vector2>();
            gamePath = ParsePath(cells, new Vector2Int(0, 0), new Vector2Int(5, 5));

            towerCtrls = new List<ITowerCtrl>();
            monsterCtrls = new List<IMonsterCtrl>();
            currLife = 3;
            currCoins = 0;

            view.UpdateCoins(currCoins);
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
                    maxHealth = int.Parse(props[2]),
                    currHealth = int.Parse(props[2])
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

            var prev = new Vector2Int(-1, -1);
            var curr = entryPos;
            var adjustment = new Vector2(0, Constants.MONSTER_POS_ADJUST_Y);

            while (curr != exitPos) {
                ret.Add(WorldPos(curr) + adjustment);

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
                    throw new System.Exception("No adjacent cell found. CURR: " + curr + ", prev: " + prev + ", nx: " + nx + ", ny: " + ny);
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
                WaitAndCreateMonster(i * 8, monsters[i]);
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
            if (currLife <= 0) {
                Debug.LogError("No lives left for monster to finish!");
                return;
            }

            view.ReduceLife();
            currLife--;
            if (currLife == 0) {
                // end game
            }
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
            t.unitPosition = GetCellPosForTower(t);
            t.position = startPoint + new Vector2(t.unitPosition.x * tileSize.x, -t.unitPosition.y * tileSize.y);

            view.CreateTower(t, tv => {
                t.valid = ValidateTower(t);

                var ctrl = CreateTowerCtrl(tv);
                ctrl.Init(t);
                towerCtrls.Add(ctrl);

                if (t.valid) {
                    MarkTowerInCells(t.unitSize, t.unitPosition, true);
                }
            });
        }

        Vector2Int GetCellPosForTower(Tower t) {
            var topLeftCorner = startPoint + new Vector2(-tileSize.x / 2, tileSize.y / 2);
            var relPos = new Vector2(t.position.x - topLeftCorner.x, topLeftCorner.y - t.position.y);
            var cellPos = new Vector2Int(Mathf.FloorToInt(relPos.x / tileSize.x), Mathf.FloorToInt(relPos.y / tileSize.y));
            return cellPos;
        }

        bool ValidateTower(Tower t) {
            var cellPos = t.unitPosition;
            for (int i = 0; i < t.unitSize.x; i++) {
                for (int j = 0; j < t.unitSize.y; j++) {
                    var posX = cellPos.x + i;
                    var posY = cellPos.y + j;

                    if (!(posX >= 0 && posX < cells[0].Count &&
                        posY >= 0 && posY < cells.Count &&
                        cells[posY][posX] == CellType.LAND)) {

                        Debug.LogError("Can't place the tower " + t + ", got cell as: " + cellPos);
                        return false;
                    }
                }
            }

            return true;
        }

        void MarkTowerInCells(Vector2Int unitSize, Vector2Int towerPos, bool present) {
            for (int i = 0; i < unitSize.x; i++) {
                for (int j = 0; j < unitSize.y; j++) {
                    var posX = towerPos.x + i;
                    var posY = towerPos.y + j;

                    cells[posY][posX] = present ? CellType.TOWER : CellType.LAND;
                }
            }
        }

        public void OnTowerRemoved(int towerId, TowerType type) {
            var tower = towerCtrls.Find(x => x.Id == towerId && x.Type == type);
            if (tower == null) {
                Debug.LogError("No tower to remove of id: " + towerId + ", type: " + type + "\n" + "Maybe there wasn't space to put the tower");
                return;
            }

            if (tower.Valid) {
                MarkTowerInCells(tower.UnitSize, tower.UnitPostion, false);
            }

            tower.DestroySelf();
            towerCtrls.Remove(tower);
        }

        public void OnInputEnd() {

        }

        public void OnMonsterDestroyed(IMonsterCtrl m) {
            currCoins += m.Value;
            view.UpdateCoins(currCoins);
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
        EXIT,
        BLOCKED,
        TOWER
    }
}