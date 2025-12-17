using UnityEngine;
using System;
using Extension.Test;

namespace Game.Player.Stats
{
    public class PoisonSystem : MonoBehaviour
    {

        public event Action<int> OnPoisonDamageApplied;

        private HealthSystem m_healthSystem = null;
        private int m_currentPoisonStack = 0;

        [SerializeField] private float m_poisonTickInterval = 1f;
        [SerializeField] private int m_damagePerStack = 1;

        private float m_poisonTickTimer = 0f;

        private void Awake()
        {
            m_healthSystem = GetComponent<HealthSystem>();
            if (m_healthSystem == null)
            {
                Debug.LogError("[PoisonSystem] HealthSystem is missing!");
            }
        }

        private void Update()
        {
            UpdatePoison();
        }

        private void UpdatePoison()
        {
            if (m_currentPoisonStack <= 0) return;

            m_poisonTickTimer -= Time.deltaTime;

            if (m_poisonTickTimer <= 0f)
            {
                ApplyPoisonDamage();
                m_poisonTickTimer = m_poisonTickInterval;
            }
        }

        public void AddPoisonStack(int pAmount)
        {
            m_currentPoisonStack += pAmount;
            Debug.Log($"[PoisonSystem] Poison stack increased -> {m_currentPoisonStack}!");
        }

        public void RemovePoisonStack(int pAmount)
        {
            m_currentPoisonStack -= pAmount;
            m_currentPoisonStack = Mathf.Max(0, m_currentPoisonStack);
            Debug.Log($"[PoisonSystem] Poison stack decreased -> {m_currentPoisonStack}!");
        }

        private void ApplyPoisonDamage()
        {
            int damage = m_currentPoisonStack * m_damagePerStack;
            m_healthSystem.TakeDamage(damage);
            OnPoisonDamageApplied?.Invoke(damage);
            Debug.Log($"[PoisonSystem] Poison damage applied: {damage} (Stack: {m_currentPoisonStack})!");
        }

        [TestMethod("Test Add Poison Stack")]
        private void TestAddPoisonStack()
        {
            AddPoisonStack(2);
        }

        [TestMethod("Test Remove Poison Stack")]
        private void TestRemovePoisonStack()
        {
            RemovePoisonStack(1);
        }

        public int CurrentPoisonStack => m_currentPoisonStack;
    }
}