using System;
using UnityEngine;

namespace Entity.Enemy.Behaviour.Empire {
    
    [RequireComponent(typeof(Rigidbody))]
    public class Missile : MonoBehaviour {
        [SerializeField] private int _damage = 1;
        [SerializeField] private int _poisonStack = 5;
        [SerializeField] private float _duration = 1;
        [SerializeField] private float _speed = 1;
        [SerializeField] private float _descendent = 1;
        private Rigidbody _rigid;
        
        private void OnTriggerEnter(Collider other) {
            if (!other.CompareTag("Player"))
                return;

            var entity = other.GetComponent<IEntity>();
            entity.GetDamage(_damage);
            entity.AddPoisonStack(_poisonStack);
        }

        private void Update() {
            var speed = _rigid.linearVelocity.magnitude;
            speed -= Time.deltaTime * _descendent;
            _rigid.linearVelocity = _rigid.linearVelocity.normalized * speed;
            
            if (speed <= 0.1f) {
                Destroy(gameObject);
            }
        } 
        
        private void Start() {
            _rigid = GetComponent<Rigidbody>();
            _rigid.useGravity = false;
            _rigid.linearVelocity = transform.forward * _speed;
        }
    }
}