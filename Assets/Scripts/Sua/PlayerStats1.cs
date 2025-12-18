using UnityEngine;
using System;

namespace Game.Player.Stats
{
    public class PlayerStats : MonoBehaviour
    {

        public event Action OnStatsInitialized;

        private int m_maxHealth = 6;
        private int m_baseAttackPower = 1;
        private int m_attackEnhancementLevel = 0;
        private int m_maxThread = 200;

        public int MaxHealth => m_maxHealth;
        public int AttackPower => m_baseAttackPower + m_attackEnhancementLevel;
        public int MaxThread => m_maxThread;

        private void Awake()
        {
            InitializeStats();
        }

        private void InitializeStats()
        {
            m_maxHealth = 6;
            m_baseAttackPower = 1;
            m_attackEnhancementLevel = 0;
            m_maxThread = 200;

            OnStatsInitialized?.Invoke();
        }

        public void UpgradeHealth()
        {
            m_maxHealth = 11;
        }

        public void UpgradeAttackPower(int pLevel)
        {
            m_attackEnhancementLevel = pLevel;
        }

        public void ResetStats()
        {
            InitializeStats();
        }
    }
}