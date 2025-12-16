using UnityEngine;
using System;

namespace Game.Player.Combat
{
    public class KnockbackSystem : MonoBehaviour
    {

        public event Action<Vector3, float> OnKnockbackApplied;

        private Game.Player.Movement.PlayerMovement m_playerMovement = null;
        private Rigidbody m_rigid = null;

        [SerializeField] private float m_knockbackForce = 10f;
        [SerializeField] private float m_knockbackDuration = 0.2f;

        private bool m_isKnockedBack = false;
        private float m_knockbackTimer = 0f;
        private Vector3 m_knockbackDirection = Vector3.zero;

        private void Awake()
        {
            m_playerMovement = GetComponent<Game.Player.Movement.PlayerMovement>();
            m_rigid = GetComponent<Rigidbody>();

            if (m_playerMovement == null)
            {
                Debug.LogError("[KnockbackSystem] PlayerMovement is missing!");
            }
            if (m_rigid == null)
            {
                Debug.LogError("[KnockbackSystem] Rigidbody is missing!");
            }
        }

        private void Update()
        {
            UpdateKnockback();
        }

        public void ApplyKnockback(Vector3 pSourcePosition)
        {
            Vector3 knockbackDir = (transform.position - pSourcePosition).normalized;
            knockbackDir.y = 0;

            ApplyKnockbackDirect(knockbackDir);
        }

        public void ApplyKnockbackDirect(Vector3 pDirection)
        {
            m_isKnockedBack = true;
            m_knockbackTimer = m_knockbackDuration;
            m_knockbackDirection = pDirection.normalized;

            OnKnockbackApplied?.Invoke(m_knockbackDirection, m_knockbackForce);
            Debug.Log($"[KnockbackSystem] Player knocked back in direction {m_knockbackDirection}!");
        }

        private void UpdateKnockback()
        {
            if (!m_isKnockedBack) return;

            m_knockbackTimer -= Time.deltaTime;

            if (m_knockbackTimer > 0f)
            {
                var velocity = m_rigid.linearVelocity;
                velocity.x = m_knockbackDirection.x * m_knockbackForce;
                m_rigid.linearVelocity = velocity;
            }
            else
            {
                m_isKnockedBack = false;
                m_knockbackDirection = Vector3.zero;
            }
        }

        [ContextMenu("Test Knockback Right")]
        private void TestKnockbackRight()
        {
            ApplyKnockbackDirect(Vector3.right);
        }

        [ContextMenu("Test Knockback Left")]
        private void TestKnockbackLeft()
        {
            ApplyKnockbackDirect(Vector3.left);
        }

        public bool IsKnockedBack => m_isKnockedBack;
    }
}