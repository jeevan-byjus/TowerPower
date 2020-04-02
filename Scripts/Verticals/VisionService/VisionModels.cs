using System;
using System.Collections.Generic;
using UnityEngine;
using Byjus.Gamepod.Template.Util;

namespace Byjus.Gamepod.Template.Verticals {

    [Serializable]
    public class JOutput {
        public List<JItem> items;
    }

    [Serializable]
    public class JItem {
        public string color;
        public int id;
        public Point pt;

        public JItem() {
        }

        public JItem(JItem other) {
            color = other.color;
            id = other.id;
            pt = other.pt;
        }

        public override string ToString() {
            return "Color: " + color + ", id: " + id + ", pos: (" + pt.x + ", " + pt.y + ")";
        }
    }

    [Serializable]
    public class Point {
        public float x;
        public float y;

        public Point(float x, float y) {
            this.x = x;
            this.y = y;
        }

        public override string ToString() {
            return x + ", " + y;
        }
    }

    public class BoundingBox {
        public Vector2 topLeftRef;
        public Vector2 newTL;
        public Vector2 newTR;
        public Vector2 newBL;
        public Vector2 newBR;

        public float topWidth;
        public float bottomWidth;
        public float height;


        public BoundingBox(List<Vector2> points) {
            topLeftRef = points[0];
            newTL = GetRelativePosFromTL(points[0]);
            newTR = GetRelativePosFromTL(points[1]);
            newBR = GetRelativePosFromTL(points[2]);
            newBL = GetRelativePosFromTL(points[3]);

            topWidth = newTR.x - newTL.x;
            bottomWidth = newBR.x - newBL.x;
            height = newTR.y - newBR.y;
        }

        override public string ToString() {
            return newTL + ", " + newTR + ", " + newBR + ", " + newBL;
        }

        public Vector2 GetRelativePosFromTL(Vector2 point) {
            return point - topLeftRef;
        }

        // w3 = ( w1 * h - w1 * y + w2 * y ) / h
        public float GetWidthAtYDist(float y) {
            var width = (topWidth * height - topWidth * y + bottomWidth * y) / height;
            return width;
        }


        public float GetLeftMostX(float y) {
            var x = newTL.x + (y - newTL.y) * (newBL.x - newTL.x) / (newBL.y - newTL.y);
            return x;
        }
        // by slope of 2 points formula
        public float GetDistFromLeft(Vector2 relPoint) {
            var leftPoint = GetLeftMostX(relPoint.y);
            var dist = relPoint.x - leftPoint;
            return dist;
        }

        public Vector2 GetScreenPoint(Vector2 screenDimens, Point point) {
            var vecPoint = new Vector2(point.x, point.y);
            var relPoint = GetRelativePosFromTL(vecPoint);

            // pos from top left
            var scrX = screenDimens.x * relPoint.x / topWidth;
            var scrY = screenDimens.y * relPoint.y / height;

            // pos from center
            scrX = -(screenDimens.x / 2) + scrX;
            scrY = (screenDimens.y / 2) + scrY;


            return new Vector2(scrX, scrY);
        }

        public bool PositionEquals(Vector2 point1, Vector2 point2) {
            return Mathf.Abs(point1.x - point2.x) < Constants.HW_POINT_COMPARE_EPSILON &&
                Mathf.Abs(point1.y - point2.y) < Constants.HW_POINT_COMPARE_EPSILON;
        }
    }
}