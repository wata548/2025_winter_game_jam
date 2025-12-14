using UnityEngine;

namespace Entity.Enemy.FSM {
    public abstract class StateBase : ScriptableObject {
        public abstract void OnEnter(NonTargetFSM pTarget);
        public abstract void OnExit(NonTargetFSM pTarget);
        public abstract void Update(NonTargetFSM pTarget);
    }
}