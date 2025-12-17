using UnityEngine;
using Game.Player.Stats;
using Game.Player.Combat;
using Extension.Test;

namespace Game.Player
{
    public class PlayerLogger : MonoBehaviour
    {

        private PlayerStats m_playerStats = null;
        private HealthSystem m_healthSystem = null;
        private ThreadSystem m_threadSystem = null;
        private MoltSystem m_moltSystem = null;
        private EnhancementSystem m_enhancementSystem = null;
        private ShellSystem m_shellSystem = null;
        private PlayerBuffSystem m_playerBuffSystem = null;

        private int m_lastHealth = -1;
        private int m_lastAttackPower = -1;
        private int m_lastThread = -1;
        private int m_lastMolt = -1;
        private string m_lastEquippedShells = "";
        private BuffType m_lastShell3Buff = BuffType.Defense;

        [SerializeField] private bool m_enableLogging = true;

        private void Awake()
        {
            m_playerStats = GetComponent<PlayerStats>();
            m_healthSystem = GetComponent<HealthSystem>();
            m_threadSystem = GetComponent<ThreadSystem>();
            m_moltSystem = GetComponent<MoltSystem>();
            m_enhancementSystem = GetComponent<EnhancementSystem>();
            m_shellSystem = GetComponent<ShellSystem>();
            m_playerBuffSystem = GetComponent<PlayerBuffSystem>();
        }

        private void Update()
        {
            if (!m_enableLogging) return;

            CheckHealthChanges();
            CheckAttackPowerChanges();
            CheckThreadChanges();
            CheckMoltChanges();
            CheckShellChanges();
            CheckShell3BuffChanges();
        }

        private void CheckHealthChanges()
        {
            int currentHealth = m_healthSystem.Hp;
            if (m_lastHealth != currentHealth)
            {
                m_lastHealth = currentHealth;
                Debug.Log($"[PlayerLogger] HP: {currentHealth}/{m_playerStats.MaxHealth}");
            }
        }

        private void CheckAttackPowerChanges()
        {
            int currentAttack = m_playerStats.AttackPower;
            if (m_lastAttackPower != currentAttack)
            {
                m_lastAttackPower = currentAttack;
                Debug.Log($"[PlayerLogger] ATK: {currentAttack}");
            }
        }

        private void CheckThreadChanges()
        {
            int currentThread = m_threadSystem.CurrentThread;
            if (m_lastThread != currentThread)
            {
                m_lastThread = currentThread;
                Debug.Log($"[PlayerLogger] Thread: {currentThread}/{m_playerStats.MaxThread}");
            }
        }

        private void CheckMoltChanges()
        {
            int currentMolt = m_moltSystem.CurrentMolt;
            if (m_lastMolt != currentMolt)
            {
                m_lastMolt = currentMolt;
                Debug.Log($"[PlayerLogger] Molt: {currentMolt}");
            }
        }

        private void CheckShellChanges()
        {
            string currentShells = GetEquippedShellsString();
            if (m_lastEquippedShells != currentShells)
            {
                m_lastEquippedShells = currentShells;
                Debug.Log($"[PlayerLogger] Shell: {currentShells}");
            }
        }

        private void CheckShell3BuffChanges()
        {
            if (!m_shellSystem.IsShellEquipped(ShellType.Shell3)) return;

            BuffType currentBuff = m_playerBuffSystem.Shell3CurrentBuff;
            if (m_lastShell3Buff != currentBuff)
            {
                m_lastShell3Buff = currentBuff;
                string buffName = currentBuff == BuffType.Defense ? "Defense" : "Offense";
                Debug.Log($"[PlayerLogger] Shell3: {buffName}");
            }
        }

        private string GetEquippedShellsString()
        {
            string shells = "";
            if (m_shellSystem.IsShellEquipped(ShellType.Shell1)) shells += "S1 ";
            if (m_shellSystem.IsShellEquipped(ShellType.Shell2)) shells += "S2 ";
            if (m_shellSystem.IsShellEquipped(ShellType.Shell3)) shells += "S3 ";
            return shells.Length > 0 ? shells.Trim() : "None";
        }

        [TestMethod("Log Current State")]
        public void LogCurrentState()
        {
            Debug.Log($"====== [PlayerLogger] ======\n" +
                $"HP: {m_healthSystem.Hp}/{m_playerStats.MaxHealth}\n" +
                $"ATK: {m_playerStats.AttackPower}\n" +
                $"Thread: {m_threadSystem.CurrentThread}/{m_playerStats.MaxThread}\n" +
                $"Molt: {m_moltSystem.CurrentMolt}\n" +
                $"Enhance ATK: Lv.{m_enhancementSystem.AttackEnhancementLevel + 1}/10\n" +
                $"Enhance Fall: Lv.{m_enhancementSystem.FallEnhancementLevel + 1}/8\n" +
                $"Enhance Heal: Lv.{m_enhancementSystem.HealingEnhancementLevel + 1}/6\n" +
                $"Shell: {GetEquippedShellsString()}\n" +
                (m_shellSystem.IsShellEquipped(ShellType.Shell3) ? $"Shell3 Buff: {(m_playerBuffSystem.Shell3CurrentBuff == BuffType.Defense ? "Defense" : "Offense")}\n" : "") +
                $"=======================");
        }
    }
}