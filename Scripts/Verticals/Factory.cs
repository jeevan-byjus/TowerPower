using System;
using UnityEngine;
using Byjus.Gamepod.TowerPower.Externals;

namespace Byjus.Gamepod.TowerPower.Verticals {
    public class Factory {
        static IVisionService visionService;

        public static void SetVisionService(IVisionService visionService) {
            Factory.visionService = visionService;
            Factory.visionService.Init();
        }

        public static IVisionService GetVisionService() {
            return visionService;
        }
    }
}