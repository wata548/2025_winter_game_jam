using UnityEngine;

namespace Entity.Enemy.FSM {
    
    //It's exist meaning is to implement spread of aggro
    public class TargetFSM : NonTargetFSM {
        //==================================================||Properties 
        public bool IsSelectTarget => Target != null;
        public GameObject Target { get; private set; }
    }
}