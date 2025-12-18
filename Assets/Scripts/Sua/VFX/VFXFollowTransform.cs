using UnityEngine;

namespace Game.VFX
{
    public class VFXFollowTransform : MonoBehaviour
    {
        private Transform m_targetTransform;
        private bool m_isFollowing;

        public void Initialize(Transform pTargetTransform)
        {
            m_targetTransform = pTargetTransform;
            m_isFollowing = true;
        }

        private void Update()
        {
            if (!m_isFollowing || m_targetTransform == null)
            {
                m_isFollowing = false;
                return;
            }

            transform.position = m_targetTransform.position;
        }

        public void StopFollowing()
        {
            m_isFollowing = false;
        }
    }
}
