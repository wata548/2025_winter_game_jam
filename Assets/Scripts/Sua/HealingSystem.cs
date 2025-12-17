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
        private int m_healCount = 0;

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

        private void Update()
        {
            UpdateHealing();
        }

        public void StartHealing()
        {
            if (m_healthSystem.Hp >= m_healthSystem.MaxHp) return;
            m_isHealing = true;
            m_healTimer = 0f;
            m_healCount = 0;
            Debug.Log("[HealingSystem] Started healing");
        }

        public void StopHealing()
        {
            if (m_isHealing)
            {
                Debug.Log("[HealingSystem] Stopped healing");
            }
            m_isHealing = false;
            m_healTimer = 0f;
            m_healCount = 0;
        }

        private void UpdateHealing()
        {
            if (!m_isHealing) return;

            m_healTimer += Time.deltaTime;

            if (m_healTimer >= m_healStartDelay)
            {
                float timeSinceStart = m_healTimer - m_healStartDelay;
                int currentHealCount = Mathf.FloorToInt(timeSinceStart / m_healInterval);

                if (currentHealCount > m_healCount)
                {
                    if (m_threadSystem.ConsumeThread(m_threadCostPerHeal))
                    {
                        m_healthSystem.GetHeal(m_healthPerHeal);
                        m_healCount = currentHealCount;
                        Debug.Log("[HealingSystem] Healing!");
                    }
                    else
                    {
                        StopHealing();
                    }
                }
            }
        }

        public bool IsHealing => m_isHealing;
    }
}