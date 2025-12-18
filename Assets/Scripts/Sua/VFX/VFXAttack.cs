using Game.Player.Combat;
using Game.Player.Stats;
using UnityEngine;

namespace Game.VFX
{
    public class VFXAttack : MonoBehaviour
    {
        private AttackSystem m_attackSystem;
        private Game.Player.Movement.PlayerMovement m_playerMovement;
        private PlayerBuffSystem m_playerBuffSystem;

        [SerializeField] private float m_aerialAttackEffectDuration = 0.5f;

        private void Awake()
        {
            m_attackSystem = GetComponent<AttackSystem>();
            m_playerMovement = GetComponent<Game.Player.Movement.PlayerMovement>();
            m_playerBuffSystem = GetComponent<PlayerBuffSystem>();
        }

        public void PlayNormalAttackEffect(Vector3 pPosition)
        {
            VFXEffectManager.EffectType effectType = m_playerBuffSystem.Shell3CurrentBuff == BuffType.Offense
                ? VFXEffectManager.EffectType.PoisonAttack
                : VFXEffectManager.EffectType.NormalAttack;

            float rotationY = transform.eulerAngles.y;
            float yRotation = (rotationY < 180f) ? 180f : 0f;

            VFXEffectManager.Instance.PlayEffect(effectType, pPosition, Quaternion.Euler(0f, yRotation, 0f));
        }

        public void PlayAerialAttackEffect(Vector3 pPosition)
        {
            VFXEffectManager.EffectType effectType = m_playerBuffSystem.Shell3CurrentBuff == BuffType.Offense
                ? VFXEffectManager.EffectType.PoisonAttackAir
                : VFXEffectManager.EffectType.NormalAttackAir;

            GameObject effect = VFXEffectManager.Instance.PlayEffectFollow(effectType, transform);
            if (effect != null)
            {
                effect.transform.rotation = Quaternion.Euler(90f, 0f, 0f);

                // VFX 속도 설정
                UnityEngine.VFX.VisualEffect vfx = effect.GetComponent<UnityEngine.VFX.VisualEffect>();
                if (vfx != null)
                {
                    vfx.playRate = 2f;
                }

                // 이건 파티클
                ParticleSystem ps = effect.GetComponent<ParticleSystem>();
                if (ps != null)
                {
                    ps.playbackSpeed = 2f;
                }

                VFXEffectManager.Instance.ReturnEffect(effect, effectType, m_aerialAttackEffectDuration);
            }
        }

        public void PlayHitEffect(Vector3 pPosition)
        {
            VFXEffectManager.EffectType effectType = m_playerBuffSystem.Shell3CurrentBuff == BuffType.Offense
                ? VFXEffectManager.EffectType.PoisonHit
                : VFXEffectManager.EffectType.Hit;

            VFXEffectManager.Instance.PlayEffect(effectType, pPosition);
        }
    }
}