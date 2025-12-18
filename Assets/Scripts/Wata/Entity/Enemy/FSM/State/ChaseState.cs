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
            pTarget.Enemy.Animation.Play("Walk");
        }

        public override void OnExit(NonTargetFSM pTarget) {
            pTarget.Enemy.Animation.Play("Idle");
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
            
            var length = fsm.Enemy.Speed * Time.deltaTime;
            var temp = Physics.BoxCastAll(
                bounds.center,
                bounds.size * 0.49f,
                Mathf.Sign(dist.x) * Vector3.right,
                Quaternion.identity,
                length
            );
            var movable = !Physics.BoxCastAll(
                bounds.center,
                bounds.size * 0.49f,
                Mathf.Sign(dist.x) * Vector3.right,
                Quaternion.identity,
                length
            ).Any(hit => hit.transform != fsm.transform && (hit.transform.CompareTag("Enemy") || hit.transform.gameObject.layer == LayerMask.GetMask("Ground")));

            pTarget.Movement.SetHorizonPower(movable ? fsm.Enemy.Speed * Mathf.Sign(dist.x) : 0);
        }
    }
}