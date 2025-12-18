using System.Linq;
using Entity.Enemy.FSM;
using UnityEngine;

namespace Entity.Enemy.Behaviour.Five {
    public class Smash: SpecialAnimationBehaviour {
        public override float Term { get; protected set; } = 8;
        [SerializeField] private float _radius = 3;
        [SerializeField] private ParticleSystem _particle;

        protected override void OnMiddle() {
            var particle = Instantiate(_particle);
            var fsm = GetComponent<TargetFSM>();
            
            var pos = fsm.Movement.Collider.bounds.center;
            pos.y -= fsm.Movement.Collider.bounds.size.y / 2;
            particle.transform.position = pos;
            particle.transform.localScale = Vector3.one * (_radius * 0.7f);

            var player = Physics.OverlapSphere(pos, _radius).FirstOrDefault(collider => collider.CompareTag("Player"));
            player?.GetComponent<IEntity>()?.GetDamage(1);
        }
    }
}