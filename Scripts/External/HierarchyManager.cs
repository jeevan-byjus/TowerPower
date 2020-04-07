using UnityEngine;
using Byjus.Gamepod.TowerPower.Controllers;
using Byjus.Gamepod.TowerPower.Views;
using Byjus.Gamepod.TowerPower.Verticals;

namespace Byjus.Gamepod.TowerPower.Externals {
    /// <summary>
    /// Since there are controllers (non-monobehaviors) involved, we can't just directly assign references
    /// So, this class is used which manages all reference assigning
    /// If a View needs some objects, it can be directly assigned
    /// This class is just to assign references between components
    /// Like, Controller-View connections, Parent-Child connections etc.
    /// </summary>
    public class HierarchyManager : MonoBehaviour {
        [SerializeField] InputParser inputParser;
        [SerializeField] GameManagerView gameManager;

        public void Setup() {
            GameManagerCtrl gameCtrl = new GameManagerCtrl();

            inputParser.inputListener = gameCtrl;
            gameManager.ctrl = gameCtrl;
            gameCtrl.view = gameManager;

            ((IGameManagerCtrl) gameCtrl).Init();
            inputParser.Init();
        }
    }
}