using System;
using System.Collections.Generic;
using System.Linq;
using Physic;
using UnityEngine;

namespace Entity.Enemy.FSM {
    [RequireComponent(typeof(Enemy))]
    public class NonTargetFSM: MonoBehaviour {
        //==================================================||Fields 
        protected Dictionary<EnemyState, StateBase> _stateMap;
        private StateBase _logic = null;
        
        private float _specialAttackTerm = -1;
        private float _remainSpecialAttack = 0;
        private bool _onSpecialTimer = false;
        private bool _usableSpecialAttack = false;      
        //==================================================||Properties 
        [Header("Fsm")] 
        
        [SerializeField] private List<StatePair> _stateList;
        [field: SerializeField] public EnemyState State { get; private set; } = default;
        public Enemy Enemy { get; private set; }
        public Movement Movement => Enemy.Movement;

        //==================================================||Methods 
        public void SetActiveSpecialAttackTimer(bool pIsActive) {
            _onSpecialTimer = pIsActive;
            _usableSpecialAttack = false;
            _remainSpecialAttack = _specialAttackTerm;
        }
        
        public bool UseSpecialAttack() {
            var result = _usableSpecialAttack;
            _usableSpecialAttack = false;
            return result;
        }
        
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

        public virtual void ChangeLogic(EnemyState pState, StateBase pLogic) {
            if(!_stateMap.TryAdd(pState, pLogic))
                _stateMap[pState] = pLogic;
        }
        
        //==================================================||Unity        
        protected virtual void Start() {
            
            Enemy = GetComponent<Enemy>();
            Enemy.SetUp();
            
            _stateMap = _stateList.ToDictionary(pair => pair.State, pair => pair.StateLogic);
            ChangeState(State);
            
            _specialAttackTerm = Enemy.SpecialAttackMotion?.Term ?? -1;
            _remainSpecialAttack = _specialAttackTerm;
            
            SetActiveSpecialAttackTimer(true);
        }

        protected virtual void Update() {
            if (_specialAttackTerm > 0 && _onSpecialTimer) {
                _remainSpecialAttack -= Time.deltaTime;
                if (_remainSpecialAttack <= 0) {
                    _remainSpecialAttack = _specialAttackTerm;
                    _usableSpecialAttack = true;
                }
            }
            _logic?.OnUpdate(this);
        }
    }
}