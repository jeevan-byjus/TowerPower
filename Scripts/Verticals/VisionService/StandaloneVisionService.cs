﻿using System.Collections.Generic;
using UnityEngine;
using System.Collections;

namespace Byjus.Gamepod.Template.Verticals {
    /// <summary>
    /// Standalone variant of the Vision Service
    /// Generates random number of blue and red cubes in a range when queried for objects
    /// </summary>
    public class StandaloneVisionService : IVisionService {

        public void Init() {

        }

        public List<ExtInput> GetVisionObjects() {
            var numRed = Random.Range(0, 5);
            var numBlue = Random.Range(0, 5);

            var ret = new List<ExtInput>();
            for (int i = 0; i < numBlue; i++) {
                ret.Add(new ExtInput {
                    type = TileType.BLUE_ROD,
                    id = i,
                    position = GeneratePos(ret)
                });
            }

            for (int i = 0; i < numRed; i++) {
                ret.Add(new ExtInput {
                    type = TileType.RED_CUBE,
                    id = i + 1000,
                    position = GeneratePos(ret)
                });
            }

            return ret;
        }

        Vector2 GeneratePos(List<ExtInput> objs) {
            var pos = GetRandomPos();
            while (ExistsPosition(pos, objs)) {
                pos = GetRandomPos();
            }

            return pos;
        }

        Vector2 GetRandomPos() {
            var x = Random.Range(-3.4f, 3.4f);
            var y = Random.Range(-4.7f, 4.7f);
            return new Vector2(x, y);
        }

        bool ExistsPosition(Vector2 testPos, List<ExtInput> objs) {
            foreach (var obj in objs) { if (obj.position == testPos) { return true; } }
            return false;
        }
    }
}