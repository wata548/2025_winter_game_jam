using System;
using UnityEngine;

namespace Entity.Enemy.Behaviour.Empire {
    public class PoisonSmoke: MonoBehaviour {

        [SerializeField] private int _addPoisonStack = 5;  
        private const float REMAIN_TIME = 8;
        private float _remain = REMAIN_TIME;
        private IEntity _target = null;
        private float _sum = 0; 
        
        private void OnTriggerEnter(Collider other) {
            if (other.CompareTag("Player")) {
                _target = other.GetComponent<IEntity>();
                _sum = 0;
            }
        }

        private void OnTriggerExit(Collider other) {
            if (other.CompareTag("Player")) {
                _target = null;
            }
        }

        private void Awake() {
            foreach(var particle in GetComponentsInChildren<ParticleSystem>())
                particle.Play();
        }
        
        private void Update() {
            _remain -= Time.deltaTime;
            if (_remain <= 0)
                Destroy(gameObject);
            
            if (_target == null)
                return;

            _sum += Time.deltaTime;
            if (_sum >= 1) {
                _sum -= 1;
                _target.AddPoisonStack(_addPoisonStack);
            }
        }
    }
}