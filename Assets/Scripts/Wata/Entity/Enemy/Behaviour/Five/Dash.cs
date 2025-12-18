using Entity.Enemy.FSM;
using UnityEngine;

namespace Entity.Enemy.Behaviour.Five {
    public class Dash: AnimationBehaviour {
        [SerializeField] private float _speed;

        protected override void OnMiddle() {
            var fsm = GetComponent<TargetFSM>();
            var direction = Mathf.Sign(fsm.Target.GetComponent<Collider>().bounds.center.x
                             - fsm.Movement.Collider.bounds.center.x);

            fsm.Movement.SetHorizonPower(_speed * direction);
        }

        protected override void OnAfter() {
            var fsm = GetComponent<TargetFSM>();
            fsm.Movement.SetHorizonPower(0);
            _animator.Play("Idle", 0, 0);
        }
    }
}