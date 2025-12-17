using UnityEngine;
using UInput = UnityEngine.Input;
using System.Collections.Generic;
using System;
using Extension.Test;

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
        [SerializeField] private float m_normalAttackDuration = 0.1f;

        [SerializeField] private float m_aerialAttackRadius = 3f;
        [SerializeField] private int m_aerialBaseDamage = 1;
        [SerializeField] private float m_aerialAttackDuration = 0.3f;

        [SerializeField] private string m_enemyTag = "Enemy";
        [SerializeField] private bool m_showGizmo = true;

        private Vector3 m_lastNormalAttackPos = Vector3.zero;
        private Vector3 m_lastNormalAttackSize = Vector3.zero;
        private Vector3 m_lastAerialAttackPos = Vector3.zero;
        private float m_lastAerialAttackRadius = 0f;
        private float m_gizmoDisplayTimer = 0f;
        private const float GIZMO_DISPLAY_DURATION = 0.5f;

        private bool m_normalAttackActive = false;
        private float m_normalAttackTimer = 0f;
        private float m_normalAttackDirection = 0f;

        private bool m_aerialAttackActive = false;
        private float m_aerialAttackTimer = 0f;
        private HashSet<Collider> m_aerialAttackHitColliders = new HashSet<Collider>();

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
            UpdateNormalAttack();
            UpdateAerialAttack();
            UpdateGizmoDisplay();
        }

        public bool TryAttack(float pDirection)
        {
            AttackType attackType = m_playerMovement.IsGrounded ? AttackType.Normal : AttackType.Aerial;

            if (!m_canAttack[attackType])
            {
                return false;
            }

            if (attackType == AttackType.Normal)
            {
                PerformNormalAttack(pDirection);
            }
            else
            {
                PerformAerialAttack();
            }

            m_attackCooldownTimers[attackType] = m_attackConfigs[attackType].m_cooldown;
            m_canAttack[attackType] = false;
            OnAttackExecuted?.Invoke();

            return true;
        }

        //==================================================||Normal Attack
        private void PerformNormalAttack(float pDirection)
        {
            m_normalAttackActive = true;
            m_normalAttackTimer = m_normalAttackDuration;
            m_normalAttackDirection = Mathf.Abs(pDirection) > 0.01f ? pDirection : m_playerMovement.GetCurrentMoveInput();
            m_gizmoDisplayTimer = GIZMO_DISPLAY_DURATION;

            Debug.Log("[AttackSystem] Normal Attack!");
        }

        private void UpdateNormalAttack()
        {
            if (!m_normalAttackActive) return;

            m_normalAttackTimer -= Time.deltaTime;

            // 위치추적
            AttackConfig config = m_attackConfigs[AttackType.Normal];
            Vector3 attackDirection = m_normalAttackDirection > 0 ? Vector3.right : Vector3.left;
            Vector3 attackPos = transform.position + attackDirection * (config.m_range / 2f);
            Vector3 attackSize = new Vector3(config.m_range, config.m_radius, 1f);

            m_lastNormalAttackPos = attackPos;
            m_lastNormalAttackSize = attackSize;

            // Hit check
            Collider[] hits = Physics.OverlapBox(
                attackPos,
                attackSize / 2f,
                Quaternion.identity
            );

            foreach (Collider hit in hits)
            {
                if (hit.CompareTag(m_enemyTag))
                {
                    DealDamage(hit.gameObject, AttackType.Normal);
                }
            }

            // end
            if (m_normalAttackTimer <= 0f)
            {
                m_normalAttackActive = false;
                m_lastNormalAttackSize = Vector3.zero;
            }
        }

        //==================================================||Aerial Attack
        private void PerformAerialAttack()
        {
            m_aerialAttackActive = true;
            m_aerialAttackTimer = m_aerialAttackDuration;
            m_lastAerialAttackRadius = m_attackConfigs[AttackType.Aerial].m_radius;
            m_gizmoDisplayTimer = GIZMO_DISPLAY_DURATION;
            m_aerialAttackHitColliders.Clear();

            Debug.Log("[AttackSystem] Aerial Attack!");
        }

        private void UpdateAerialAttack()
        {
            if (!m_aerialAttackActive) return;

            m_aerialAttackTimer -= Time.deltaTime;

            // 위치추적
            m_lastAerialAttackPos = transform.position;

            // Hit check
            AttackConfig config = m_attackConfigs[AttackType.Aerial];
            Collider[] hits = Physics.OverlapSphere(
                m_lastAerialAttackPos,
                config.m_radius
            );

            foreach (Collider hit in hits)
            {
                if (hit.CompareTag(m_enemyTag) && !m_aerialAttackHitColliders.Contains(hit))
                {
                    m_aerialAttackHitColliders.Add(hit);
                    DealDamage(hit.gameObject, AttackType.Aerial);
                }
            }

            // end
            if (m_aerialAttackTimer <= 0f)
            {
                m_aerialAttackActive = false;
                m_aerialAttackHitColliders.Clear();
            }
        }

        private void DealDamage(GameObject pTarget, AttackType pAttackType)
        {
            AttackConfig config = m_attackConfigs[pAttackType];
            int damage = m_playerStats.AttackPower + config.m_baseDamage;
            Debug.Log($"[AttackSystem] {pTarget.name} took {damage} damage from {pAttackType}!");
        }

        private void UpdateAttackCooldowns()
        {
            List<AttackType> attackTypes = new List<AttackType>(m_canAttack.Keys);

            foreach (var attackType in attackTypes)
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

        private void UpdateGizmoDisplay()
        {
            if (m_gizmoDisplayTimer > 0f)
            {
                m_gizmoDisplayTimer -= Time.deltaTime;
            }
        }

        [TestMethod("Test Normal Attack")]
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

        private void OnDrawGizmos()
        {
            if (!m_showGizmo || m_gizmoDisplayTimer <= 0f) return;

            // Normal 말이 안 된다는 뜻
            if (m_normalAttackActive && m_lastNormalAttackSize.magnitude > 0)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireCube(m_lastNormalAttackPos, m_lastNormalAttackSize);
                Gizmos.color = new Color(1, 0, 0, 0.1f);
                Gizmos.DrawCube(m_lastNormalAttackPos, m_lastNormalAttackSize);
            }

            // 공중
            if (m_aerialAttackActive && m_lastAerialAttackRadius > 0)
            {
                Gizmos.color = Color.blue;
                DrawVerticalWireCircle(m_lastAerialAttackPos, m_lastAerialAttackRadius);
                Gizmos.color = new Color(0, 0, 1, 0.1f);
                DrawVerticalFilledCircle(m_lastAerialAttackPos, m_lastAerialAttackRadius);
            }
        }

        private void DrawVerticalWireCircle(Vector3 pCenter, float pRadius)
        {
            int segments = 16;
            float deltaTheta = Mathf.PI * 2f / segments;

            for (int i = 0; i < segments; i++)
            {
                float theta1 = i * deltaTheta;
                float theta2 = (i + 1) * deltaTheta;

                Vector3 point1 = pCenter + new Vector3(Mathf.Cos(theta1) * pRadius, Mathf.Sin(theta1) * pRadius, 0);
                Vector3 point2 = pCenter + new Vector3(Mathf.Cos(theta2) * pRadius, Mathf.Sin(theta2) * pRadius, 0);

                Gizmos.DrawLine(point1, point2);
            }
        }

        private void DrawVerticalFilledCircle(Vector3 pCenter, float pRadius)
        {
            int segments = 8;
            float deltaTheta = Mathf.PI * 2f / segments;

            for (int i = 0; i < segments; i++)
            {
                float theta1 = i * deltaTheta;
                float theta2 = (i + 1) * deltaTheta;

                Vector3 point1 = pCenter + new Vector3(Mathf.Cos(theta1) * pRadius, Mathf.Sin(theta1) * pRadius, 0);
                Vector3 point2 = pCenter + new Vector3(Mathf.Cos(theta2) * pRadius, Mathf.Sin(theta2) * pRadius, 0);

                Gizmos.DrawLine(pCenter, point1);
                Gizmos.DrawLine(point1, point2);
            }
        }
    }
}