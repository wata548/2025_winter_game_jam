using Entity;
using Extension.Test;
using Game.VFX;
using System;
using UnityEngine;

namespace Game.Player.Stats
{
    public class HealthSystem : MonoBehaviour, IEntity
    {
        //==================================================||IEntity Implementation
        public int MaxHp => m_playerStats.MaxHealth;
        public int Hp => m_currentHealth;
        public bool IsDead => m_currentHealth <= 0;
        public int PoisonStack => m_currentPoisonStack;
        public int CurrentHealth => m_currentHealth;
        public bool IsInvulnerable => m_isInvulnerable;

        //==================================================||Fields
        public event Action<int> OnHealthChanged;
        public event Action OnPlayerDead;
        public event Action<int> OnPoisonStackChanged;

        private PlayerStats m_playerStats = null;
        private PlayerBuffSystem m_playerBuffSystem = null;
        private PoisonSystem m_poisonSystem = null;
        private int m_currentHealth = 0;
        private int m_currentPoisonStack = 0;
        private bool m_isInvulnerable = false;
        private float m_invulnerableTimer = 0f;

        [SerializeField] private float m_invulnerableDuration = 0.5f;

        private void Awake()
        {
            m_playerStats = GetComponent<PlayerStats>();
            m_playerBuffSystem = GetComponent<PlayerBuffSystem>();
            m_poisonSystem = GetComponent<PoisonSystem>();

            if (m_playerStats == null)
            {
                Debug.LogError("[HealthSystem] PlayerStats is missing!");
            }
            if (m_playerBuffSystem == null)
            {
                Debug.LogError("[HealthSystem] PlayerBuffSystem is missing!");
            }
            if (m_poisonSystem == null)
            {
                Debug.LogError("[HealthSystem] PoisonSystem is missing!");
            }
        }

        private void Start()
        {
            m_currentHealth = m_playerStats.MaxHealth;
            OnHealthChanged?.Invoke(m_currentHealth);
        }

        private void Update()
        {
            UpdateInvulnerable();
        }

        public void GetDamage(int pDamage)
        {
            if (m_isInvulnerable) return;

            // Def
            if (m_playerBuffSystem.CanBlockDamage())
            {
                m_playerBuffSystem.SwitchShell3Buff();
                m_isInvulnerable = true;
                m_invulnerableTimer = m_invulnerableDuration;
                Debug.Log("[HealthSystem] Damage blocked by Defense!");
                return;
            }

            // Poison
            int finalDamage = pDamage;
            if (m_poisonSystem.IsPoisoned)
            {
                finalDamage = Mathf.FloorToInt(pDamage * m_poisonSystem.GetDamageMultiplier());
                Debug.Log($"[HealthSystem] Poison damage multiplier applied! {pDamage} ���� {finalDamage}");
            }

            m_currentHealth -= finalDamage;
            m_isInvulnerable = true;
            m_invulnerableTimer = m_invulnerableDuration;

            VFXEffectManager.EffectType effectType = m_poisonSystem.IsPoisoned
                ? Game.VFX.VFXEffectManager.EffectType.PoisonHit
                : Game.VFX.VFXEffectManager.EffectType.Hit;
            Game.VFX.VFXEffectManager.Instance.PlayEffect(effectType, transform.position);

            OnHealthChanged?.Invoke(m_currentHealth);

            // Shell3 check!!
            m_playerBuffSystem.OnPlayerHit();

            if (m_currentHealth <= 0)
            {
                m_currentHealth = 0;
                OnPlayerDead?.Invoke();
                FadeController.Instance.Load("Death");
                Debug.Log("[HealthSystem] Player Dead!");
            }
        }

        public void GetHeal(int pAmount)
        {
            if (m_currentHealth >= m_playerStats.MaxHealth) return;

            m_currentHealth = Mathf.Min(m_currentHealth + pAmount, m_playerStats.MaxHealth);
            OnHealthChanged?.Invoke(m_currentHealth);
        }

        public void AddPoisonStack(int pAmount)
        {
            m_currentPoisonStack += pAmount;
            OnPoisonStackChanged?.Invoke(m_currentPoisonStack);
        }

        public void RemovePoisonStack(int pAmount)
        {
            m_currentPoisonStack = Mathf.Max(0, m_currentPoisonStack - pAmount);
            OnPoisonStackChanged?.Invoke(m_currentPoisonStack);
        }

        private void UpdateInvulnerable()
        {
            if (m_isInvulnerable)
            {
                m_invulnerableTimer -= Time.deltaTime;
                if (m_invulnerableTimer <= 0f)
                {
                    m_isInvulnerable = false;
                }
            }
        }

        [TestMethod("Test Damage 1")]
        private void TestGetDamage1()
        {
            GetDamage(1);
        }

        [TestMethod("Test Damage 3")]
        private void TestGetDamage5()
        {
            GetDamage(3);
        }

        [TestMethod("Test Hit VFX")]
        private void TestHitVFX()
        {
            Game.VFX.VFXEffectManager.Instance.PlayEffect(
                Game.VFX.VFXEffectManager.EffectType.Hit,
                transform.position
            );
            Debug.Log("[HealthSystem] Test Hit VFX played!");
        }

        [TestMethod("Test Poison Hit VFX")]
        private void TestPoisonHitVFX()
        {
            Game.VFX.VFXEffectManager.Instance.PlayEffect(
                Game.VFX.VFXEffectManager.EffectType.PoisonHit,
                transform.position
            );
            Debug.Log("[HealthSystem] Test Poison Hit VFX played!");
        }
    }
}