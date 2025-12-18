using UnityEngine;
using Game.Player.Stats;

namespace Game.UI
{
    public class EnhancementUIController : MonoBehaviour
    {
        [SerializeField] private GameObject m_playerObject = null;
        private EnhancementSystem m_enhancementSystem = null;

        private void Start()
        {
            if (m_playerObject == null)
                m_playerObject = GameObject.FindGameObjectWithTag("Player");

            m_enhancementSystem = m_playerObject.GetComponent<EnhancementSystem>();
        }

        public void OnAttackEnhanceClick()
        {
            m_enhancementSystem.TryEnhanceAttack();
        }

        public void OnFallEnhanceClick()
        {
            m_enhancementSystem.TryEnhanceFall();
        }

        public void OnHealingEnhanceClick()
        {
            m_enhancementSystem.TryEnhanceHealing();
        }
    }
}