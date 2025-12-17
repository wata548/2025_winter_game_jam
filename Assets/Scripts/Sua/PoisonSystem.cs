using UnityEngine;
using System;
using Extension.Test;

namespace Game.Player.Stats
{
    public class PoisonSystem : MonoBehaviour
    {

        public event Action<int> OnPoisonStackChanged;
        public event Action OnPoisoned;
        public event Action OnPoisonCured;

        private HealthSystem m_healthSystem = null;
        private int m_currentPoisonStack = 0;
        private bool m_isPoisoned = false;

        private float m_lastStackAddTime = 0f;
        private float m_stackDecayTimer = 0f;
        private const int POISON_STACK_MAX = 10;
        private const float STACK_DECAY_DELAY = 10f;
        private const float STACK_DECAY_INTERVAL = 1f;
        private const float POISON_DAMAGE_MULTIPLIER = 2f;

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
            UpdateStackDecay();
        }

        public void AddPoisonStack(int pAmount)
        {
            if (m_currentPoisonStack >= POISON_STACK_MAX) return;

            m_currentPoisonStack = Mathf.Min(m_currentPoisonStack + pAmount, POISON_STACK_MAX);
            m_lastStackAddTime = Time.time;

            OnPoisonStackChanged?.Invoke(m_currentPoisonStack);

            // 중독..
            if (m_currentPoisonStack >= POISON_STACK_MAX && !m_isPoisoned)
            {
                m_isPoisoned = true;
                OnPoisoned?.Invoke();
                Debug.Log("[PoisonSystem] Player is Poisoned! (2x damage)");
            }

            Debug.Log($"[PoisonSystem] Poison stack: {m_currentPoisonStack}/{POISON_STACK_MAX}");
        }

        public void RemovePoisonStack(int pAmount)
        {
            m_currentPoisonStack = Mathf.Max(0, m_currentPoisonStack - pAmount);
            m_lastStackAddTime = Time.time;
            m_stackDecayTimer = 0f;

            OnPoisonStackChanged?.Invoke(m_currentPoisonStack);

            // 해독!
            if (m_currentPoisonStack < POISON_STACK_MAX && m_isPoisoned)
            {
                m_isPoisoned = false;
                OnPoisonCured?.Invoke();
                Debug.Log("[PoisonSystem] Poison cured!");
            }

            Debug.Log($"[PoisonSystem] Poison stack: {m_currentPoisonStack}/{POISON_STACK_MAX}");
        }

        private void UpdateStackDecay()
        {
            if (m_currentPoisonStack <= 0) return;

            float timeSinceLastAdd = Time.time - m_lastStackAddTime;

            if (timeSinceLastAdd >= STACK_DECAY_DELAY)
            {
                m_stackDecayTimer += Time.deltaTime;

                if (m_stackDecayTimer >= STACK_DECAY_INTERVAL)
                {
                    RemovePoisonStack(1);
                    m_stackDecayTimer = 0f;
                }
            }
        }

        [TestMethod("Test Add Poison Stack")]
        private void TestAddPoisonStack()
        {
            AddPoisonStack(3);
        }

        [TestMethod("Test Remove Poison Stack")]
        private void TestRemovePoisonStack()
        {
            RemovePoisonStack(1);
        }

        [TestMethod("Test Max Poison")]
        private void TestMaxPoison()
        {
            AddPoisonStack(10);
        }

        public int CurrentPoisonStack => m_currentPoisonStack;
        public bool IsPoisoned => m_isPoisoned;
        public float GetDamageMultiplier() => m_isPoisoned ? POISON_DAMAGE_MULTIPLIER : 1f;
    }
}