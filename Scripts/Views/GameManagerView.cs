using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using Byjus.Gamepod.TowerPower.Controllers;

namespace Byjus.Gamepod.TowerPower.Views {

    public class GameManagerView : BaseView, IGameManagerView {
        public IGameManagerCtrl ctrl;

        [SerializeField] GameObject landPrefab;
        [SerializeField] GameObject bentRoadPrefab;
        [SerializeField] GameObject straightRoadPrefab;
        [SerializeField] MonsterView monsterPrefabA;
        [SerializeField] MonsterView monsterPrefabB;

        public void CreateMonster(Monster m, Action<MonsterView> onDone) {
            var mV = Instantiate(m.type == MonsterType.TYPE_A ? monsterPrefabA : monsterPrefabB);
            mV.transform.position = ctrl.entry;
            onDone(mV);
        }

        public void DrawLevel(List<List<CellType>> cells, Action onDone) {
            Vector2 pos = ctrl.startPoint;
            foreach (var row in cells) {
                foreach (var cell in row) {
                    GameObject toInstantiate = landPrefab;
                    float rotation = 0;
                    switch (cell) {
                        case CellType.ENTRY: toInstantiate = straightRoadPrefab; break;
                        case CellType.EXIT: toInstantiate = straightRoadPrefab; break;
                        case CellType.STRAIGHT_H: toInstantiate = straightRoadPrefab; break;
                        case CellType.STRAIGHT_V: toInstantiate = straightRoadPrefab; rotation = 90; break;
                        case CellType.BEND_BL: toInstantiate = bentRoadPrefab; break;
                        case CellType.BEND_BR: toInstantiate = bentRoadPrefab; rotation = 90; break;
                        case CellType.BEND_TL: toInstantiate = bentRoadPrefab; rotation = -90; break;
                        case CellType.BEND_TR: toInstantiate = bentRoadPrefab; rotation = 180; break;
                    }

                    var obj = Instantiate(toInstantiate, transform);
                    obj.transform.Rotate(new Vector3(0, 0, rotation));
                    obj.transform.position = pos;

                    pos.x += ctrl.tileSize.x;
                }

                pos.x = ctrl.startPoint.x;
                pos.y -= ctrl.tileSize.y;
            }

            onDone();
        }
    }

    public interface IGameManagerView : IBaseView {
        void DrawLevel(List<List<CellType>> cells, Action onDone);
        void CreateMonster(Monster m, Action<MonsterView> onDone);
    }
}