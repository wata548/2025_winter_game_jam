using System;
using System.Collections;
using UnityEngine;

namespace Entity.Enemy.Behaviour {
    public static class Wait {

        public static IEnumerator ForSecond(float pSec, Action pAction) {
            yield return new WaitForSeconds(pSec);
            pAction?.Invoke();
        }

        public static IEnumerator Until(Func<bool> pCondition, Action pAction, Action pOnBegin = null) {

            pOnBegin?.Invoke();
            while (!pCondition()) {
                yield return null;
            }

            pAction?.Invoke();
        }

        public static IEnumerator ForAnimation(Animator pAnimator, Action pAction, Action pOnBegin = null, float pPoint = 1) {
            pOnBegin?.Invoke();

            while (pAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < pPoint) {
                yield return null;
            }
            pAction?.Invoke();
        }
    }
}