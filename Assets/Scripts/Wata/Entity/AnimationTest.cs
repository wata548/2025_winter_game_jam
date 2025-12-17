using Extension.Test;
using UnityEngine;

namespace Entity {
    public class AnimationTest : MonoBehaviour {
        [SerializeField] private Animator _animator;

        [TestMethod]
        private void Play(string pAnim) {
            _animator.Play(pAnim);
        }
        
    }
}