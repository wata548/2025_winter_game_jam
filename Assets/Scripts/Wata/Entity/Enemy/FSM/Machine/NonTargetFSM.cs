using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Entity.Enemy.FSM {
    [RequireComponent(typeof(Enemy))]
    public class NonTargetFSM: MonoBehaviour {
        //==================================================||Fields 
        private IReadOnlyDictionary<EnemyState, StateBase> _stateMap;
        private StateBase _logic = null;
       
        //==================================================||Properties 
        [Header("Fsm")] 
        [SerializeField] private List<StatePair> _stateList;
        public EnemyState State { get; private set; } = default;
        public Enemy Enemy { get; private set; }

        //==================================================||Methods 
        public void ChangeState(EnemyState pTargetState) {
            
            if (_stateMap.TryGetValue(pTargetState, out var newState)) {
                Debug.LogError($"{name} doesn't have {pTargetState} state");
                return;
            }

            if (_logic != null && State == pTargetState)
                return;
            
            _logic?.OnExit(this);
            State = pTargetState;
            _logic = _stateMap[pTargetState];
            _logic.OnEnter(this);
        }

        //==================================================||Unity        
        protected virtual void Awake() {
            _stateMap = _stateList.ToDictionary(pair => pair.State, pair => pair.StateLogic);
            Enemy = GetComponent<Enemy>();
        }

        protected virtual void Update() {
            _logic?.Update(this);
        }
    }
}