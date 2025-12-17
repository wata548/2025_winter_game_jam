using System.Linq;
using UnityEngine;

namespace Entity.Enemy.FSM {
    [CreateAssetMenu(menuName = "EnemyFSM/Chase")]
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

            var sizeInterval = (fsm.transform.localScale.x + fsm.Target.transform.localScale.x) * 0.5f;
            var dist = fsm.Target.transform.position - fsm.transform.position;
            if (dist.magnitude - sizeInterval <= _attackableDistance) {
                pTarget.ChangeState(EnemyState.Attack);
                return;
            }
            
            var length = fsm.Enemy.Speed * Time.deltaTime;
            
            var movable = !Physics.BoxCastAll(
                fsm.transform.position,
                fsm.transform.localScale * 0.49f,
                Mathf.Sign(dist.x) * Vector3.right,
                Quaternion.identity,
                length
            ).Any(hit => hit.transform != fsm.transform);

            pTarget.Movement.SetHorizonPower(movable ? fsm.Enemy.Speed * Mathf.Sign(dist.x) : 0);
        }
    }
}