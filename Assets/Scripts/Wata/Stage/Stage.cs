using System.Collections.Generic;
using System.Linq;
using Entity.Enemy;
using Unity.VisualScripting;
using UnityEngine;

namespace Stage {
    public class Stage : MonoBehaviour {
        [SerializeField] private Transform _playerSpawn;
        private IEnumerable<Enemy> _enemies = null;

        public void RemoveAllEnemies() {
            foreach (var enemy in _enemies) {
                Destroy(enemy);
            }
        }
        
        public bool ExistMonster {
            get {
                if (_enemies == null)
                    return false;
                _enemies = _enemies.Where(enemy => enemy != null && !enemy.IsDead);
                return !_enemies.Any();
            }
        }

        private void Awake() {
            _enemies = GameObject.FindGameObjectsWithTag("Enemy")
                .Where(obj => obj != null && obj.TryGetComponent<Enemy>(out _))
                .Select(obj => obj.GetComponent<Enemy>());
            GameObject.FindWithTag("Player").transform.position = _playerSpawn.position;
        }
    }
}