using UnityEngine;
using Physic;

namespace Entity.Enemy.Behaviour.Empire {
    public class Attack: AnimationBehaviour {
        [SerializeField] private Missile _missile;
        protected override void OnMiddle() {
            var movement = GetComponent<Movement>();
            var newMissile = Instantiate(_missile);
            newMissile.transform.position = GetComponent<Collider>().bounds.center;
            var rotation = newMissile.transform.rotation.eulerAngles;
            rotation.y = movement.SeeRight ? 90 : -90;
            newMissile.transform.rotation = Quaternion.Euler(rotation);
        }
    }
}