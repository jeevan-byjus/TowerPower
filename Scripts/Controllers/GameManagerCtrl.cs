using UnityEngine;
using Byjus.Gamepod.Template.Views;
using Byjus.Gamepod.Template.Verticals;
using System.Collections.Generic;

namespace Byjus.Gamepod.Template.Controllers {
    public class GameManagerCtrl : IGameManagerCtrl, IExtInputListener {
        public IGameManagerView view;

        public void Init() {
        }

        public void OnInputStart() {
            throw new System.NotImplementedException();
        }

        public void OnInputEnd() {
            throw new System.NotImplementedException();
        }

        public void OnBlueCubeAdded() {
            throw new System.NotImplementedException();
        }

        public void OnRedCubeAdded() {
            throw new System.NotImplementedException();
        }
    }

    public interface IGameManagerCtrl {
        void Init();
    }
}