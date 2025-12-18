using System;
using Entity.Enemy.Behaviour;
using Extension.Test;
using Game.Item;
using Physic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Entity.Enemy {
    [RequireComponent(typeof(Movement))]
    public class Enemy: MonoBehaviour, IEntity {

        [TestMethod]
        private void Death() {
            GetDamage(Int32.MaxValue);
        }
        
        //==================================================||Properties 
        [field: SerializeField] public float Speed { get; private set; }
        [field: SerializeField] public int MaxHp { get; private set; }
        [field: SerializeField] public AttackMotion AttackMotion { get; private set; }
        [field: SerializeField] public SpecialAttackMotion SpecialAttackMotion { get; private set; }
        public int Hp { get; private set; }
        public bool IsDead { get; private set; } = false;
        public int PoisonStack { get; private set; }

        [field: Space]
        [Header("Damage")]
        [field: SerializeField] public int GivePoisonStack { get; set; } = 0;
        [field: SerializeField] public int Power { get; set; } = 1;

        [FormerlySerializedAs("_droopPeel")]
        [Space, Header("Peel")] 
        [SerializeField] private int _droopPeelCnt;
        [SerializeField] private Magnet _peel;
        
        public Movement Movement { get; private set; } = null;
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

        public void AddPoisonStack(int pAmount) => PoisonStack += pAmount;
        public void RemovePoisonStack(int pAmount) => PoisonStack = Mathf.Max(0, PoisonStack - pAmount);

        protected virtual void OnDamage(int pAmount) {
            Debug.Log($"{name} is hit({pAmount}).");
        }

        protected virtual void OnHeal(int pAmount) {
            Debug.Log($"{name} is healed({pAmount}).");
        }
        protected virtual void OnDeath() {
            Debug.Log($"{name} is dead.");
            
            var peel = Instantiate(_peel);
            peel.transform.position = transform.position;
            peel.SetItemValue(_droopPeelCnt);
            
            Destroy(gameObject);
        }
        
        public void SetUp() {
            Hp = MaxHp;
            Movement = GetComponent<Movement>();
        }
        
        //==================================================||Unity

        private void OnCollisionEnter(Collision other) {
            if (!other.transform.CompareTag("Player"))
                return;

            var entity = other.gameObject.GetComponent<IEntity>();
            entity.GetDamage(Power);
            entity.AddPoisonStack(GivePoisonStack);
        }
    }
}