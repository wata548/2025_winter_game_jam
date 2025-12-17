using System.Linq;
using UnityEngine;

namespace Entity.Enemy.FSM.BlueBelt {
    
    [CreateAssetMenu(menuName = "EnemyFSM/BlueBeltChase")]
    public class ChaseState: StateBase {
        [SerializeField] private float _attackableDistance;
        
        public override void OnEnter(NonTargetFSM pTarget) {
            if (pTarget is not TargetFSM fsm) {
                Debug.LogError($"{pTarget.gameObject.name} should be Target FSM, this is nontarget FSM");
                pTarget.ChangeState(EnemyState.Idle);
                return;
            }
        }

        public override void OnExit(NonTargetFSM pTarget) {
        }

        public override void OnUpdate(NonTargetFSM pTarget) {
            
            if (pTarget is not TargetFSM fsm) {
                Debug.LogError($"{pTarget.gameObject.name} should be Target FSM, this is nontarget FSM");
                pTarget.ChangeState(EnemyState.Idle);
                return;
            }

            if (fsm.UseSpecialAttack()) {
                pTarget.ChangeState(EnemyState.SpecialAttack);
                return;
            }

            var bounds = fsm.Movement.Collider.bounds;
            var sizeInterval = (bounds.size.x + bounds.size.x) * 0.5f;
            var targetPos = fsm.Target.GetComponent<Collider>().bounds.center;
            var dist = targetPos - bounds.center;
            if (dist.magnitude - sizeInterval <= _attackableDistance) {
                pTarget.ChangeState(EnemyState.Attack);
                return;
            }
            
        }
    }
}