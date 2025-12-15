using UnityEngine;

namespace Entity.Enemy.FSM {
    [CreateAssetMenu(menuName = "EnemyFSM/Idle")]
    public class IdleState: StateBase {

       //==================================================||Fields 
        [SerializeField] private float _minIdleTime;
        [SerializeField] private float _maxIdleTime;
        [SerializeField] private EnemyState[] _nextState;
        private float _remainTime;
        
       //==================================================||Methods 
        public override void OnEnter(NonTargetFSM pTarget) {
            _remainTime = Random.Range(_minIdleTime, _maxIdleTime);
        }

        public override void OnExit(NonTargetFSM pTarget) {
        }

        public override void Update(NonTargetFSM pTarget) {
            _remainTime -= Time.deltaTime;
            if (_remainTime > 0)
                return;

            var idx = Random.Range(0, _nextState.Length);
            pTarget.ChangeState(_nextState[idx]);
        }
    }
}