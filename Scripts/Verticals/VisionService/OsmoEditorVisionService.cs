using System.Collections.Generic;
using Byjus.Gamepod.Template.Util;
using UnityEngine;
using Byjus.Gamepod.Template.Externals;

#if !CC_STANDALONE
namespace Byjus.Gamepod.Template.Verticals {

    /// <summary>
    /// Editor implementation of Vision Service
    /// If running on editor, this uses the tangible manager's dummy popup to bring deck objects on screen
    /// </summary>
    public class OsmoEditorVisionService : IVisionService {
        IOsmoEditorVisionHelper visionHelper;

        public OsmoEditorVisionService(IOsmoEditorVisionHelper visionHelper) {
            this.visionHelper = visionHelper;
        }

        public List<ExtInput> GetVisionObjects() {
            var aliveObjs = visionHelper.tangibleManager.AliveObjects;

            var ret = new List<ExtInput>();
            foreach (var obj in aliveObjs) {

                var pos = GetWorldPos(new Vector2(obj.Location.X, obj.Location.Y));

                if (obj.Id < 10) {
                    for (int i = 0; i < 10; i++) {
                        ret.Add(new ExtInput { id = obj.Id, type = TileType.BLUE_ROD, position = pos + new Vector3(i * 1, 0) });
                    }
                } else {
                    ret.Add(new ExtInput { id = obj.Id, type = TileType.RED_CUBE, position = pos });
                }
            }
            return ret;
        }

        Vector3 GetWorldPos(Vector3 editorPos) {
            var mDimens = CameraUtil.MainDimens();
            var edDimens = visionHelper.GetCameraDimens();
            var x = editorPos.x * (mDimens.x / edDimens.x);
            var y = editorPos.y * (mDimens.y / edDimens.y);

            return new Vector2(x, y);
        }

        public void Init() {

        }
    }
}
#endif