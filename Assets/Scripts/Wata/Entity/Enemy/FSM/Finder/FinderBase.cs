using UnityEngine;

namespace Entity.Enemy.FSM {
    
    public abstract class FinderBase: ScriptableObject {
        public abstract bool FindPlayer(TargetFSM pFsm, out Transform pPlayer);
    }
}