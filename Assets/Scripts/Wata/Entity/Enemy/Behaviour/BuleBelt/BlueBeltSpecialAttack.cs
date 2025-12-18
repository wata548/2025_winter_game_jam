using Entity.Enemy.FSM;
using UnityEngine;

namespace Entity.Enemy.Behaviour.BlueBelt {
    public class BlueBeltSpecialAttack: SpecialAttackMotion {
        public override bool IsEnd { get; protected set; } = true;
        [SerializeField] private Animator _animator;
        [SerializeField] private float _speed = 15;
        [SerializeField] private float _wait = 0.8f;
        [SerializeField] private WindStrom _wind;
        [field: SerializeField] public override float Term { get; protected set; } = 7;
        
        public override void Play() {
            IsEnd = false;
            
            var fsm = GetComponent<TargetFSM>();
            var dir = Mathf.Sign((fsm.Target.GetComponent<Collider>().bounds.center 
                                  - fsm.Movement.Collider.bounds.center).x);
            fsm.Movement.SetHorizonPower(dir * _speed);

            var wait = Wait.ForSecond(_wait, () => {
                _animator.Play("Idle", 0, 0);
                IsEnd = true;
            });
            var interval = Wait.ForAnimation(_animator, () => {
                    fsm.Movement.SetHorizonPower(0);
                    var obj = Instantiate(_wind);
                    var pos = fsm.Movement.Collider.bounds.center;
                    pos.y -= fsm.Movement.Collider.bounds.size.y / 2;
                    obj.transform.position = pos;
                    StartCoroutine(wait);
                }, () => {
                    _animator.Play("Spin", 0, 0);
                    _animator.Update(0);
                }
            );
            StartCoroutine(interval);
        }

    }
}