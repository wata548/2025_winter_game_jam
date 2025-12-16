using UnityEngine;
using System;
using Game.Player.Movement;

namespace Game.Player.Stats
{
    public class EnhancementSystem : MonoBehaviour
    {

        public event Action<int> OnAttackEnhancementChanged;
        public event Action<int> OnFallEnhancementChanged;
        public event Action<int> OnHealingEnhancementChanged;

        private MoltSystem m_moltSystem = null;
        private PlayerMovement m_playerMovement = null;
        private Game.Player.Stats.PlayerStats m_playerStats = null;

        private int m_attackEnhancementLevel = 0;
        private int m_fallEnhancementLevel = 0;
        private int m_healingEnhancementLevel = 0;

        private const int ATTACK_ENHANCEMENT_COST = 10;
        private const int ATTACK_ENHANCEMENT_MAX = 10;
        private const int FALL_ENHANCEMENT_COST = 15;
        private const int FALL_ENHANCEMENT_MAX = 3;
        private const int HEALING_ENHANCEMENT_COST = 20;
        private const int HEALING_ENHANCEMENT_MAX = 5;

        private void Awake()
        {
            m_moltSystem = GetComponent<MoltSystem>();
            m_playerMovement = GetComponent<PlayerMovement>();
            m_playerStats = GetComponent<Game.Player.Stats.PlayerStats>();

            if (m_moltSystem == null)
            {
                Debug.LogError("[EnhancementSystem] MoltSystem is missing!");
            }
            if (m_playerMovement == null)
            {
                Debug.LogError("[EnhancementSystem] PlayerMovement is missing!");
            }
            if (m_playerStats == null)
            {
                Debug.LogError("[EnhancementSystem] PlayerStats is missing!");
            }
        }

        private void Start()
        {
            ApplyAllEnhancements();
        }

        public bool TryEnhanceAttack()
        {
            if (m_attackEnhancementLevel >= ATTACK_ENHANCEMENT_MAX)
            {
                Debug.LogWarning("[EnhancementSystem] Attack enhancement is already maxed!");
                return false;
            }

            if (!m_moltSystem.ConsumeMolt(ATTACK_ENHANCEMENT_COST))
            {
                return false;
            }

            m_attackEnhancementLevel++;
            m_playerStats.UpgradeAttackPower(m_attackEnhancementLevel);
            OnAttackEnhancementChanged?.Invoke(m_attackEnhancementLevel);
            return true;
        }

        public bool TryEnhanceFall()
        {
            if (m_fallEnhancementLevel >= FALL_ENHANCEMENT_MAX)
            {
                Debug.LogWarning("[EnhancementSystem] Fall enhancement is already maxed!");
                return false;
            }

            if (!m_moltSystem.ConsumeMolt(FALL_ENHANCEMENT_COST))
            {
                return false;
            }

            m_fallEnhancementLevel++;
            float fallSpeedMultiplier = 1f - (m_fallEnhancementLevel * 0.1f);
            m_playerMovement.ApplyFallSpeedMultiplier(fallSpeedMultiplier);
            OnFallEnhancementChanged?.Invoke(m_fallEnhancementLevel);
            return true;
        }

        public bool TryEnhanceHealing()
        {
            if (m_healingEnhancementLevel >= HEALING_ENHANCEMENT_MAX)
            {
                Debug.LogWarning("[EnhancementSystem] Healing enhancement is already maxed!");
                return false;
            }

            if (!m_moltSystem.ConsumeMolt(HEALING_ENHANCEMENT_COST))
            {
                return false;
            }

            m_healingEnhancementLevel++;
            OnHealingEnhancementChanged?.Invoke(m_healingEnhancementLevel);
            return true;
        }

        private void ApplyAllEnhancements()
        {
            if (m_attackEnhancementLevel > 0)
            {
                m_playerStats.UpgradeAttackPower(m_attackEnhancementLevel);
            }

            if (m_fallEnhancementLevel > 0)
            {
                float fallSpeedMultiplier = 1f - (m_fallEnhancementLevel * 0.1f);
                m_playerMovement.ApplyFallSpeedMultiplier(fallSpeedMultiplier);
            }
        }

        public float GetHealingSpeedMultiplier()
        {
            return 1f - (m_healingEnhancementLevel * 0.1f);
        }

        [ContextMenu("Test Enhance Attack")]
        private void TestEnhanceAttack()
        {
            TryEnhanceAttack();
        }

        [ContextMenu("Test Enhance Fall")]
        private void TestEnhanceFall()
        {
            TryEnhanceFall();
        }

        [ContextMenu("Test Enhance Healing")]
        private void TestEnhanceHealing()
        {
            TryEnhanceHealing();
        }

        public int AttackEnhancementLevel => m_attackEnhancementLevel;
        public int FallEnhancementLevel => m_fallEnhancementLevel;
        public int HealingEnhancementLevel => m_healingEnhancementLevel;
    }
}