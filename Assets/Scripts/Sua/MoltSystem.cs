using UnityEngine;
using System;
using Extension.Test;

namespace Game.Player.Stats
{
    public class MoltSystem : MonoBehaviour
    {

        public event Action<int> OnMoltChanged;

        private int m_currentMolt = 0;

        private void Start()
        {
            OnMoltChanged?.Invoke(m_currentMolt);
        }

        public void AddMolt(int pAmount)
        {
            m_currentMolt += pAmount;
            OnMoltChanged?.Invoke(m_currentMolt);
        }

        public bool ConsumeMolt(int pAmount)
        {
            if (m_currentMolt < pAmount)
            {
                Debug.LogWarning($"[MoltSystem] Not enough molt! (need: {pAmount}, current: {m_currentMolt})");
                return false;
            }

            m_currentMolt -= pAmount;
            OnMoltChanged?.Invoke(m_currentMolt);
            return true;
        }

        [TestMethod("Test Add Molt")]
        private void TestAddMolt()
        {
            AddMolt(10);
        }

        [TestMethod("Test Consume Molt")]
        private void TestConsumeMolt()
        {
            ConsumeMolt(10);
        }

        public int CurrentMolt => m_currentMolt;
    }
}