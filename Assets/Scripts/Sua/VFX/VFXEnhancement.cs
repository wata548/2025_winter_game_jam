using UnityEngine;
using Game.Player.Stats;
using Extension.Test;

namespace Game.VFX
{
    public class VFXEnhancement : MonoBehaviour
    {
        private EnhancementSystem m_enhancementSystem;

        private void Awake()
        {
            m_enhancementSystem = GetComponent<EnhancementSystem>();

            if (m_enhancementSystem != null)
            {
                m_enhancementSystem.OnAttackEnhancementChanged += OnAttackEnhanced;
            }
        }

        private void OnDestroy()
        {
            if (m_enhancementSystem != null)
            {
                m_enhancementSystem.OnAttackEnhancementChanged -= OnAttackEnhanced;
            }
        }

        private void OnAttackEnhanced(int pLevel)
        {
            VFXEffectManager.Instance.PlayEffectFollow(
                VFXEffectManager.EffectType.Enhancement,
                transform
            );
        }

        public void PlayDefenseEffect()
        {
            VFXEffectManager.Instance.PlayEffectFollow(
                VFXEffectManager.EffectType.Defense,
                transform
            );
        }

        public void PlayOffenseBuff()
        {
            VFXEffectManager.Instance.PlayEffectFollow(
                VFXEffectManager.EffectType.OffenseBuff,
                transform
            );
        }

        [TestMethod("Test Enhancement VFX")]
        private void TestEnhancementVFX()
        {
            VFXEffectManager.Instance.PlayEffectFollow(
                VFXEffectManager.EffectType.Enhancement,
                transform
            );
            Debug.Log("[VFXEnhancement] Test Enhancement VFX played!");
        }

        [TestMethod("Test Defense VFX")]
        private void TestDefenseVFX()
        {
            PlayDefenseEffect();
            Debug.Log("[VFXEnhancement] Test Defense VFX played!");
        }

        [TestMethod("Test OffenseBuff VFX")]
        private void TestOffenseBuffVFX()
        {
            PlayOffenseBuff();
            Debug.Log("[VFXEnhancement] Test OffenseBuff VFX played!");
        }
    }
}