using UnityEngine;
using Byjus.Gamepod.TowerPower.Views;
using Byjus.Gamepod.TowerPower.Verticals;
using System.Collections.Generic;

namespace Byjus.Gamepod.TowerPower.Controllers {
    public class GameManagerCtrl : IGameManagerCtrl, IExtInputListener, IMonsterParent {
        public IGameManagerView view;

        public Vector2 startPoint { get; private set; }
        public Vector2 tileSize { get; private set; }
        public Vector2 entry { get; private set; }
        public Vector2 exit { get; private set; }
        public List<Vector2> gamePath { get; private set; }

        List<List<CellType>> cells;
        List<IMonsterCtrl> monsterCtrls;

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
                Debug.LogError("Adding point: " + curr + ", prev: " + prev);
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
                        Debug.LogError("Next point: " + curr);
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
                    var ctrl = CreateMonsterCtrl(m, mv);
                    ctrl.Init(m, gamePath);
                    monsterCtrls.Add(ctrl);
                });
            });
        }

        IMonsterCtrl CreateMonsterCtrl(Monster m, MonsterView mView) {
            var ctrl = new MonsterCtrl();
            mView.ctrl = ctrl;
            ctrl.view = mView;
            ctrl.parent = this;
            return ctrl;
        }

        public void OnInputStart() {
            //throw new System.NotImplementedException();
        }

        public void OnInputEnd() {
            //throw new System.NotImplementedException();
        }

        public void OnBlueCubeAdded() {
            //throw new System.NotImplementedException();
        }

        public void OnRedCubeAdded() {
            //throw new System.NotImplementedException();
        }

        public void OnEndOfPath(IMonsterCtrl m) {
            // reduce HP or anything else
            m.Destroy();
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