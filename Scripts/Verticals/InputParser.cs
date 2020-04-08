using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Byjus.Gamepod.TowerPower.Util;

namespace Byjus.Gamepod.TowerPower.Verticals {
    public class InputParser : MonoBehaviour {
        public IExtInputListener inputListener;

        IVisionService visionService;
        int inputCount;
        List<Tower> currentObjects;

        public void Init() {
            visionService = Factory.GetVisionService();
            inputCount = 0;
            currentObjects = new List<Tower>();

            StartCoroutine(ListenForInput());
        }

        IEnumerator ListenForInput() {
            yield return new WaitForSeconds(Constants.INPUT_DELAY);
            inputCount++;


            inputListener.OnInputStart();

            var objs = visionService.GetVisionObjects();
            Process(objs);
            inputListener.OnInputEnd();

            StartCoroutine(ListenForInput());
        }

        void Process(List<Tower> objs) {
            Segregate(objs, out List<Tower> extraOld, out List<Tower> extraNew);

            foreach (var old in extraOld) {
                currentObjects.Remove(old);
                inputListener.OnTowerRemoved(old.id, old.type);
            }

            foreach (var newO in extraNew) {
                int id = FindNextAvailableId(newO.type);
                newO.id = id;
                currentObjects.Add(newO);
                // always send copy to avoid shared ref erros
                inputListener.OnTowerAdded(new Tower(newO));
            }
        }

        void Segregate(List<Tower> newObjs, out List<Tower> extraOld, out List<Tower> extraNew) {
            extraOld = new List<Tower>();
            extraNew = new List<Tower>();
            extraNew.AddRange(newObjs);

            foreach (var old in currentObjects) {
                bool found = false;
                foreach (var newO in extraNew) {
                    if (old.type == newO.type && GenUtil.EqualPositionSw(old.position, newO.position)) {
                        old.position = newO.position;
                        found = true;
                        extraNew.Remove(newO);
                        break;
                    }
                }

                if (!found) {
                    extraOld.Add(old);
                }
            }
        }

        int FindNextAvailableId(TowerType tileType) {
            int currMax = 0;
            foreach (var tile in currentObjects) {
                if (tile.type == tileType && tile.id > currMax) {
                    currMax = tile.id;
                }
            }

            return currMax + 1;
        }
    }

    public interface IExtInputListener {
        void OnInputStart();
        void OnTowerAdded(Tower t);
        void OnTowerRemoved(int towerId, TowerType type);
        void OnInputEnd();
    }

}