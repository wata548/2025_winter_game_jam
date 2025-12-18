using UnityEngine;
using Game.Player.Stats;
using Entity;
using Extension.Test;

namespace Game.VFX
{
    public class VFXPoison : MonoBehaviour
    {
        private PoisonSystem m_poisonSystem;

        private void Awake()
        {
            m_poisonSystem = GetComponent<PoisonSystem>();

            if (m_poisonSystem != null)
            {
                m_poisonSystem.OnPoisoned += OnPoisoned;
            }
        }

        private void OnDestroy()
        {
            if (m_poisonSystem != null)
            {
                m_poisonSystem.OnPoisoned -= OnPoisoned;
            }
        }

        private void OnPoisoned()
        {
            VFXEffectManager.Instance.PlayEffectFollow(
                VFXEffectManager.EffectType.Poison,
                transform
            );
        }

        public void PlayPoisonEffectForEntity(Transform pEntityTransform)
        {
            if (pEntityTransform != null)
            {
                VFXEffectManager.Instance.PlayEffectFollow(
                    VFXEffectManager.EffectType.Poison,
                    pEntityTransform
                );
            }
        }

        [TestMethod("Test Poison VFX")]
        private void TestPoisonVFX()
        {
            VFXEffectManager.Instance.PlayEffectFollow(
                VFXEffectManager.EffectType.Poison,
                transform
            );
            Debug.Log("[VFXPoison] Test Poison VFX played!");
        }
    }
}