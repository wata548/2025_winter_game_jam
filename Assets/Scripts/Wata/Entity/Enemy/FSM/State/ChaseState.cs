using System.Linq;
using UnityEngine;

namespace Entity.Enemy.FSM {
    [CreateAssetMenu(menuName = "EnemyFSM/Chase")]
    public class ChaseState: StateBase {
        [SerializeField] private float _attackableDistance;
        
        public override void OnEnter(NonTargetFSM pTarget) {
            
        }

        public override void OnExit(NonTargetFSM pTarget) {
        }

        public override void OnUpdate(NonTargetFSM pTarget) {
            if (pTarget is not TargetFSM fsm) {
                Debug.LogError($"{pTarget.gameObject.name} should be Target FSM, this is nontarget FSM");
                pTarget.ChangeState(EnemyState.Idle);
                return;
            }
            
            var dist = fsm.Target.transform.position - fsm.transform.position;
            if (dist.magnitude <= _attackableDistance) {
                Debug.Log("Attack");
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