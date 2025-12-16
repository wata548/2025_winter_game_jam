using UnityEngine;
using System;
using Entity;

namespace Game.Player.Stats
{
    public class HealthSystem : MonoBehaviour, IEntity
    {

        public event Action<int, int> OnHealthChanged;
        public event Action OnDeath;

        private Game.Player.Stats.PlayerStats m_playerStats = null;
        private int m_currentHealth = 0;
        private bool m_isInvincible = false;
        private float m_invincibilityTimer = 0f;

        [SerializeField] private float m_invincibilityDuration = 0.3f;

        private void Awake()
        {
            m_playerStats = GetComponent<Game.Player.Stats.PlayerStats>();
            if (m_playerStats == null)
            {
                Debug.LogError("[HealthSystem] PlayerStats is missing!");
                return;
            }
        }

        private void Start()
        {
            m_currentHealth = m_playerStats.MaxHealth;
            OnHealthChanged?.Invoke(m_currentHealth, m_playerStats.MaxHealth);
        }

        private void Update()
        {
            UpdateInvincibility();
        }

        public void TakeDamage(int pDamage)
        {
            if (m_isInvincible || m_currentHealth <= 0)
            {
                return;
            }

            m_currentHealth -= pDamage;
            m_currentHealth = Mathf.Max(0, m_currentHealth);

            OnHealthChanged?.Invoke(m_currentHealth, m_playerStats.MaxHealth);
            SetInvincible(true);

            if (m_currentHealth <= 0)
            {
                OnDeath?.Invoke();
            }
        }

        public void Heal(int pAmount)
        {
            m_currentHealth += pAmount;
            m_currentHealth = Mathf.Min(m_currentHealth, m_playerStats.MaxHealth);

            OnHealthChanged?.Invoke(m_currentHealth, m_playerStats.MaxHealth);
        }

        public void FullHeal()
        {
            m_currentHealth = m_playerStats.MaxHealth;
            OnHealthChanged?.Invoke(m_currentHealth, m_playerStats.MaxHealth);
        }

        public void SetInvincible(bool pInvincible)
        {
            m_isInvincible = pInvincible;
            m_invincibilityTimer = pInvincible ? m_invincibilityDuration : 0f;
        }

        private void UpdateInvincibility()
        {
            if (!m_isInvincible) return;

            m_invincibilityTimer -= Time.deltaTime;
            if (m_invincibilityTimer <= 0f)
            {
                m_isInvincible = false;
            }
        }

        //==================================================||IEntity Implementation
        public int MaxHp => m_playerStats.MaxHealth;
        public int Hp => m_currentHealth;
        public bool IsDead => !IsAlive;

        public void GetDamage(int pAmount)
        {
            TakeDamage(pAmount);
        }

        public void GetHeal(int pAmount)
        {
            Heal(pAmount);
        }

        //==================================================||Properties
        public int CurrentHealth => m_currentHealth;
        public bool IsInvincible => m_isInvincible;
        public bool IsAlive => m_currentHealth > 0;
    }
}