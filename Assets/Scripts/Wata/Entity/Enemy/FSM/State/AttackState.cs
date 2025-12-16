using UnityEngine;

namespace Entity.Enemy.FSM {
    
    [CreateAssetMenu(menuName = "EnemyFSM/Attack")]
    public class AttackState: StateBase {
        
        //==================================================||Methods 
        public override void OnEnter(NonTargetFSM pTarget) {
            pTarget.Enemy.AttackMotion.Play();
        }

        public override void OnExit(NonTargetFSM pTarget) {
            
        }

        public override void OnUpdate(NonTargetFSM pTarget) {
            if (!pTarget.Enemy.AttackMotion.IsEnd)
                return;
            
            ChangeState(pTarget);
        }
    }
}