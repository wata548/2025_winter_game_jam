using System.Collections;
using UnityEngine;

namespace Entity.Enemy.Behaviour {
    public class TestBehaviour2: SpecialAttackMotion {
        [field: SerializeField] public override float Term { get; protected set; } = 5;
        public override bool IsEnd { get; protected set; } = true;
        
        public override void Play() {
            IsEnd = false;
            StartCoroutine(Angry());
        }

        private IEnumerator Angry(float pDuration = 0.5f) {

            var render = GetComponent<MeshRenderer>();
            var origin = render.material.color;
            render.material.color = Color.red;
            yield return new WaitForSeconds(pDuration);
            render.material.color = origin;
            IsEnd = true;
        }

    }
}