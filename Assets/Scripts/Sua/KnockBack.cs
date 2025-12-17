using UnityEngine;
using System;
using Extension.Test;

namespace Game.Player.Combat
{
    public class KnockbackSystem : MonoBehaviour
    {

        private Rigidbody m_rigid = null;

        [SerializeField] private float m_knockbackForce = 15f;
        [SerializeField] private float m_knockbackUpForce = 5f;
        [SerializeField] private string m_enemyTag = "Enemy";

        private void Awake()
        {
            m_rigid = GetComponent<Rigidbody>();

            if (m_rigid == null)
            {
                Debug.LogError("[KnockbackSystem] Rigidbody is missing!");
            }
        }

        public void ApplyKnockback(Vector3 pSourcePosition)
        {
            Vector3 knockbackDir = (transform.position - pSourcePosition).normalized;
            knockbackDir.y = 0;

            var velocity = m_rigid.linearVelocity;
            velocity.x = knockbackDir.x * m_knockbackForce;
            velocity.y += m_knockbackUpForce;
            m_rigid.linearVelocity = velocity;

            Debug.Log("[KnockbackSystem] Knockback applied!");
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
            if (pCollider.CompareTag(m_enemyTag))
            {
                ApplyKnockback(pCollider.transform.position);
            }
        }

        [TestMethod("Test Knockback Right")]
        private void TestKnockbackRight()
        {
            var velocity = m_rigid.linearVelocity;
            velocity.x = m_knockbackForce;
            velocity.y += m_knockbackUpForce;
            m_rigid.linearVelocity = velocity;
        }

        [TestMethod("Test Knockback Left")]
        private void TestKnockbackLeft()
        {
            var velocity = m_rigid.linearVelocity;
            velocity.x = -m_knockbackForce;
            velocity.y += m_knockbackUpForce;
            m_rigid.linearVelocity = velocity;
        }
    }
}