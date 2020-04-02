using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Byjus.Gamepod.Template.Verticals;

namespace Byjus.Gamepod.Template.Tests {
    public class BoundingBoxTestSuite : BaseTestSuite {
        [Test]
        public void TestWithCamera() {
            var camDimens = new Vector2(30, 40);

            var boundingBox = new BoundingBox(new List<Vector2> { new Vector2(-100, 90), new Vector2(100, 90), new Vector2(100, -200), new Vector2(-100, -200) });

            Debug.Log("Top: " + boundingBox.topWidth + ", Bottom: " + boundingBox.bottomWidth + ", height: " + boundingBox.height + "\n");

            Assert.AreEqual(boundingBox.topWidth, boundingBox.bottomWidth);
            Assert.AreEqual(new Vector2(-15, 20), boundingBox.GetScreenPoint(camDimens, new Point(-100, 90)));
            Assert.AreEqual(new Vector2(15, 20), boundingBox.GetScreenPoint(camDimens, new Point(100, 90)));
            Assert.AreEqual(new Vector2(15, -20), boundingBox.GetScreenPoint(camDimens, new Point(100, -200)));
            Assert.AreEqual(new Vector2(-15, -20), boundingBox.GetScreenPoint(camDimens, new Point(-100, -200)));
            Assert.AreEqual(new Vector2(0, 0), boundingBox.GetScreenPoint(camDimens, new Point(0, -55)));
        }
    }
}
