using TMPro;
using UnityEngine;

namespace Entity.Enemy.Behaviour.BlueBelt {
    [RequireComponent(typeof(Rigidbody))]
    public class WindStrom: MonoBehaviour {
        [SerializeField] private ParticleSystem _particle;
        private Rigidbody _rigid;
        private const float REMAIN = 3;
        private float _remain = REMAIN;

        public void GivePower(Vector3 pPower) =>
            _rigid.linearVelocity = pPower;

        private void Update() {
            _remain -= Time.deltaTime;
            if (_remain <= 0)
                Destroy(gameObject);
        }
        
        private void Awake() {
            _rigid = GetComponent<Rigidbody>();
        }
    }
}