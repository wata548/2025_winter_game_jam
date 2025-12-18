using Entity.Enemy;
using Stage;
using UnityEngine;

public class Summoner : Enemy {

    [SerializeField] private Enemy[] _target;
    protected override void OnDeath() {
        var newEnemy = Instantiate(_target[Random.Range(0, _target.Length)]);
        newEnemy.transform.position = transform.position;
        StageManager.Instance.Refresh();
        base.OnDeath();
        
    }
}