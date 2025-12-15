using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UIElements.Experimental;

namespace Entity.Enemy.FSM {
    [CreateAssetMenu(menuName = "EnemyFSM/Patrol")]
    public class Patrol: StateBase {

       //==================================================||Fields 
        [SerializeField] private float _minLength;
        [SerializeField] private float _maxLength;
        private float _remainDistance;
        private bool _isSetted = false;
        
       //==================================================||Methods 
       public void SetUp(NonTargetFSM pTarget) {

           if (_isSetted || !pTarget.Movement.IsGround)
               return;

           Debug.Log("Setted");
           _isSetted = true;
           var dir = pTarget.Enemy.Speed;
           var right = pTarget.Movement.IsMovable(Vector3.right * dir);
           var left = pTarget.Movement.IsMovable(Vector3.left * dir);
           if (left && right)
               dir *= Random.Range(0, 2) == 0 ? 1 : -1;
           else if (left)
               dir *= -1;
           else if (!right) {
               ChangeState(pTarget);
               return;
           }

           _remainDistance = Random.Range(_minLength, _maxLength);
           pTarget.Movement.SetHorizonPower(dir);
       }
       
        public override void OnEnter(NonTargetFSM pTarget) {
            Debug.Log("Enter Patrol");
            _isSetted = false;
            SetUp(pTarget);
        }
        public override void OnExit(NonTargetFSM pTarget) {
            Debug.Log("Exit Patrol");
        }

        public override void OnUpdate(NonTargetFSM pTarget) {
            
            SetUp(pTarget);
            if (!_isSetted) 
                return;
            
            _remainDistance -= Time.deltaTime * pTarget.Enemy.Speed;
            
            if (_remainDistance <= 0) {
                pTarget.Movement.SetHorizonPower(0);
                ChangeState(pTarget);
                return;
            }

            if (!pTarget.Movement.IsMovable(Vector3.zero)) {
                pTarget.Movement.SetHorizonPower(0);
                ChangeState(pTarget);
            }
        }
    }
}