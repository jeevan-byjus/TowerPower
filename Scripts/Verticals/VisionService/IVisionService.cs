using System.Collections.Generic;
using UnityEngine;

namespace Byjus.Gamepod.TowerPower.Verticals {
    /// <summary>
    /// This is the interface used by whichever class wants to read Vision Data
    /// Difference is it should mainly work with in-game models and shouldn't use anything platform dependent
    /// so, no vision related models or any other external platform related models
    /// </summary>
    public interface IVisionService {
        void Init();
        List<Tower> GetVisionObjects();
    }

    public enum TowerType { HUNDRED, TEN, ONE }

    public class Tower {
        public TowerType type;
        public int id;
        public Vector2 position;
        public Vector2Int unitPosition;
        public Vector2Int unitSize;
        public int unitRange;
        public float timeBetweenShots;
        public int damage;
        public bool valid;

        public Tower() {

        }

        public Tower(Tower other) {
            type = other.type;
            id = other.id;
            position = other.position;
            unitSize = other.unitSize;
            unitRange = other.unitRange;
            timeBetweenShots = other.timeBetweenShots;
            damage = other.damage;
            valid = other.valid;
        }

        public override string ToString() {
            return "type: " + type + "\n" +
                "Id: " + id + "\n" +
                "Position: " + position + "\n" +
                "Size: " + unitSize + "\n" +
                "Range: " + unitRange + "\n" +
                "Time: " + timeBetweenShots + "\n" +
                "Damage: " + damage + "\n" +
                "Valid: " + valid + "\n";
        }
    }
}