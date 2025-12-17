using System;
using System.Collections.Generic;
using System.Linq;
using Entity.Enemy;
using UnityEngine;

namespace Stage {
    public class Stage : MonoBehaviour {
        [SerializeField] private Stage _nextStage;
        [SerializeField] private Transform _playerSpawn;
        private IEnumerable<Enemy> _enemies = null;

        public bool Moveable {
            get {
                if (_enemies == null)
                    return false;
                _enemies = _enemies.Where(enemy => enemy != null && !enemy.IsDead);
                return !_enemies.Any();
            }
        }

        private void Awake() {
            _enemies = GameObject.FindGameObjectsWithTag("Enemy")
                .Select(obj => obj.GetComponent<Enemy>());
            GameObject.FindWithTag("Player").transform.position = _playerSpawn.position;
        }
    }
}