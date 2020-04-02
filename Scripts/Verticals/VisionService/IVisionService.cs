using System.Collections.Generic;
using UnityEngine;

namespace Byjus.Gamepod.Template.Verticals {
    /// <summary>
    /// This is the interface used by whichever class wants to read Vision Data
    /// Difference is it should mainly work with in-game models and shouldn't use anything platform dependent
    /// so, no vision related models or any other external platform related models
    /// </summary>
    public interface IVisionService {
        void Init();
        List<ExtInput> GetVisionObjects();
    }

    public enum TileType { RED_CUBE, BLUE_ROD }

    public class ExtInput {
        public TileType type;
        public int id;
        public Vector2 position;

        public ExtInput() { }

        public ExtInput(TileType type, int id, Vector2 position) {
            this.type = type;
            this.id = id;
            this.position = position;
        }

        public override string ToString() {
            return id + ", " + type + ", (" + position.x + ", " + position.y + ")";
        }
    }
}