using System.Collections;
using UnityEngine;

namespace Entity.Enemy.Behaviour {
    public class TestBehaviour: AttackMotion {
        
        //==================================================||Properties
        public override bool IsEnd { get; protected set; } = true;
        
       //==================================================||Methods 
        public override void Play() {
            IsEnd = false;
            StartCoroutine(Turn());
        }

        private IEnumerator Turn(float pDuration = 0.5f, float pDelay = 0.3f) {
            
            var speed = 1f / pDuration * 360f;
            var rotation = transform.rotation.eulerAngles;
            var origin = rotation;
            var sum = 0f;
            while (sum < 360) {
                var delta = speed * Time.deltaTime;
                rotation.y += delta;
                sum += delta;

                transform.rotation = Quaternion.Euler(rotation);
                yield return null;
            }
            transform.rotation = Quaternion.Euler(origin);
            yield return new WaitForSeconds(pDelay);
            IsEnd = true;
        }
    }
}