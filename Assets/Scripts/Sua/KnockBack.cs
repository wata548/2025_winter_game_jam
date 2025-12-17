using UnityEngine;
using System;
using Extension.Test;

namespace Game.Player.Combat
{
    public class KnockbackSystem : MonoBehaviour
    {

        public event Action<Vector3, float> OnKnockbackApplied;

        private Game.Player.Movement.PlayerMovement m_playerMovement = null;
        private Rigidbody m_rigid = null;

        [SerializeField] private float m_knockbackForce = 10f;
        [SerializeField] private float m_knockbackDuration = 0.2f;
        [SerializeField] private bool m_showGizmo = true;
        [SerializeField] private string m_enemyTag = "Enemy";

        private bool m_isKnockedBack = false;
        private float m_knockbackTimer = 0f;
        private Vector3 m_knockbackDirection = Vector3.zero;
        private float m_gizmoDisplayTimer = 0f;
        private const float GIZMO_DISPLAY_DURATION = 0.5f;

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
            UpdateGizmoDisplay();
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
            m_gizmoDisplayTimer = GIZMO_DISPLAY_DURATION;

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

        private void UpdateGizmoDisplay()
        {
            if (m_gizmoDisplayTimer > 0f)
            {
                m_gizmoDisplayTimer -= Time.deltaTime;
            }
        }

        private void OnTriggerEnter(Collider pCollider)
        {
            if (pCollider.CompareTag(m_enemyTag))
            {
                ApplyKnockback(pCollider.transform.position);
            }
        }

        private void OnTriggerStay(Collider pCollider)
        {
            if (pCollider.CompareTag(m_enemyTag) && !m_isKnockedBack)
            {
                ApplyKnockback(pCollider.transform.position);
            }
        }

        [TestMethod("Test Knockback Right")]
        private void TestKnockbackRight()
        {
            ApplyKnockbackDirect(Vector3.right);
        }

        [TestMethod("Test Knockback Left")]
        private void TestKnockbackLeft()
        {
            ApplyKnockbackDirect(Vector3.left);
        }

        public bool IsKnockedBack => m_isKnockedBack;

        private void OnDrawGizmos()
        {
            if (!m_showGizmo || m_gizmoDisplayTimer <= 0f) return;

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, transform.position + m_knockbackDirection * m_knockbackForce);

            Vector3 arrowEnd = transform.position + m_knockbackDirection * m_knockbackForce;
            Vector3 arrowLeft = Quaternion.Euler(0, 30, 0) * (m_knockbackDirection * 2f);
            Vector3 arrowRight = Quaternion.Euler(0, -30, 0) * (m_knockbackDirection * 2f);

            Gizmos.DrawLine(arrowEnd, arrowEnd - arrowLeft);
            Gizmos.DrawLine(arrowEnd, arrowEnd - arrowRight);
        }
    }
}