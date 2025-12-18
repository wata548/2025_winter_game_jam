using UnityEngine;
using Extension.Test;

namespace Game.Player.Combat
{
    public class KnockbackSystem : MonoBehaviour
    {

        private Rigidbody m_rigid = null;
        private Game.Player.Movement.PlayerMovement m_playerMovement = null;

        [SerializeField] private float m_knockbackForce = 30f;
        [SerializeField] private float m_knockbackUpForce = 10f;
        [SerializeField] private float m_knockbackDuration = 0.3f;
        [SerializeField] private string m_enemyTag = "Enemy";

        private bool m_isKnockedBack = false;
        private float m_knockbackTimer = 0f;

        private void Awake()
        {
            m_rigid = GetComponent<Rigidbody>();
            m_playerMovement = GetComponent<Game.Player.Movement.PlayerMovement>();

            if (m_rigid == null)
            {
                Debug.LogError("[KnockbackSystem] Rigidbody is missing!");
            }
            if (m_playerMovement == null)
            {
                Debug.LogError("[KnockbackSystem] PlayerMovement is missing!");
            }
        }

        private void Update()
        {
            UpdateKnockback();
        }

        private void ApplyKnockback(Vector3 pSourcePosition)
        {
            Vector3 direction = (transform.position - pSourcePosition).normalized;
            direction.y = 0;

            Vector3 knockbackForce = new Vector3(
                direction.x * m_knockbackForce,
                m_knockbackUpForce,
                0
            );

            m_rigid.AddForce(knockbackForce, ForceMode.Impulse);

            m_isKnockedBack = true;
            m_knockbackTimer = m_knockbackDuration;

            Debug.Log($"[KnockbackSystem] Knockback applied!");
        }

        private void UpdateKnockback()
        {
            if (!m_isKnockedBack) return;

            m_knockbackTimer -= Time.deltaTime;

            if (m_knockbackTimer <= 0f)
            {
                m_isKnockedBack = false;
                Debug.Log("[KnockbackSystem] Knockback ended!");

                if (m_playerMovement != null)
                {
                    m_playerMovement.UnblockInput();
                }
            }
            else
            {
                if (m_playerMovement != null)
                {
                    m_playerMovement.BlockInput();
                }
            }
        }

        private void OnCollisionEnter(Collision pCollision)
        {
            if (m_isKnockedBack) return;

            Debug.Log($"[KnockbackSystem] Collision with {pCollision.gameObject.name}!");

            if (pCollision.gameObject.CompareTag(m_enemyTag))
            {
                Debug.Log("[KnockbackSystem] Enemy detected! Applying knockback!");
                ApplyKnockback(pCollision.transform.position);
            }
        }

        [TestMethod("Test Knockback Right")]
        private void TestKnockbackRight()
        {
            m_rigid.AddForce(new Vector3(m_knockbackForce, m_knockbackUpForce, 0), ForceMode.Impulse);
            m_isKnockedBack = true;
            m_knockbackTimer = m_knockbackDuration;
        }

        [TestMethod("Test Knockback Left")]
        private void TestKnockbackLeft()
        {
            m_rigid.AddForce(new Vector3(-m_knockbackForce, m_knockbackUpForce, 0), ForceMode.Impulse);
            m_isKnockedBack = true;
            m_knockbackTimer = m_knockbackDuration;
        }

        public bool IsKnockedBack => m_isKnockedBack;

        //private void ApplyKnockback(Vector3 pSourcePosition)
        //{
        //    Vector3 direction = (transform.position - pSourcePosition).normalized;
        //    direction.y = 0;

        //    m_knockbackVelocity = new Vector3(
        //        direction.x * m_knockbackForce,
        //        m_knockbackUpForce,
        //        0
        //    );

        //    m_isKnockedBack = true;
        //    m_knockbackTimer = m_knockbackDuration;

        //    Debug.Log($"[KnockbackSystem] Knockback applied! Duration: {m_knockbackDuration}s");
        //}

        //private void UpdateKnockback()
        //{
        //    if (!m_isKnockedBack) return;

        //    m_knockbackTimer -= Time.deltaTime;
        //    float elapsedTime = m_knockbackDuration - m_knockbackTimer;

        //    if (m_knockbackTimer > 0f)
        //    {
        //        var velocity = m_rigid.linearVelocity;

        //        velocity.x = m_knockbackVelocity.x;

        //        if (elapsedTime < 0.1f)
        //        {
        //            velocity.y = m_knockbackVelocity.y * (1f - elapsedTime / 0.1f);
        //        }
        //        else
        //        {
        //            float fallTime = elapsedTime - 0.1f;
        //            velocity.y = -9.8f * fallTime;
        //        }

        //        m_rigid.linearVelocity = velocity;

        //        if (m_playerMovement != null)
        //        {
        //            m_playerMovement.BlockInput();
        //        }
        //    }
        //    else
        //    {
        //        m_isKnockedBack = false;
        //        Debug.Log("[KnockbackSystem] Knockback ended!");

        //        if (m_playerMovement != null)
        //        {
        //            m_playerMovement.UnblockInput();
        //        }
        //    }
        //}
    }
}
