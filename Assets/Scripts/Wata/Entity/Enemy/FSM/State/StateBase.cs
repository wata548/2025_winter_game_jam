using UnityEngine;

namespace Entity.Enemy.FSM {
    public abstract class StateBase : ScriptableObject {
       //==================================================|| Fields 
        [SerializeField] private EnemyState[] _nextState;
       
       //==================================================||Methods 
        public abstract void OnEnter(NonTargetFSM pTarget);
        public abstract void OnExit(NonTargetFSM pTarget);
        public abstract void OnUpdate(NonTargetFSM pTarget);

        public void ChangeState(NonTargetFSM pFsm) {
            var idx = Random.Range(0, _nextState.Length);
            pFsm.ChangeState(_nextState[idx]);
        }
    }
}