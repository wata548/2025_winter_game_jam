using UnityEngine;
using UInput = UnityEngine.Input;
using System.Collections.Generic;
using System;

namespace Game.Player.Combat
{
    public class AttackSystem : MonoBehaviour
    {

        public event Action OnAttackExecuted;

        private Game.Player.Stats.PlayerStats m_playerStats = null;
        private Game.Player.Movement.PlayerMovement m_playerMovement = null;

        private Dictionary<AttackType, AttackConfig> m_attackConfigs = new Dictionary<AttackType, AttackConfig>();
        private Dictionary<AttackType, float> m_attackCooldownTimers = new Dictionary<AttackType, float>();
        private Dictionary<AttackType, bool> m_canAttack = new Dictionary<AttackType, bool>();

        [SerializeField] private float m_normalAttackCooldown = 0.5f;
        [SerializeField] private float m_normalAttackRange = 2f;
        [SerializeField] private float m_normalAttackHeight = 1f;
        [SerializeField] private int m_normalBaseDamage = 1;

        [SerializeField] private float m_aerialAttackRadius = 3f;
        [SerializeField] private int m_aerialBaseDamage = 1;

        [SerializeField] private LayerMask m_enemyLayer;

        private void Awake()
        {
            m_playerStats = GetComponent<Game.Player.Stats.PlayerStats>();
            m_playerMovement = GetComponent<Game.Player.Movement.PlayerMovement>();

            if (m_playerStats == null)
            {
                Debug.LogError("[AttackSystem] PlayerStats is missing!");
            }
            if (m_playerMovement == null)
            {
                Debug.LogError("[AttackSystem] PlayerMovement is missing!");
            }

            InitializeAttackConfigs();
        }

        private void InitializeAttackConfigs()
        {
            m_attackConfigs[AttackType.Normal] = new AttackConfig
            {
                m_cooldown = m_normalAttackCooldown,
                m_range = m_normalAttackRange,
                m_radius = 0,
                m_baseDamage = m_normalBaseDamage
            };

            m_attackConfigs[AttackType.Aerial] = new AttackConfig
            {
                m_cooldown = 0,
                m_range = 0,
                m_radius = m_aerialAttackRadius,
                m_baseDamage = m_aerialBaseDamage
            };

            m_canAttack[AttackType.Normal] = true;
            m_canAttack[AttackType.Aerial] = true;
            m_attackCooldownTimers[AttackType.Normal] = 0f;
            m_attackCooldownTimers[AttackType.Aerial] = 0f;
        }

        private void Update()
        {
            UpdateAttackCooldowns();
        }

        public bool TryAttack(float pDirection)
        {
            AttackType attackType = m_playerMovement.IsGrounded ? AttackType.Normal : AttackType.Aerial;

            if (!m_canAttack[attackType])
            {
                return false;
            }

            PerformAttack(attackType, pDirection);

            m_attackCooldownTimers[attackType] = m_attackConfigs[attackType].m_cooldown;
            m_canAttack[attackType] = false;
            OnAttackExecuted?.Invoke();

            return true;
        }

        private void PerformAttack(AttackType pAttackType, float pDirection)
        {
            if (pAttackType == AttackType.Normal)
            {
                PerformNormalAttack(pDirection);
            }
            else if (pAttackType == AttackType.Aerial)
            {
                PerformAerialAttack();
            }
        }

        private void PerformNormalAttack(float pDirection)
        {
            AttackConfig config = m_attackConfigs[AttackType.Normal];
            Vector3 attackDirection = pDirection > 0 ? Vector3.right : Vector3.left;
            Vector3 attackPos = transform.position + attackDirection * (config.m_range / 2f);
            Vector3 attackSize = new Vector3(config.m_range, config.m_radius, 1f);

            Collider[] hits = Physics.OverlapBox(
                attackPos,
                attackSize / 2f,
                Quaternion.identity,
                m_enemyLayer
            );

            foreach (Collider hit in hits)
            {
                DealDamage(hit.gameObject, AttackType.Normal);
            }

            Debug.Log("[AttackSystem] Normal Attack!");
        }

        private void PerformAerialAttack()
        {
            AttackConfig config = m_attackConfigs[AttackType.Aerial];
            Vector3 attackPos = transform.position;

            Collider[] hits = Physics.OverlapSphere(
                attackPos,
                config.m_radius,
                m_enemyLayer
            );

            foreach (Collider hit in hits)
            {
                DealDamage(hit.gameObject, AttackType.Aerial);
            }

            Debug.Log("[AttackSystem] Aerial Attack!");
        }

        private void DealDamage(GameObject pTarget, AttackType pAttackType)
        {
            AttackConfig config = m_attackConfigs[pAttackType];
            int damage = m_playerStats.AttackPower + config.m_baseDamage;
            Debug.Log($"[AttackSystem] {pTarget.name} took {damage} damage from {pAttackType}!");
        }

        private void UpdateAttackCooldowns()
        {
            foreach (var attackType in m_canAttack.Keys)
            {
                if (!m_canAttack[attackType])
                {
                    m_attackCooldownTimers[attackType] -= Time.deltaTime;

                    if (m_attackCooldownTimers[attackType] <= 0f)
                    {
                        m_canAttack[attackType] = true;
                        m_attackCooldownTimers[attackType] = 0f;
                    }
                }
            }
        }

        [ContextMenu("Test Normal Attack")]
        private void TestNormalAttack()
        {
            TryAttack(1f);
        }

        public bool CanAttack(AttackType pAttackType) => m_canAttack[pAttackType];
        public float GetAttackCooldownProgress(AttackType pAttackType)
        {
            AttackConfig config = m_attackConfigs[pAttackType];
            if (config.m_cooldown <= 0) return 1f;
            return Mathf.Clamp01(1f - (m_attackCooldownTimers[pAttackType] / config.m_cooldown));
        }
    }
}