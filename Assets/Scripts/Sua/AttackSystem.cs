using UnityEngine;
using UInput = UnityEngine.Input;
using System.Collections.Generic;
using Entity.Enemy;
using Extension.Test;
using Game.VFX;

namespace Game.Player.Combat
{
    public class AttackSystem : MonoBehaviour
    {

        private Game.Player.Stats.PlayerStats m_playerStats = null;
        private Game.Player.Movement.PlayerMovement m_playerMovement = null;

        private Dictionary<AttackType, AttackConfig> m_attackConfigs = new Dictionary<AttackType, AttackConfig>();
        private Dictionary<AttackType, float> m_attackCooldownTimers = new Dictionary<AttackType, float>();
        private Dictionary<AttackType, bool> m_canAttack = new Dictionary<AttackType, bool>();

        [SerializeField] private float m_normalAttackCooldown = 0.5f;
        [SerializeField] private float m_normalAttackRange = 2f;
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
            m_attackConfigs = new Dictionary<AttackType, AttackConfig>();
            m_canAttack = new Dictionary<AttackType, bool>();
            m_attackCooldownTimers = new Dictionary<AttackType, float>();

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
            if (m_playerMovement == null)
            {
                Debug.LogError("[AttackSystem] PlayerMovement is not initialized!");
                return false;
            }

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

            return true;
        }

        private void PerformNormalAttack(float pDirection)
        {
            m_normalAttackActive = true;
            m_normalAttackTimer = m_normalAttackDuration;
            m_normalAttackDirection = Mathf.Abs(pDirection) > 0.01f ? pDirection : m_playerMovement.GetCurrentMoveInput();
            m_gizmoDisplayTimer = GIZMO_DISPLAY_DURATION;

            Game.VFX.VFXAttack vfxAttack = GetComponent<Game.VFX.VFXAttack>();
            if (vfxAttack != null)
            {
                float rotationY = transform.eulerAngles.y;
                Vector3 attackDirection = (rotationY < 180f) ? Vector3.right : Vector3.left;
                Vector3 attackPos = transform.position + attackDirection * (m_attackConfigs[AttackType.Normal].m_range / 2f);
                vfxAttack.PlayNormalAttackEffect(attackPos);
            }

            Debug.Log("[AttackSystem] Normal Attack!");
        }

        private void UpdateNormalAttack()
        {
            if (!m_normalAttackActive) return;

            m_normalAttackTimer -= Time.deltaTime;

            AttackConfig config = m_attackConfigs[AttackType.Normal];
            float rotationY = transform.eulerAngles.y;
            Vector3 attackDirection = (rotationY < 180f) ? Vector3.right : Vector3.left;
            Vector3 attackPos = transform.position + attackDirection * (config.m_range / 2f);
            Vector3 attackSize = new Vector3(config.m_range, config.m_radius, 1f);

            m_lastNormalAttackPos = attackPos;
            m_lastNormalAttackSize = attackSize;

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

            if (m_normalAttackTimer <= 0f)
            {
                m_normalAttackActive = false;
                m_lastNormalAttackSize = Vector3.zero;
            }
        }

        private void PerformAerialAttack()
        {
            m_aerialAttackActive = true;
            m_aerialAttackTimer = m_aerialAttackDuration;
            m_lastAerialAttackRadius = m_attackConfigs[AttackType.Aerial].m_radius;
            m_gizmoDisplayTimer = GIZMO_DISPLAY_DURATION;
            m_aerialAttackHitColliders.Clear();

            Game.VFX.VFXAttack vfxAttack = GetComponent<Game.VFX.VFXAttack>();
            if (vfxAttack != null)
            {
                vfxAttack.PlayAerialAttackEffect(transform.position);
            }

            Debug.Log("[AttackSystem] Aerial Attack!");
        }

        private void UpdateAerialAttack()
        {
            if (!m_aerialAttackActive) return;

            m_aerialAttackTimer -= Time.deltaTime;

            m_lastAerialAttackPos = transform.position;

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

            Game.VFX.VFXAttack vfxAttack = GetComponent<Game.VFX.VFXAttack>();
            if (vfxAttack != null)
            {
                vfxAttack.PlayHitEffect(pTarget.transform.position);
            }

            pTarget.GetComponent<Enemy>().GetDamage(damage);
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

        [TestMethod("Test Aerial Attack")]
        private void TestAerialAttack()
        {
            if (m_playerMovement.IsGrounded)
            {
                Debug.LogWarning("[AttackSystem] Must be airborne to test aerial attack!");
                return;
            }
            TryAttack(0f);
        }

        [TestMethod("Test Normal Hit VFX")]
        private void TestNormalHitVFX()
        {
            VFXEffectManager.Instance.PlayEffect(
                VFXEffectManager.EffectType.Hit,
                transform.position + Vector3.up
            );
            Debug.Log("[AttackSystem] Test Normal Hit VFX played!");
        }

        [TestMethod("Test Poison Hit VFX")]
        private void TestPoisonHitVFX()
        {
            VFXEffectManager.Instance.PlayEffect(
                VFXEffectManager.EffectType.PoisonHit,
                transform.position + Vector3.up
            );
            Debug.Log("[AttackSystem] Test Poison Hit VFX played!");
        }

        [TestMethod("Test Normal Attack VFX")]
        private void TestNormalAttackVFX()
        {
            float rotationY = transform.eulerAngles.y;
            float yRotation = (rotationY < 180f) ? 180f : 0f;

            VFXEffectManager.Instance.PlayEffect(
                VFXEffectManager.EffectType.NormalAttack,
                transform.position + Vector3.right,
                Quaternion.Euler(0f, yRotation, 0f)
            );
            Debug.Log("[AttackSystem] Test Normal Attack VFX played!");
        }

        [TestMethod("Test Poison Attack VFX")]
        private void TestPoisonAttackVFX()
        {
            float rotationY = transform.eulerAngles.y;
            float yRotation = (rotationY < 180f) ? 180f : 0f;

            VFXEffectManager.Instance.PlayEffect(
                VFXEffectManager.EffectType.PoisonAttack,
                transform.position + Vector3.right,
                Quaternion.Euler(0f, yRotation, 0f)
            );
            Debug.Log("[AttackSystem] Test Poison Attack VFX played!");
        }

        [TestMethod("Test Aerial Attack VFX")]
        private void TestAerialAttackVFX()
        {
            GameObject effect = VFXEffectManager.Instance.PlayEffectFollow(
                VFXEffectManager.EffectType.NormalAttackAir,
                transform
            );
            if (effect != null)
            {
                effect.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
            }
            Debug.Log("[AttackSystem] Test Aerial Attack VFX played!");
        }

        [TestMethod("Test Poison Aerial Attack VFX")]
        private void TestPoisonAerialAttackVFX()
        {
            GameObject effect = VFXEffectManager.Instance.PlayEffectFollow(
                VFXEffectManager.EffectType.PoisonAttackAir,
                transform
            );
            if (effect != null)
            {
                effect.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
            }
            Debug.Log("[AttackSystem] Test Poison Aerial Attack VFX played!");
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

            if (m_normalAttackActive && m_lastNormalAttackSize.magnitude > 0)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireCube(m_lastNormalAttackPos, m_lastNormalAttackSize);
                Gizmos.color = new Color(1, 0, 0, 0.1f);
                Gizmos.DrawCube(m_lastNormalAttackPos, m_lastNormalAttackSize);
            }

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