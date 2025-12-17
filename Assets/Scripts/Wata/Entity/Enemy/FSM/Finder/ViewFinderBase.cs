using System.Linq;
using UnityEngine;

namespace Entity.Enemy.FSM {
    
    [CreateAssetMenu(menuName = "Finder/View")]
    public class ViewFinderBase: FinderBase {

        [SerializeField] private float _maxDistance;
        public override bool FindPlayer(TargetFSM pFsm, out Transform pPlayer) {
            var direction = pFsm.Movement.SeeRight ? 1 : -1;
            var scale = pFsm.Movement.Collider.bounds.size;
            
            var start = pFsm.Movement.Collider.bounds.center 
                        + Vector3.right * (direction * scale.x / 2);
            var player = Physics.RaycastAll(
                    start,
                    Vector3.right * direction,
                    _maxDistance
                ).Where(hit => hit.transform.CompareTag("Player"))
                .Select(hit => hit.transform)
                .FirstOrDefault();

            
            Debug.DrawLine(start, start + Vector3.right * direction * _maxDistance);
            
            pPlayer = player;
            return player != null;
        }
    }
}