using System.Collections.Generic;
using UnityEngine;

namespace Entity.Enemy.FSM {
    public abstract class SpreadAggroBase: ScriptableObject {
        public abstract void OnGetAggro(TargetFSM pFsm);

        protected void SetTarget(TargetFSM pFsm, IEnumerable<TargetFSM> pTargetFsms) {
            var target = pFsm.Target;
            foreach (var targetFsm in pTargetFsms) {
                targetFsm.Target = target;
                targetFsm.ChangeState(EnemyState.Chase);
            }
        }
    }
}