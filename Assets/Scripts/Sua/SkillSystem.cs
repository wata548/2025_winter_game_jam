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
        public int m_moltCost;
    }

    public class SkillSystem : MonoBehaviour
    {

        public event Action<ShellType> OnSkillExecuted;

        private ShellSystem m_shellSystem = null;
        private Game.Player.Stats.MoltSystem m_moltSystem = null;
        private AttackSystem m_attackSystem = null;

        private Dictionary<ShellType, SkillConfig> m_skillConfigs = new Dictionary<ShellType, SkillConfig>();
        private Dictionary<ShellType, float> m_skillCooldownTimers = new Dictionary<ShellType, float>();
        private Dictionary<ShellType, bool> m_canUseSkill = new Dictionary<ShellType, bool>();

        [SerializeField] private float m_shell1SkillCooldown = 5f;
        [SerializeField] private int m_shell1SkillMoltCost = 20;
        [SerializeField] private float m_shell1SkillRadius = 5f;
        [SerializeField] private int m_shell1PoisonStackAmount = 2;

        [SerializeField] private float m_shell2SkillCooldown = 3f;
        [SerializeField] private int m_shell2SkillMoltCost = 15;
        [SerializeField] private float m_shell2MoveSpeedMultiplier = 1.5f;
        [SerializeField] private float m_shell2DurationSeconds = 5f;

        [SerializeField] private float m_shell3SkillCooldown = 4f;
        [SerializeField] private int m_shell3SkillMoltCost = 25;

        [SerializeField] private string m_enemyTag = "Enemy";

        private float m_shell2BuffTimer = 0f;
        private bool m_shell2BuffActive = false;

        private void Awake()
        {
            m_shellSystem = GetComponent<ShellSystem>();
            m_moltSystem = GetComponent<Game.Player.Stats.MoltSystem>();
            m_attackSystem = GetComponent<AttackSystem>();

            if (m_shellSystem == null)
            {
                Debug.LogError("[SkillSystem] ShellSystem is missing!");
            }
            if (m_moltSystem == null)
            {
                Debug.LogError("[SkillSystem] MoltSystem is missing!");
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
        }

        private void InitializeSkillConfigs()
        {
            m_skillConfigs[ShellType.Shell1] = new SkillConfig
            {
                m_cooldown = m_shell1SkillCooldown,
                m_moltCost = m_shell1SkillMoltCost
            };

            m_skillConfigs[ShellType.Shell2] = new SkillConfig
            {
                m_cooldown = m_shell2SkillCooldown,
                m_moltCost = m_shell2SkillMoltCost
            };

            m_skillConfigs[ShellType.Shell3] = new SkillConfig
            {
                m_cooldown = m_shell3SkillCooldown,
                m_moltCost = m_shell3SkillMoltCost
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

            SkillConfig config = m_skillConfigs[pShellType];

            if (!m_moltSystem.ConsumeMolt(config.m_moltCost))
            {
                return false;
            }

            ExecuteSkill(pShellType);

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

        [ContextMenu("Test Execute Shell1 Skill")]
        private void TestExecuteShell1Skill()
        {
            TryExecuteSkill(ShellType.Shell1);
        }

        public bool CanUseSkill(ShellType pShellType) => m_canUseSkill[pShellType];
        public float GetSkillCooldownProgress(ShellType pShellType)
        {
            SkillConfig config = m_skillConfigs[pShellType];
            return Mathf.Clamp01(1f - (m_skillCooldownTimers[pShellType] / config.m_cooldown));
        }
        public bool IsShell2BuffActive => m_shell2BuffActive;
        public float Shell2BuffRemainingTime => m_shell2BuffTimer;
    }
}