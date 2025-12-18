using UnityEngine;
using Extension.Test;

namespace Game.Player.Stats
{
    public class HealingSystem : MonoBehaviour
    {
        private HealthSystem m_healthSystem = null;
        private ThreadSystem m_threadSystem = null;
        private EnhancementSystem m_enhancementSystem = null;

        private bool m_isHealing = false;
        private float m_healTimer = 0f;
        private float m_healIntervalTimer = 0f;

        [SerializeField] private float m_healStartDelay = 2f;
        [SerializeField] private float m_healInterval = 1f;
        [SerializeField] private int m_threadCostPerHeal = 10;
        [SerializeField] private int m_healthPerHeal = 1;

        private void Awake()
        {
            m_healthSystem = GetComponent<HealthSystem>();
            m_threadSystem = GetComponent<ThreadSystem>();
            m_enhancementSystem = GetComponent<EnhancementSystem>();

            if (m_healthSystem == null) Debug.LogError("[HealingSystem] HealthSystem is missing!");
            if (m_threadSystem == null) Debug.LogError("[HealingSystem] ThreadSystem is missing!");
            if (m_enhancementSystem == null) Debug.LogError("[HealingSystem] EnhancementSystem is missing!");
        }

        private void Start()
        {
            m_healthSystem.OnPlayerDamaged += OnPlayerDamaged;
        }

        private void OnDestroy()
        {
            if (m_healthSystem != null)
            {
                m_healthSystem.OnPlayerDamaged -= OnPlayerDamaged;
            }
        }

        private void Update()
        {
            UpdateHealing();
        }

        public void StartHealing()
        {
            if (m_healthSystem.Hp >= m_healthSystem.MaxHp)
            {
                Debug.Log("[HealingSystem] Already at max health!");
                return;
            }

            if (m_isHealing) return;

            m_isHealing = true;
            m_healTimer = 0f;
            m_healIntervalTimer = 0f;
            Debug.Log($"[HealingSystem] Started healing. HP: {m_healthSystem.Hp}/{m_healthSystem.MaxHp}");
        }

        public void StopHealing()
        {
            if (m_isHealing)
            {
                Debug.Log($"[HealingSystem] Stopped healing. HP: {m_healthSystem.Hp}/{m_healthSystem.MaxHp}");
            }
            m_isHealing = false;
            m_healTimer = 0f;
            m_healIntervalTimer = 0f;
        }

        private void OnPlayerDamaged()
        {
            if (m_isHealing)
            {
                Debug.Log("[HealingSystem] Healing interrupted by damage!");
                StopHealing();
            }
        }

        private void UpdateHealing()
        {
            if (!m_isHealing) return;

            if (m_healthSystem.Hp >= m_healthSystem.MaxHp)
            {
                StopHealing();
                return;
            }

            m_healTimer += Time.deltaTime;

            if (m_healTimer < m_healStartDelay)
            {
                return;
            }

            float healIntervalWithEnhancement = m_healInterval * m_enhancementSystem.GetHealingSpeedMultiplier();
            m_healIntervalTimer += Time.deltaTime;

            if (m_healIntervalTimer >= healIntervalWithEnhancement)
            {
                m_healIntervalTimer -= healIntervalWithEnhancement;

                if (m_threadSystem.ConsumeThread(m_threadCostPerHeal))
                {
                    m_healthSystem.GetHeal(m_healthPerHeal);
                    Debug.Log($"[HealingSystem] Healed! HP: {m_healthSystem.Hp}/{m_healthSystem.MaxHp} | Thread: -{m_threadCostPerHeal}");
                }
                else
                {
                    Debug.Log("[HealingSystem] Not enough thread! Stopping healing.");
                    StopHealing();
                }
            }
        }

        [TestMethod("Test Start Healing")]
        private void TestStartHealing()
        {
            StartHealing();
        }

        [TestMethod("Test Stop Healing")]
        private void TestStopHealing()
        {
            StopHealing();
        }

        public bool IsHealing => m_isHealing;
    }
}