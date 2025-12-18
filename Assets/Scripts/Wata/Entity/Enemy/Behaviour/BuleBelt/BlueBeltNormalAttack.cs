using Entity.Enemy.FSM;
using UnityEngine;
using UnityEngine.Serialization;

namespace Entity.Enemy.Behaviour.BlueBelt {
    public class BlueBeltNormalAttack: AttackMotion {
        public override bool IsEnd { get; protected set; } = true;
        [SerializeField] private Animator _animator;
        [SerializeField] private float _speed;
        [SerializeField] private float _onAir = 0.2f;
        [SerializeField] private float _wait = 0.4f;
        [SerializeField] private float _stormTerm = 0.4f;
        [SerializeField] private WindStrom _wind;
        
        public override void Play() {
            IsEnd = false;
            var fsm = GetComponent<TargetFSM>();

            var endCondition = Wait.ForSecond(_wait, () => IsEnd = true);
            var wait = Wait.ForSecond(_onAir, () => {
                fsm.Movement.enabled = true;
                _animator.Play("Idle", 0, 0);
                StartCoroutine(endCondition);
            });
            
            var jump = Wait.ForAnimation(_animator, 
                () => {
                    fsm.Movement.enabled = false;
                    StartCoroutine(wait);
                }, 
                () => fsm.Movement.Jump()
            );
            var jumpReady = Wait.ForAnimation(_animator, 
                () => StartCoroutine(jump), null, 0.2f);
            
            var spinAnimation = Wait.ForAnimation(_animator, 
                () => {
                    fsm.Movement.SetHorizonPower(0);
                    _animator.Play("Jump", 0, 0);
                    _animator.Update(0);
                    StartCoroutine(jumpReady);
                },
                () => {
                    var dir = Mathf.Sign((fsm.Target.transform.position - fsm.transform.position).x);
                    fsm.Movement.SetHorizonPower(_speed * dir);
                    _animator.Play("Spin", 0, 0);
                    _animator.Update(0);
                }
            );
            var stormInterval = Wait.ForSecond(_stormTerm, 
                () => StartCoroutine(spinAnimation)
            );
            var makeStormAnimation = Wait.ForAnimation(_animator, 
                () => StartCoroutine(stormInterval),
                () => {
                    _animator.Play("MakeStorm", 0, 0);
                    _animator.Update(0);
                    var obj = Instantiate(_wind);
                    
                    var pos = fsm.Movement.Collider.bounds.center;
                    pos.y -= fsm.Movement.Collider.bounds.size.y / 2;
                    var dir = Mathf.Sign((fsm.Target.transform.position - fsm.transform.position).x);
                    pos.x += dir * _speed;
                    
                    obj.transform.position = pos;
                }
            );
            
            StartCoroutine(makeStormAnimation);
        }
    } 
}