using UnityEngine;
using System.Collections;
using System;

namespace Byjus.Gamepod.TowerPower.Views {
    public class BaseView : MonoBehaviour, IBaseView {
        public void Wait(float time, Action onDone) {
            StartCoroutine(WaitFor(time, onDone));
        }

        protected IEnumerator WaitFor(float time, Action onDone) {
            yield return new WaitForSeconds(time);

            onDone();
        }
    }

    public interface IBaseView {
        void Wait(float time, Action onDone);
    }
}