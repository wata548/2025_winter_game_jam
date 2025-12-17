using Entity.Enemy.FSM;
using UnityEngine;

namespace Entity.Enemy.Behaviour.BlueBelt {
    public class BlueBeltNormalAttack: AttackMotion {
        public override bool IsEnd { get; protected set; } = true;
        [SerializeField] private Animator _animator;
        [SerializeField] private float _speed;
        [SerializeField] private float _wait = 1.3f;
        [SerializeField] private float _stormTerm = 0.4f;
        
        public override void Play() {
            IsEnd = false;
            var fsm = GetComponent<TargetFSM>();

            var wait = Wait.ForSecond(_wait, () => {
                _animator.Play("Idle", 0, 0);
                IsEnd = true;
            });
            var spinAnimation = Wait.ForAnimation(_animator, "Spin", () => {
                fsm.Movement.SetHorizonPower(0);
                fsm.Movement.Jump();
                _animator.Play("Jump", 0, 0);
                StartCoroutine(wait);
            });
            var stormInterval = Wait.ForSecond(_stormTerm, () => {
                var dir = Mathf.Sign((fsm.Target.transform.position - fsm.transform.position).x);
                fsm.Movement.SetHorizonPower(_speed * dir);
                StartCoroutine(spinAnimation);
            });
            var makeStormAnimation = Wait.ForAnimation(_animator, "MakeStorm", () => StartCoroutine(stormInterval));
            
            StartCoroutine(makeStormAnimation);
        }
    } 
}