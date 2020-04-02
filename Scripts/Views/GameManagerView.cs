using UnityEngine;
using Byjus.Gamepod.Template.Controllers;

namespace Byjus.Gamepod.Template.Views {

    public class GameManagerView : MonoBehaviour, IGameManagerView {
        public IGameManagerCtrl ctrl;
    }

    public interface IGameManagerView {
        
    }
}