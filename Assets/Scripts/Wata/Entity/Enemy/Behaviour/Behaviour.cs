using Extension.Test;
using UnityEngine;

namespace Entity.Enemy.Behaviour {
    public abstract class Motion: MonoBehaviour {
        public abstract bool IsEnd { get; protected set; }
        [TestMethod]
        public abstract void Play();
    }
    
    public abstract class AttackMotion: Motion{}

    public abstract class SpecialAttackMotion : Motion {
        public abstract float Term { get; protected set; }
    }
}