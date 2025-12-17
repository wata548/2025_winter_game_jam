using UnityEngine;
using System.Collections.Generic;
using System;
using Entity;
using Extension.Test;

namespace Game.Player.Combat
{
    [System.Serializable]
    public class SkillConfig
    {
        public float m_cooldown;
    }

    public class SkillSystem : MonoBehaviour
    {

        public event Action<ShellType> OnSkillExecuted;

        private ShellSystem m_shellSystem = null;
        private AttackSystem m_attackSystem = null;

        private Dictionary<ShellType, SkillConfig> m_skillConfigs = new Dictionary<ShellType, SkillConfig>();
        private Dictionary<ShellType, float> m_skillCooldownTimers = new Dictionary<ShellType, float>();
        private Dictionary<ShellType, bool> m_canUseSkill = new Dictionary<ShellType, bool>();

        [SerializeField] private float m_shell1SkillCooldown = 5f;
        [SerializeField] private float m_shell1SkillRadius = 5f;
        [SerializeField] private int m_shell1PoisonStackAmount = 2;

        [SerializeField] private float m_shell2SkillCooldown = 3f;
        [SerializeField] private float m_shell2MoveSpeedMultiplier = 1.5f;
        [SerializeField] private float m_shell2DurationSeconds = 5f;

        [SerializeField] private float m_shell3SkillCooldown = 4f;

        [SerializeField] private string m_enemyTag = "Enemy";
        [SerializeField] private bool m_showGizmo = true;

        private float m_shell2BuffTimer = 0f;
        private bool m_shell2BuffActive = false;
        private float m_gizmoDisplayTimer = 0f;
        private const float GIZMO_DISPLAY_DURATION = 0.5f;

        private void Awake()
        {
            m_shellSystem = GetComponent<ShellSystem>();
            m_attackSystem = GetComponent<AttackSystem>();

            if (m_shellSystem == null)
            {
                Debug.LogError("[SkillSystem] ShellSystem is missing!");
            }
            if (m_attackSystem == null)
            {
                Debug.LogError("[SkillSystem] AttackSystem is missing!");
            }

            InitializeSkillConfigs();
        }

        private void Update()
        {
            UpdateSkillCooldowns();
            UpdateShell2Buff();
            UpdateGizmoDisplay();
        }

        private void InitializeSkillConfigs()
        {
            m_skillConfigs[ShellType.Shell1] = new SkillConfig
            {
                m_cooldown = m_shell1SkillCooldown
            };

            m_skillConfigs[ShellType.Shell2] = new SkillConfig
            {
                m_cooldown = m_shell2SkillCooldown
            };

            m_skillConfigs[ShellType.Shell3] = new SkillConfig
            {
                m_cooldown = m_shell3SkillCooldown
            };

            m_canUseSkill[ShellType.Shell1] = true;
            m_canUseSkill[ShellType.Shell2] = true;
            m_canUseSkill[ShellType.Shell3] = true;

            m_skillCooldownTimers[ShellType.Shell1] = 0f;
            m_skillCooldownTimers[ShellType.Shell2] = 0f;
            m_skillCooldownTimers[ShellType.Shell3] = 0f;
        }

        public bool TryExecuteSkill(ShellType pShellType)
        {
            if (!m_shellSystem.IsShellEquipped(pShellType))
            {
                Debug.LogWarning($"[SkillSystem] {pShellType} is not equipped!");
                return false;
            }

            if (!m_canUseSkill[pShellType])
            {
                Debug.LogWarning($"[SkillSystem] {pShellType} skill is on cooldown!");
                return false;
            }

            ExecuteSkill(pShellType);

            SkillConfig config = m_skillConfigs[pShellType];
            m_skillCooldownTimers[pShellType] = config.m_cooldown;
            m_canUseSkill[pShellType] = false;
            OnSkillExecuted?.Invoke(pShellType);

            return true;
        }

        private void ExecuteSkill(ShellType pShellType)
        {
            if (pShellType == ShellType.Shell1)
            {
                ExecuteShell1Skill();
            }
            else if (pShellType == ShellType.Shell2)
            {
                ExecuteShell2Skill();
            }
            else if (pShellType == ShellType.Shell3)
            {
                ExecuteShell3Skill();
            }
        }

        private void ExecuteShell1Skill()
        {
            m_gizmoDisplayTimer = GIZMO_DISPLAY_DURATION;

            Collider[] hits = Physics.OverlapSphere(
                transform.position,
                m_shell1SkillRadius
            );

            foreach (Collider hit in hits)
            {
                if (hit.CompareTag(m_enemyTag))
                {
                    IEntity entity = hit.GetComponent<IEntity>();
                    if (entity != null)
                    {
                        entity.AddPoisonStack(m_shell1PoisonStackAmount);
                    }
                }
            }

            Debug.Log($"[SkillSystem] Shell1 Skill executed! Poison applied to enemies in {m_shell1SkillRadius}m radius!");
        }

        private void ExecuteShell2Skill()
        {
            m_shell2BuffActive = true;
            m_shell2BuffTimer = m_shell2DurationSeconds;
            Debug.Log($"[SkillSystem] Shell2 Skill executed! Move speed increased for {m_shell2DurationSeconds} seconds!");
        }

        private void UpdateShell2Buff()
        {
            if (!m_shell2BuffActive) return;

            m_shell2BuffTimer -= Time.deltaTime;

            if (m_shell2BuffTimer <= 0f)
            {
                m_shell2BuffActive = false;
                Debug.Log("[SkillSystem] Shell2 buff expired!");
            }
        }

        private void ExecuteShell3Skill()
        {
            Game.Player.Stats.PlayerBuffSystem playerBuffSystem = GetComponent<Game.Player.Stats.PlayerBuffSystem>();
            if (playerBuffSystem != null)
            {
                playerBuffSystem.ForceSwitch();
                Debug.Log("[SkillSystem] Shell3 Skill executed! Buff state switched!");
            }
        }

        private void UpdateSkillCooldowns()
        {
            foreach (var shellType in m_canUseSkill.Keys)
            {
                if (!m_canUseSkill[shellType])
                {
                    m_skillCooldownTimers[shellType] -= Time.deltaTime;

                    if (m_skillCooldownTimers[shellType] <= 0f)
                    {
                        m_canUseSkill[shellType] = true;
                        m_skillCooldownTimers[shellType] = 0f;
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

        [TestMethod("Test Execute Shell1 Skill")]
        private void TestExecuteShellSkill(int i = 1)
        {
            switch (i)
            {
                case 1:
                    TryExecuteSkill(ShellType.Shell1);
                    break;
                case 2:
                    TryExecuteSkill(ShellType.Shell2);
                    break;
                case 3:
                    TryExecuteSkill(ShellType.Shell3);
                    break;
                default:
                    TryExecuteSkill(ShellType.Shell1);
                    return;
            }
        }


        public bool CanUseSkill(ShellType pShellType) => m_canUseSkill[pShellType];
        public float GetSkillCooldownProgress(ShellType pShellType)
        {
            SkillConfig config = m_skillConfigs[pShellType];
            return Mathf.Clamp01(1f - (m_skillCooldownTimers[pShellType] / config.m_cooldown));
        }
        public bool IsShell2BuffActive => m_shell2BuffActive;
        public float Shell2BuffRemainingTime => m_shell2BuffTimer;

        private void OnDrawGizmos()
        {
            if (!m_showGizmo || m_gizmoDisplayTimer <= 0f) return;

            // Shell1 Skill Radius
            Gizmos.color = new Color(1, 0, 1, 0.3f);
            DrawWireSphere(transform.position, m_shell1SkillRadius);
        }

        private void DrawWireSphere(Vector3 pCenter, float pRadius)
        {
            int segments = 16;
            float deltaTheta = Mathf.PI * 2f / segments;

            for (int i = 0; i < segments; i++)
            {
                float theta1 = i * deltaTheta;
                float theta2 = (i + 1) * deltaTheta;

                Vector3 point1 = pCenter + new Vector3(Mathf.Cos(theta1) * pRadius, 0, Mathf.Sin(theta1) * pRadius);
                Vector3 point2 = pCenter + new Vector3(Mathf.Cos(theta2) * pRadius, 0, Mathf.Sin(theta2) * pRadius);

                Gizmos.DrawLine(point1, point2);
            }
        }
    }
}