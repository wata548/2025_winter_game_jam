using System;
using System.Collections.Generic;
using System.Linq;
using Physic;
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
        [field: SerializeField] public EnemyState State { get; private set; } = default;
        public Enemy Enemy { get; private set; }
        public Movement Movement => Enemy.Movement;

        //==================================================||Methods 
        public void ChangeState(EnemyState pTargetState) {
            
            if (!_stateMap.TryGetValue(pTargetState, out var newState)) {
                Debug.LogError($"{name} doesn't have {pTargetState} state");
                return;
            }

            Movement.SetHorizonPower(0);

            _logic?.OnExit(this);
            State = pTargetState;
            _logic = newState;
            _logic.OnEnter(this);
        }

        //==================================================||Unity        
        protected virtual void Start() {
            
            Enemy = GetComponent<Enemy>();
            Enemy.SetUp();
            
            _stateMap = _stateList.ToDictionary(pair => pair.State, pair => pair.StateLogic);
            ChangeState(State);
        }

        protected virtual void Update() {
            _logic?.OnUpdate(this);
        }
    }
}