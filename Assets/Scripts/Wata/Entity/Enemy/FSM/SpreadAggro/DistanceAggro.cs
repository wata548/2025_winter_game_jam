using System.Linq;
using UnityEngine;

namespace Entity.Enemy.FSM {
    [CreateAssetMenu(menuName = "AggroSpread/Distance")]
    public class DistanceAggro: SpreadAggroBase {
        [SerializeField] private float _radis = 3;
        
        public override void OnGetAggro(TargetFSM pFsm) {
            var pos = pFsm.transform.position;
            var targets = Physics.OverlapSphere(pos, _radis)
                .Select(colider => colider.GetComponent<TargetFSM>())
                .Where(fsm => fsm != null && fsm.Target == null);
            SetTarget(pFsm, targets);
        }
    }
}