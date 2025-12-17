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
        private SkillSystem m_skillSystem = null;
        private PlayerBuffSystem m_playerBuffSystem = null;
        private Game.Player.Movement.PlayerMovement m_playerMovement = null;

        private int m_lastHealth = -1;
        private int m_lastAttackPower = -1;
        private int m_lastThread = -1;
        private int m_lastMolt = -1;
        private int m_lastAttackEnhancement = -1;
        private int m_lastFallEnhancement = -1;
        private int m_lastHealingEnhancement = -1;
        private string m_lastEquippedShells = "";
        private BuffType m_lastBuffType = BuffType.Defense;

        [SerializeField] private bool m_enableLogging = true;

        private void Awake()
        {
            m_playerStats = GetComponent<PlayerStats>();
            m_healthSystem = GetComponent<HealthSystem>();
            m_threadSystem = GetComponent<ThreadSystem>();
            m_moltSystem = GetComponent<MoltSystem>();
            m_enhancementSystem = GetComponent<EnhancementSystem>();
            m_shellSystem = GetComponent<ShellSystem>();
            m_skillSystem = GetComponent<SkillSystem>();
            m_playerBuffSystem = GetComponent<PlayerBuffSystem>();
            m_playerMovement = GetComponent<Game.Player.Movement.PlayerMovement>();
        }

        private void Update()
        {
            if (!m_enableLogging) return;

            CheckHealthChanges();
            CheckAttackPowerChanges();
            CheckThreadChanges();
            CheckMoltChanges();
            CheckEnhancementChanges();
            CheckShellChanges();
            CheckBuffChanges();
        }

        private void CheckHealthChanges()
        {
            int currentHealth = m_healthSystem.CurrentHealth;
            if (m_lastHealth != currentHealth)
            {
                m_lastHealth = currentHealth;
                Debug.Log($"[PlayerStateLogger] HP: {currentHealth}/{m_playerStats.MaxHealth}");
            }
        }

        private void CheckAttackPowerChanges()
        {
            int currentAttack = m_playerStats.AttackPower;
            if (m_lastAttackPower != currentAttack)
            {
                m_lastAttackPower = currentAttack;
                Debug.Log($"[PlayerStateLogger] ATK: {currentAttack}");
            }
        }

        private void CheckThreadChanges()
        {
            int currentThread = m_threadSystem.CurrentThread;
            if (m_lastThread != currentThread)
            {
                m_lastThread = currentThread;
                Debug.Log($"[PlayerStateLogger] Thread: {currentThread}/{m_playerStats.MaxThread}");
            }
        }

        private void CheckMoltChanges()
        {
            int currentMolt = m_moltSystem.CurrentMolt;
            if (m_lastMolt != currentMolt)
            {
                m_lastMolt = currentMolt;
                Debug.Log($"[PlayerStateLogger] Molt: {currentMolt}");
            }
        }

        private void CheckEnhancementChanges()
        {
            int attackEnhance = m_enhancementSystem.AttackEnhancementLevel;
            int fallEnhance = m_enhancementSystem.FallEnhancementLevel;
            int healingEnhance = m_enhancementSystem.HealingEnhancementLevel;

            if (m_lastAttackEnhancement != attackEnhance)
            {
                m_lastAttackEnhancement = attackEnhance;
                Debug.Log($"[PlayerStateLogger] Atk: Lv.{attackEnhance}/10");
            }

            if (m_lastFallEnhancement != fallEnhance)
            {
                m_lastFallEnhancement = fallEnhance;
                Debug.Log($"[PlayerStateLogger] FallSpd: Lv.{fallEnhance}/3");
            }

            if (m_lastHealingEnhancement != healingEnhance)
            {
                m_lastHealingEnhancement = healingEnhance;
                Debug.Log($"[PlayerStateLogger] HealSpd: Lv.{healingEnhance}/5");
            }
        }

        private void CheckShellChanges()
        {
            string currentShells = GetEquippedShellsString();
            if (m_lastEquippedShells != currentShells)
            {
                m_lastEquippedShells = currentShells;
                Debug.Log($"[PlayerStateLogger] Sheel: {currentShells}");
            }
        }

        private string GetEquippedShellsString()
        {
            string shells = "";
            if (m_shellSystem.IsShellEquipped(ShellType.Shell1)) shells += "Shell1 ";
            if (m_shellSystem.IsShellEquipped(ShellType.Shell2)) shells += "Shell2 ";
            if (m_shellSystem.IsShellEquipped(ShellType.Shell3)) shells += "Shell3 ";
            return shells.Length > 0 ? shells : "NOPE";
        }

        private void CheckBuffChanges()
        {
            BuffType currentBuff = m_playerBuffSystem.CurrentBuff;
            if (m_lastBuffType != currentBuff)
            {
                m_lastBuffType = currentBuff;
                string buffName = currentBuff == BuffType.Defense ? "Def" : "Atk";
                Debug.Log($"[PlayerStateLogger] currnet buff: {buffName}");
            }
        }

        [TestMethod("Log Current State")]
        public void LogCurrentState()
        {
            Debug.Log($"====== [PlayerStateLogger] Player Log!!! ======" +
                $"\nHealth: {m_healthSystem.CurrentHealth}/{m_playerStats.MaxHealth}" +
                $"\nAtk: {m_playerStats.AttackPower}" +
                $"\nThread: {m_threadSystem.CurrentThread}/{m_playerStats.MaxThread}" +
                $"\nMolt: {m_moltSystem.CurrentMolt}" +
                $"\nUpgrade.atk: Lv.{m_enhancementSystem.AttackEnhancementLevel}/10" +
                $"\nUpgrade.fall: Lv.{m_enhancementSystem.FallEnhancementLevel}/3" +
                $"\nUpgrade.heal: Lv.{m_enhancementSystem.HealingEnhancementLevel}/5" +
                $"\nShell: {GetEquippedShellsString()}" +
                $"\nBuff: {(m_playerBuffSystem.CurrentBuff == BuffType.Defense ? "Def" : "Atk")}" +
                $"\n================================================");
        }
    }
}