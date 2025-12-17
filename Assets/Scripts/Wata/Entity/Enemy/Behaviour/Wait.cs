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

        public static IEnumerator ForAnimation(Animator pAnimator, string pAnimation, Action pAction, Action pOnBegin = null) {
            pOnBegin?.Invoke();

            pAnimator.Play(pAnimation, 0, 0);
            pAnimator.Update(0);

            while (pAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1) {
                yield return null;
            }
            pAction?.Invoke();
        }
    }
}