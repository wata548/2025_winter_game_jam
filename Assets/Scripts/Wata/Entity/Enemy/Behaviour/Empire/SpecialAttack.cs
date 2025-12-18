using UnityEngine;

namespace Entity.Enemy.Behaviour.Empire {
    public class SpecialAttack: SpecialAttackMotion {
        public override bool IsEnd { get; protected set; } = true;
        [SerializeField] protected float _afterDelay;
        [SerializeField] protected float _beforeDelay;
        [SerializeField] protected Animator _animator;
        [SerializeField] protected string _targetAnimation;
        [SerializeField] protected float _middle = 0.3f;

        [SerializeField] private PoisonSmoke _smoke;

        protected virtual void OnAfter() { }
        protected virtual void OnBefore() { }

        protected virtual void OnMiddle() {
            var newObj = Instantiate(_smoke);
            newObj.transform.position = GetComponent<Collider>().bounds.center;
        }
        
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

        public override float Term { get; protected set; } = 6;
    }
}