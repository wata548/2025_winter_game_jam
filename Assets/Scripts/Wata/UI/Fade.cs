using System;
using System.Collections;
using Extension.Test;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public class Fade: MonoBehaviour {
        [SerializeField] private Image _background;
        private Coroutine _routine = null;
        public static Fade Instance { get; private set; } = null;

        private IEnumerator ChangeAlpha(float pTargetValue, float pDuration, Action pAction = null) {
            var color = _background.color;
            var sum = pTargetValue - color.a;
            var time = pDuration;
            while (true) {
                time -= Time.deltaTime;
                if (time <= 0)
                    break;

                color.a += Time.deltaTime * sum / pDuration;
                Debug.Log(color.a);
                _background.color = color;
                yield return null;
            }

            color.a = pTargetValue;
            _background.color = color;
            pAction?.Invoke();
        }

        public void Change(float pValue, float pDuration = 0.4f, Action pAction = null) {
            if (_routine != null)
                StopCoroutine(_routine);
            _routine = StartCoroutine(ChangeAlpha(pValue, pDuration, pAction));
        }

        public void Awake() {
            Instance = this;
        }
    }
}