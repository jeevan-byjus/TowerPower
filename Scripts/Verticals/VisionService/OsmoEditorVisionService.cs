using System.Collections.Generic;
using Byjus.Gamepod.TowerPower.Util;
using UnityEngine;
using Byjus.Gamepod.TowerPower.Externals;

#if !CC_STANDALONE
namespace Byjus.Gamepod.TowerPower.Verticals {

    /// <summary>
    /// Editor implementation of Vision Service
    /// If running on editor, this uses the tangible manager's dummy popup to bring deck objects on screen
    /// </summary>
    public class OsmoEditorVisionService : IVisionService {
        IOsmoEditorVisionHelper visionHelper;

        public OsmoEditorVisionService(IOsmoEditorVisionHelper visionHelper) {
            this.visionHelper = visionHelper;
        }

        public List<Tower> GetVisionObjects() {
            var aliveObjs = visionHelper.tangibleManager.AliveObjects;

            var ret = new List<Tower>();
            foreach (var obj in aliveObjs) {
                var pos = GetWorldPos(new Vector2(obj.Location.X, obj.Location.Y));

                if (obj.Id % 3 == 0) {
                    ret.Add(new Tower {
                        type = TowerType.HUNDRED,
                        id = obj.Id,
                        position = pos,
                        unitSize = new Vector2Int(1, 1),
                        unitRange = 1,
                        damage = 100,
                        timeBetweenShots = 3.5f,
                    });
                } else if (obj.Id % 3 == 1) {
                    ret.Add(new Tower {
                        type = TowerType.TEN,
                        id = obj.Id,
                        position =pos ,
                        unitSize = new Vector2Int(1, 1),
                        unitRange = 1,
                        damage = 10,
                        timeBetweenShots = 2f,
                    });
                } else if (obj.Id % 3 == 2) {
                    ret.Add(new Tower {
                        type = TowerType.ONE,
                        id = obj.Id,
                        position = pos,
                        unitSize = new Vector2Int(1, 1),
                        unitRange = 1,
                        damage = 1,
                        timeBetweenShots = 1.5f,
                    });
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