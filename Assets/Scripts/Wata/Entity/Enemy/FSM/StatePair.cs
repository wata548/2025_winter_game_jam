using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Entity.Enemy.FSM {
    
    [Serializable]
    public class StatePair {
        [field: SerializeField] public EnemyState State { get; private set; }
        [field: SerializeField] public StateBase StateLogic { get; private set; }
    }
}