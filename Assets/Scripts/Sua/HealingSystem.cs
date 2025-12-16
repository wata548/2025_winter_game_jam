using UnityEngine;

namespace Game.Player.Stats
{
    public class HealingSystem : MonoBehaviour
    {

        private HealthSystem m_healthSystem = null;
        private ThreadSystem m_threadSystem = null;
        private EnhancementSystem m_enhancementSystem = null;

        private bool m_isHealing = false;
        private float m_healTimer = 0f;

        [SerializeField] private float m_healTick = 0.1f;
        [SerializeField] private int m_threadCostPerHeal = 10;
        [SerializeField] private int m_healthPerHeal = 1;

        private void Awake()
        {
            m_healthSystem = GetComponent<HealthSystem>();
            m_threadSystem = GetComponent<ThreadSystem>();
            m_enhancementSystem = GetComponent<EnhancementSystem>();

            if (m_healthSystem == null)
            {
                Debug.LogError("[HealingSystem] HealthSystem is missing!");
            }
            if (m_threadSystem == null)
            {
                Debug.LogError("[HealingSystem] ThreadSystem is missing!");
            }
            if (m_enhancementSystem == null)
            {
                Debug.LogError("[HealingSystem] EnhancementSystem is missing!");
            }
        }

        private void Update()
        {
            UpdateHealing();
        }

        public void StartHealing()
        {
            if (m_healthSystem.CurrentHealth >= m_healthSystem.CurrentHealth)
            {
                return;
            }
            m_isHealing = true;
            m_healTimer = 0f;
        }

        public void StopHealing()
        {
            m_isHealing = false;
            m_healTimer = 0f;
        }

        private void UpdateHealing()
        {
            if (!m_isHealing) return;

            float enhancedHealTick = m_healTick * m_enhancementSystem.GetHealingSpeedMultiplier();
            m_healTimer += Time.deltaTime;

            if (m_healTimer >= enhancedHealTick)
            {
                if (m_threadSystem.ConsumeThread(m_threadCostPerHeal))
                {
                    m_healthSystem.Heal(m_healthPerHeal);
                    m_healTimer = 0f;
                }
                else
                {
                    StopHealing();
                }
            }
        }

        public bool IsHealing => m_isHealing;
    }
}