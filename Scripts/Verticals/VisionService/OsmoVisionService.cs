using UnityEngine;
using System;
using System.Collections.Generic;
using Byjus.Gamepod.Template.Util;
using UnityEngine.UI;

#if !CC_STANDALONE
using Osmo.SDK.VisionPlatformModule;
using Osmo.SDK.Vision;

namespace Byjus.Gamepod.Template.Verticals {

    /// <summary>
    /// Implementation of VisionService on iPad Osmo Build
    /// This is the class which connects with Vision and manages conversion from Vision to InGame models
    /// Shouldn't have any game logic here
    /// But, can contain accumulation, and any other parsing logic if required.
    /// sole purpose is to read Vision data, convert to in game models and send those when requested for
    /// </summary>
    public class OsmoVisionService : MonoBehaviour, IVisionService {
        string lastJson;
        BoundingBox visionBoundingBox;

        public void Init() {
            lastJson = "";
            visionBoundingBox = new BoundingBox(new List<Vector2> { new Vector2(-100, 90), new Vector2(100, 90), new Vector2(100, -200), new Vector2(-100, -200) });

            VisionConnector.Register(
                    apiKey: API.Key,
                    objectName: "OsmoVisionServiceView",
                    functionName: "DispatchEvent",
                    mode: 147,
                    async: false,
                    hires: false
                );
        }

        public void DispatchEvent(string json) {
            if (json == null) { return; }
            lastJson = json;
        }

        public List<ExtInput> GetVisionObjects() {
            var output = JsonUtility.FromJson<JOutput>(lastJson);

            var ret = new List<ExtInput>();
            int numBlues = 0, numReds = 0;
            foreach (var item in output.items) {
                var pos = visionBoundingBox.GetScreenPoint(CameraUtil.MainDimens(), item.pt);
                pos = PosAdjustments(pos);

                if (string.Equals(item.color, "blue")) {
                    var it = new ExtInput { id = 100 + numBlues++, type = TileType.BLUE_ROD, position = pos };
                    ret.Add(it);
                } else if (string.Equals(item.color, "red")) {
                    var it = new ExtInput { id = 1000 + numReds++, type = TileType.RED_CUBE, position = pos };
                    ret.Add(it);
                }
            }

            return ret;
        }

        Vector2 PosAdjustments(Vector2 screenPoint) {
            // for camera
            var x = screenPoint.x * Constants.CAMERA_ADJUST_X_RATIO;
            var y = screenPoint.y * Constants.CAMERA_ADJUST_Y_RATIO;

            // round off
            x = (float) Math.Round(x, Constants.POSITION_ROUND_OFF_TO_DIGITS);
            y = (float) Math.Round(y, Constants.POSITION_ROUND_OFF_TO_DIGITS);

            return new Vector2(x, y);
        }
    }
}
#endif