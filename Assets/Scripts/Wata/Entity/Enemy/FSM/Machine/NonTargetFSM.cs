using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Entity.Enemy.FSM {
    [RequireComponent(typeof(Enemy))]
    public class NonTargetFSM: MonoBehaviour {
        //==================================================||Fields 
        private IReadOnlyDictionary<EnemyState, StateBase> _stateMap;
        private StateBase _curState = null;
       
        //==================================================||Properties 
        [Header("Fsm")] 
        [SerializeField] private List<StatePair> _stateList;

        //==================================================||Methods 
        public void ChangeState(EnemyState pTargetState) {
            
            if (_stateMap.TryGetValue(pTargetState, out var newState)) {
                Debug.LogError($"{name} doesn't have {pTargetState} state");
                return;
            }
            
            _curState?.OnExit(this);
            _curState = _stateMap[pTargetState];
            _curState.OnEnter(this);
        }

        //==================================================||Unity        
        protected virtual void Awake() {
            _stateMap = _stateList.ToDictionary(pair => pair.State, pair => pair.StateLogic);
        }

        protected virtual void Update() {
            _curState?.Update(this);
        }
    }
}