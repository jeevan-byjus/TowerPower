using System.Collections.Generic;
using UnityEngine;
using System.Collections;

namespace Byjus.Gamepod.TowerPower.Verticals {
    /// <summary>
    /// Standalone variant of the Vision Service
    /// Generates random number of blue and red cubes in a range when queried for objects
    /// </summary>
    public class StandaloneVisionService : IVisionService {

        public void Init() {

        }

        public List<Tower> GetVisionObjects() {
            var numHundreds = Random.Range(0, 3);
            var numTens = Random.Range(0, 3);
            var numOnes = Random.Range(0, 3);

            var ret = new List<Tower>();

            for (int i = 0; i < numHundreds; i++) {
                ret.Add(new Tower {
                    type = TowerType.HUNDRED,
                    id = i,
                    position = GeneratePos(ret),
                    unitSize = new Vector2Int(1, 1),
                    unitRange = 1,
                    damage = 100,
                    timeBetweenShots = 3.5f,
                });
            }

            for (int i = 0; i < numTens; i++) {
                ret.Add(new Tower {
                    type = TowerType.TEN,
                    id = i + 100,
                    position = GeneratePos(ret),
                    unitSize = new Vector2Int(1, 1),
                    unitRange = 1,
                    damage = 10,
                    timeBetweenShots = 2f,
                });
            }

            for (int i = 0; i < numOnes; i++) {
                ret.Add(new Tower {
                    type = TowerType.ONE,
                    id = 1000 + i,
                    position = GeneratePos(ret),
                    unitSize = new Vector2Int(1, 1),
                    unitRange = 1,
                    damage = 1,
                    timeBetweenShots = 1.5f,
                });
            }

            return ret;
        }

        Vector2 GeneratePos(List<Tower> objs) {
            var pos = GetRandomPos();
            while (ExistsPosition(pos, objs)) {
                pos = GetRandomPos();
            }

            return pos;
        }

        Vector2 GetRandomPos() {
            var x = Random.Range(-14, 14);
            var y = Random.Range(-19, 19);
            return new Vector2(x, y);
        }

        bool ExistsPosition(Vector2 testPos, List<Tower> objs) {
            foreach (var obj in objs) { if (obj.position == testPos) { return true; } }
            return false;
        }
    }
}