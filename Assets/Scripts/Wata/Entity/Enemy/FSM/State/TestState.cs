using UnityEngine;

namespace Entity.Enemy.FSM {
    [CreateAssetMenu(menuName = "EnemyFSM/Test")]
    public class TestState: StateBase {

        //==================================================||Methods 
        public override void OnEnter(NonTargetFSM pTarget) =>
            Debug.Log($"Enter {pTarget.State}state");

        public override void OnExit(NonTargetFSM pTarget) =>
            Debug.Log($"Exit {pTarget.State}state");

        public override void OnUpdate(NonTargetFSM pTarget) { }
    }
}