using UnityEngine;
using Extension.Test;

namespace Game.Player.Combat
{
    public class KnockbackSystem : MonoBehaviour
    {

        private Rigidbody m_rigid = null;

        [SerializeField] private float m_knockbackForce = 12f;
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

        private void ApplyKnockback(Vector3 pSourcePosition)
        {
            Vector3 direction = (transform.position - pSourcePosition).normalized;
            direction.y = 0;

            var velocity = m_rigid.linearVelocity;
            velocity.x = direction.x * m_knockbackForce;
            velocity.y += m_knockbackUpForce;
            m_rigid.linearVelocity = velocity;
        }

        private void OnTriggerEnter(Collider pCollider)
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