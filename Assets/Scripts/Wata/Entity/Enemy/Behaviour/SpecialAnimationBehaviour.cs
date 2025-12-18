
using UnityEngine;

namespace Entity.Enemy.Behaviour {
    public abstract class SpecialAnimationBehaviour: SpecialAttackMotion {
        public override bool IsEnd { get; protected set; } = true;
        [SerializeField] protected float _afterDelay;
        [SerializeField] protected float _beforeDelay;
        [SerializeField] protected Animator _animator;
        [SerializeField] protected string _targetAnimation;
        [SerializeField] protected float _middle = 0.3f;


        protected virtual void OnAfter() { }
        protected virtual void OnBefore() { }

        protected virtual void OnMiddle() { }
        
        public override void Play() {
            IsEnd = false;
            _animator.Play(_targetAnimation, 0, 0);
            _animator.Update(0);
            var after = Wait.ForSecond(_afterDelay, () => IsEnd = true);
            var animationSecond = Wait.ForAnimation(_animator, () => {
                StartCoroutine(after);
                OnAfter();
            });
            var animationFirst = Wait.ForAnimation(_animator, () => {
                StartCoroutine(animationSecond);
                OnMiddle();
            }, OnBefore, _middle);
            
            
            var before = Wait.ForSecond(_beforeDelay, () => StartCoroutine(animationFirst));
            StartCoroutine(before);
        }

    }
}