using UnityEngine;
using System.Collections.Generic;
using System;
using Entity;

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
        private Game.Player.Stats.PlayerStats m_playerStats = null;
        private Game.Player.Movement.PlayerMovement m_playerMovement = null;
        private Game.Player.Stats.PlayerBuffSystem m_playerBuffSystem = null;

        private Dictionary<ShellType, SkillConfig> m_skillConfigs = new Dictionary<ShellType, SkillConfig>();
        private Dictionary<ShellType, float> m_skillCooldownTimers = new Dictionary<ShellType, float>();
        private Dictionary<ShellType, bool> m_canUseSkill = new Dictionary<ShellType, bool>();

        [SerializeField] private float m_shell1SkillCooldown = 5f;
        [SerializeField] private float m_shell1SkillRadius = 5f;
        [SerializeField] private int m_shell1PoisonStackAmount = 2;

        [SerializeField] private float m_shell2SkillCooldown = 3f;

        [SerializeField] private float m_shell3SkillCooldown = 4f;

        [SerializeField] private string m_enemyTag = "Enemy";
        [SerializeField] private bool m_showGizmo = true;

        private float m_gizmoDisplayTimer = 0f;
        private const float GIZMO_DISPLAY_DURATION = 0.5f;

        private void Awake()
        {
            m_shellSystem = GetComponent<ShellSystem>();
            m_playerStats = GetComponent<Game.Player.Stats.PlayerStats>();
            m_playerMovement = GetComponent<Game.Player.Movement.PlayerMovement>();
            m_playerBuffSystem = GetComponent<Game.Player.Stats.PlayerBuffSystem>();

            if (m_shellSystem == null)
            {
                Debug.LogError("[SkillSystem] ShellSystem is missing!");
            }

            InitializeSkillConfigs();
        }

        private void Update()
        {
            UpdateSkillCooldowns();
            UpdateGizmoDisplay();
        }

        private void InitializeSkillConfigs()
        {
            m_skillConfigs[ShellType.Shell1] = new SkillConfig { m_cooldown = m_shell1SkillCooldown };
            m_skillConfigs[ShellType.Shell2] = new SkillConfig { m_cooldown = m_shell2SkillCooldown };
            m_skillConfigs[ShellType.Shell3] = new SkillConfig { m_cooldown = m_shell3SkillCooldown };

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
                return false;
            }

            if (!m_canUseSkill[pShellType])
            {
                return false;
            }

            ExecuteSkill(pShellType);

            m_skillCooldownTimers[pShellType] = m_skillConfigs[pShellType].m_cooldown;
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

            Collider[] hits = Physics.OverlapSphere(transform.position, m_shell1SkillRadius);

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

            Debug.Log("[SkillSystem] Shell1 Skill executed!");
        }

        private void ExecuteShell2Skill()
        {
            if (m_playerBuffSystem != null)
            {
                m_playerBuffSystem.TryApplyShell2Buff();
            }
        }

        private void ExecuteShell3Skill()
        {
            if (m_playerBuffSystem != null)
            {
                m_playerBuffSystem.SwitchShell3Buff();
                Debug.Log("[SkillSystem] Shell3 Skill executed!");
            }
        }

        private void UpdateSkillCooldowns()
        {
            List<ShellType> shellTypes = new List<ShellType>(m_canUseSkill.Keys);

            foreach (var shellType in shellTypes)
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

        public bool CanUseSkill(ShellType pShellType) => m_canUseSkill[pShellType];
        public float GetSkillCooldownProgress(ShellType pShellType)
        {
            SkillConfig config = m_skillConfigs[pShellType];
            return Mathf.Clamp01(1f - (m_skillCooldownTimers[pShellType] / config.m_cooldown));
        }

        private void OnDrawGizmos()
        {
            if (!m_showGizmo || m_gizmoDisplayTimer <= 0f) return;

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