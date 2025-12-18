using UnityEngine;

namespace Entity.Enemy.Behaviour.Empire {
    public class SpecialAttack: SpecialAnimationBehaviour {

        [SerializeField] private PoisonSmoke _smoke;

        protected virtual void OnMiddle() {
            var newObj = Instantiate(_smoke);
            newObj.transform.position = GetComponent<Collider>().bounds.center;
        }
        public override float Term { get; protected set; } = 6;
    }
}