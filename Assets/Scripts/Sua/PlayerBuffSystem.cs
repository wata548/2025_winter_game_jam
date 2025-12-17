using Game.Player.Combat;
using System;
using UnityEngine;

namespace Game.Player.Stats
{
    public enum BuffType
    {
        Shell2Buff, // Jump, Spd x2
        Defense,    // 절대방어
        Offense     // Atk x2
    }

    public class PlayerBuffSystem : MonoBehaviour
    {

        public event Action<BuffType> OnBuffApplied;

        private ShellSystem m_shellSystem = null;
        private Game.Player.Movement.PlayerMovement m_playerMovement = null;
        private PlayerStats m_playerStats = null;

        // Shell2 Buff
        private bool m_shell2BuffActive = false;
        private float m_shell2BuffTimer = 0f;
        [SerializeField] private float m_shell2BuffDuration = 5f;

        // Shell3 Buff (Defense/Offense)
        private BuffType m_shell3CurrentBuff = BuffType.Defense;

        private void Awake()
        {
            m_shellSystem = GetComponent<ShellSystem>();
            m_playerMovement = GetComponent<Game.Player.Movement.PlayerMovement>();
            m_playerStats = GetComponent<PlayerStats>();

            if (m_shellSystem == null) Debug.LogError("[PlayerBuffSystem] ShellSystem is missing!");
            if (m_playerMovement == null) Debug.LogError("[PlayerBuffSystem] PlayerMovement is missing!");
            if (m_playerStats == null) Debug.LogError("[PlayerBuffSystem] PlayerStats is missing!");
        }

        private void Update()
        {
            UpdateShell2Buff();
        }

        //==================================================||Shell2 Buff
        public bool TryApplyShell2Buff()
        {
            if (!m_shellSystem.IsShellEquipped(ShellType.Shell2))
            {
                return false;
            }

            m_shell2BuffActive = true;
            m_shell2BuffTimer = m_shell2BuffDuration;

            OnBuffApplied?.Invoke(BuffType.Shell2Buff);
            Debug.Log("[PlayerBuffSystem] Shell2 Buff activated!");
            return true;
        }

        private void UpdateShell2Buff()
        {
            if (!m_shell2BuffActive) return;

            m_shell2BuffTimer -= Time.deltaTime;

            if (m_shell2BuffTimer <= 0f)
            {
                m_shell2BuffActive = false;
                Debug.Log("[PlayerBuffSystem] Shell2 Buff expired!");
            }
        }

        //==================================================||Shell3 Buff (Defense/Offense)
        public void OnPlayerHit()
        {
            if (!m_shellSystem.IsShellEquipped(ShellType.Shell3))
            {
                return;
            }

            if (m_shell3CurrentBuff == BuffType.Defense)
            {
                Debug.Log("[PlayerBuffSystem] Defense blocked damage!");
            }
            else if (m_shell3CurrentBuff == BuffType.Offense)
            {
                SwitchShell3Buff();
            }
        }

        public void SwitchShell3Buff()
        {
            m_shell3CurrentBuff = m_shell3CurrentBuff == BuffType.Defense ? BuffType.Offense : BuffType.Defense;
            OnBuffApplied?.Invoke(m_shell3CurrentBuff);

            string buffName = m_shell3CurrentBuff == BuffType.Defense ? "방어" : "공격";
            Debug.Log($"[PlayerBuffSystem] Shell3 switched to {buffName}!");
        }

        public bool CanBlockDamage()
        {
            if (!m_shellSystem.IsShellEquipped(ShellType.Shell3))
            {
                return false;
            }
            return m_shell3CurrentBuff == BuffType.Defense;
        }

        public int GetOffenseBonus()
        {
            if (!m_shellSystem.IsShellEquipped(ShellType.Shell3))
            {
                return 0;
            }
            return m_shell3CurrentBuff == BuffType.Offense ? 2 : 0;
        }

        //==================================================||Properties
        public bool IsShell2BuffActive => m_shell2BuffActive;
        public float Shell2BuffRemainingTime => m_shell2BuffTimer;
        public BuffType Shell3CurrentBuff => m_shell3CurrentBuff;
    }
}