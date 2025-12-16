using UnityEngine;

namespace Entity.Enemy.FSM {
    [CreateAssetMenu(menuName = "EnemyFSM/SpecialAttack")]
    public class SpecialAttackState: StateBase {
        public override void OnEnter(NonTargetFSM pTarget) {
            pTarget.Enemy.SpecialAttackMotion.Play();
        }

        public override void OnExit(NonTargetFSM pTarget) {
        }

        public override void OnUpdate(NonTargetFSM pTarget) {
            if (!pTarget.Enemy.SpecialAttackMotion.IsEnd)
                return;
            
            ChangeState(pTarget);
        }
    }
}