using UnityEngine;
using System;

namespace Game.Player.Stats
{
    public enum BuffType
    {
        Defense,
        Offense
    }

    public class PlayerBuffSystem : MonoBehaviour
    {

        public event Action<BuffType> OnBuffApplied;
        public event Action<BuffType> OnBuffRemoved;

        private Game.Player.Stats.PlayerStats m_playerStats = null;

        private BuffType m_currentBuff = BuffType.Defense;
        private int m_hitCount = 0;
        private const int HITS_TO_SWITCH = 1;
        private const int OFFENSE_ATTACK_BONUS = 2;

        private void Awake()
        {
            m_playerStats = GetComponent<Game.Player.Stats.PlayerStats>();
            if (m_playerStats == null)
            {
                Debug.LogError("[PlayerBuffSystem] PlayerStats is missing!");
            }
        }

        private void Start()
        {
            m_currentBuff = BuffType.Defense;
            OnBuffApplied?.Invoke(m_currentBuff);
        }

        public void OnPlayerHit()
        {
            m_hitCount++;

            if (m_hitCount >= HITS_TO_SWITCH)
            {
                m_hitCount = 0;
                SwitchBuff();
            }
        }

        private void SwitchBuff()
        {
            BuffType previousBuff = m_currentBuff;
            m_currentBuff = m_currentBuff == BuffType.Defense ? BuffType.Offense : BuffType.Defense;

            OnBuffRemoved?.Invoke(previousBuff);
            OnBuffApplied?.Invoke(m_currentBuff);

            Debug.Log($"[PlayerBuffSystem] Switched to {m_currentBuff}!");
        }

        public void ForceSwitch()
        {
            m_hitCount = 0;
            SwitchBuff();
        }

        public bool CanBlockDamage()
        {
            return m_currentBuff == BuffType.Defense;
        }

        public int GetOffenseBonus()
        {
            return m_currentBuff == BuffType.Offense ? OFFENSE_ATTACK_BONUS : 0;
        }

        [ContextMenu("Test Hit")]
        private void TestHit()
        {
            OnPlayerHit();
        }

        [ContextMenu("Test Force Switch")]
        private void TestForceSwitch()
        {
            ForceSwitch();
        }

        public BuffType CurrentBuff => m_currentBuff;
        public int HitCount => m_hitCount;
    }
}