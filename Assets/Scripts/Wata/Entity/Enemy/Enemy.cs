using System;
using Entity.Enemy.FSM;
using Physic;
using UnityEngine;

namespace Entity.Enemy {
    [RequireComponent(typeof(Movement))]
    public abstract class Enemy: MonoBehaviour, IEntity {
        
        
        //==================================================||Properties 
        [field: SerializeField] public float Speed { get; private set; }
        [field: SerializeField] public int MaxHp { get; private set; }
        public int Hp { get; private set; }
        public bool IsDead { get; private set; } = false;

        //==================================================||Methods 
        public void GetDamage(int pAmount) {
            Hp -= pAmount;
            OnDamage(pAmount);
            
            if (Hp < 0) {
                Hp = 0;
                IsDead = true;
                OnDeath();
            }
        }

        public void GetHeal(int pAmount) {
            Hp = Math.Max(Hp + pAmount, MaxHp);
            OnHeal(pAmount);
        }

        protected virtual void OnDamage(int pAmount) {
            Debug.Log($"{name} is hit({pAmount}).");
        }

        protected virtual void OnHeal(int pAmount) {
            Debug.Log($"{name} is healed({pAmount}).");
        }
        protected virtual void OnDeath() {
            Debug.Log($"{name} is dead.");
            Destroy(gameObject);
        }

        //==================================================||Unity
        private void Awake() {
            Hp = MaxHp;
        }

        private void Update() {
            if (IsDead)
                return;
        }
    }
}