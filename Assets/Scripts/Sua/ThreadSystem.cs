using UnityEngine;
using System;

namespace Game.Player.Stats
{
    public class ThreadSystem : MonoBehaviour
    {

        public event Action<int> OnThreadChanged;

        private PlayerStats m_playerStats = null;
        private int m_currentThread = 0;

        private void Awake()
        {
            m_playerStats = GetComponent<PlayerStats>();
            if (m_playerStats == null)
            {
                Debug.LogError("[ThreadSystem] PlayerStats is missing!");
            }
        }

        private void Start()
        {
            m_currentThread = 0;
            OnThreadChanged?.Invoke(m_currentThread);
        }

        public void AddThread(int pAmount)
        {
            m_currentThread += pAmount;
            m_currentThread = Mathf.Min(m_currentThread, m_playerStats.MaxThread);
            OnThreadChanged?.Invoke(m_currentThread);
        }

        public bool ConsumeThread(int pAmount)
        {
            if (m_currentThread < pAmount)
            {
                Debug.LogWarning($"[ThreadSystem] Not enough thread! (need: {pAmount}, have: {m_currentThread})");
                return false;
            }

            m_currentThread -= pAmount;
            OnThreadChanged?.Invoke(m_currentThread);
            return true;
        }

        [ContextMenu("Test Add Thread")]
        private void TestAddThread()
        {
            AddThread(10);
        }

        [ContextMenu("Test Consume Thread")]
        private void TestConsumeThread()
        {
            ConsumeThread(10);
        }

        public int CurrentThread => m_currentThread;
    }
}