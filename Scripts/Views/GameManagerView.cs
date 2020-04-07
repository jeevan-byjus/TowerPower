using UnityEngine;
using System;
using System.Collections.Generic;
using Byjus.Gamepod.TowerPower.Controllers;

namespace Byjus.Gamepod.TowerPower.Views {

    public class GameManagerView : MonoBehaviour, IGameManagerView {
        public IGameManagerCtrl ctrl;

        [SerializeField] Vector2 drawStartPoint;
        [SerializeField] Vector2 tileSize;
        [SerializeField] GameObject landPrefab;
        [SerializeField] GameObject bentRoadPrefab;
        [SerializeField] GameObject straightRoadPrefab;

        public void DrawLevel(List<List<CellType>> cells, Action onDone) {
            Vector2 pos = drawStartPoint;
            foreach (var row in cells) {
                foreach (var cell in row) {
                    GameObject toInstantiate = landPrefab;
                    float rotation = 0;
                    switch (cell) {
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

                    pos.x += tileSize.x;
                }

                pos.x = drawStartPoint.x;
                pos.y -= tileSize.y;
            }

            onDone();
        }
    }

    public interface IGameManagerView {
        void DrawLevel(List<List<CellType>> cells, Action onDone);
    }
}