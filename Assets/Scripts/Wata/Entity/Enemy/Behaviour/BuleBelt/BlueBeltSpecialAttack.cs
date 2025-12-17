using Entity.Enemy.FSM;
using UnityEngine;

namespace Entity.Enemy.Behaviour.BlueBelt {
    public class BlueBeltSpecialAttack: SpecialAttackMotion {
        public override bool IsEnd { get; protected set; } = true;
        [SerializeField] private float _duration = 2;
        [SerializeField] private float _speed = 15;
        
        public override void Play() {
            IsEnd = false;
            
            var fsm = GetComponent<TargetFSM>();
            var dir = Mathf.Sign((fsm.Target.GetComponent<Collider>().bounds.center 
                                  - fsm.Movement.Collider.bounds.center).x);
            fsm.Movement.SetHorizonPower(dir * _speed);
            var interval = Wait.ForSecond(_duration, () => IsEnd = true);
            StartCoroutine(interval);
        }

        public override float Term { get; protected set; } = 3;
    }
}