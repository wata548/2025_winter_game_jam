using System;
using UnityEngine;

namespace Entity.Enemy.FSM {
    
    //It's exist meaning is to implement spread of aggro
    public class TargetFSM : NonTargetFSM {
        //==================================================||Fields
        [SerializeField] private SpreadAggroBase _aggro;
        private GameObject _target;
        
        //==================================================||Properties 
        public GameObject Target {
            get => _target;
            set {
                _target = value;
                _aggro.OnGetAggro(this);
                SetActiveSpecialAttackTimer(_target != null);
            }
        }
        
       //==================================================||Unity 
       protected override void Start() {
           base.Start();
           SetActiveSpecialAttackTimer(false);
       }
    }
}