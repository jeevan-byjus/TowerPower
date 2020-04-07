using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.TestTools;
using Byjus.Gamepod.TowerPower.Verticals;
using Byjus.Gamepod.TowerPower.Controllers;
using Byjus.Gamepod.TowerPower.Views;
using Byjus.Gamepod.TowerPower.Util;
using System.IO;


namespace Byjus.Gamepod.TowerPower.Tests {
    public class UtilTest : BaseTestSuite {

        [SetUp]
        public void Setup() {
            BaseInit();
            CreateMainCamera();
        }

        [TearDown]
        public void Teardown() {
            BaseTearDown();
        }

        [Test]
        public void SamePositionTests() {
            Assert.AreEqual(true, GenUtil.EqualPositionSw(new Vector2(-2.99f, 1.6f), new Vector2(-2.98f, 1.6f)));
        }
    }
}