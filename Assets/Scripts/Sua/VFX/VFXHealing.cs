using UnityEngine;
using Game.Player.Stats;

namespace Game.VFX
{
    public class VFXHealing : MonoBehaviour
    {
        private HealingSystem m_healingSystem;

        private void Awake()
        {
            m_healingSystem = GetComponent<HealingSystem>();
        }

        public void PlayHealingEffect()
        {
            if (m_healingSystem.IsHealing)
            {
                VFXEffectManager.Instance.PlayEffectFollow(
                    VFXEffectManager.EffectType.Healing,
                    transform
                );
            }
        }
    }
}
