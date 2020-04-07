using UnityEngine;
using Byjus.Gamepod.TowerPower.Views;
using Byjus.Gamepod.TowerPower.Verticals;
using System.Collections.Generic;

namespace Byjus.Gamepod.TowerPower.Controllers {
    public class GameManagerCtrl : IGameManagerCtrl, IExtInputListener {
        public IGameManagerView view;

        List<List<CellType>> cells;

        public void Init() {
            string levelS = "0 0 0 0 0 0 0 0 0 0\n" +
                "1 1 1 5 0 0 0 0 0 0\n" +
                "0 0 0 4 1 1 1 5 0 0\n" +
                "0 0 0 0 0 0 0 2 0 0\n" +
                "0 0 0 0 0 0 0 2 0 0\n" +
                "0 6 1 1 1 1 1 3 0 0\n" +
                "0 2 0 0 0 0 0 0 0 0\n" +
                "0 4 1 5 0 0 0 0 0 0\n" +
                "0 0 0 4 1 1 1 1 1 1\n" +
                "0 0 0 0 0 0 0 0 0 0";

            cells = Parse(levelS);
            view.DrawLevel(cells, () => { });
        }

        List<List<CellType>> Parse(string s) {
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
    }

    public interface IGameManagerCtrl {
        void Init();
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