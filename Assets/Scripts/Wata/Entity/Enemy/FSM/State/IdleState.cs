using UnityEngine;

namespace Entity.Enemy.FSM {
    [CreateAssetMenu(menuName = "EnemyFSM/Idle")]
    public class IdleState: StateBase {

        //==================================================||Fields 
        [SerializeField] private float _minIdleTime;
        [SerializeField] private float _maxIdleTime;
        
        [Space, Header("Finder")]
        [SerializeField] private FinderBase _finder;
        [SerializeField] private EnemyState _whenFindState;
        private float _remainTime;
        
        //==================================================||Methods 
        public override void OnEnter(NonTargetFSM pTarget) {
            _remainTime = Random.Range(_minIdleTime, _maxIdleTime);
        }

        public override void OnExit(NonTargetFSM pTarget) {
        }

        public override void OnUpdate(NonTargetFSM pTarget) {
            _remainTime -= Time.deltaTime;
            
            if (pTarget is TargetFSM targetFsm && _finder.FindPlayer(targetFsm, out var player)) {
                targetFsm.Target = player.gameObject;
                pTarget.ChangeState(_whenFindState);
                return;
            }
            
            if (_remainTime > 0)
                return;
            
            ChangeState(pTarget);
        }
    }
}