using Entity.Enemy.FSM;
using UnityEngine;

namespace Entity.Enemy.Behaviour.BlueBelt {
    public class BlueBeltSpecialAttack: SpecialAttackMotion {
        public override bool IsEnd { get; protected set; } = true;
        [SerializeField] private Animator _animator;
        [SerializeField] private float _speed = 15;
        
        public override void Play() {
            IsEnd = false;
            
            var fsm = GetComponent<TargetFSM>();
            var dir = Mathf.Sign((fsm.Target.GetComponent<Collider>().bounds.center 
                                  - fsm.Movement.Collider.bounds.center).x);
            fsm.Movement.SetHorizonPower(dir * _speed);
            var interval = Wait.ForAnimation(_animator, "Spin", () => {
                fsm.Movement.SetHorizonPower(0);
                IsEnd = true;
            });
            StartCoroutine(interval);
        }

        public override float Term { get; protected set; } = 3;
    }
}