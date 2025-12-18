using System;
using UnityEngine;

namespace Entity.Enemy {
    public class EnemyAnimation: MonoBehaviour {
        [SerializeField] private Animator _animator;

        public bool IsPlaying =>
            (_animator?.GetCurrentAnimatorStateInfo(0).normalizedTime ?? 10) < 1; 
        
        public void Play(string pName) {
            
            if (_animator == null)
                return;
            
            _animator.Play(pName, 0, 0);
            _animator.Update(0);
        }
    }
}