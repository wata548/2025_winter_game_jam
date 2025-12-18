using UnityEngine;

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
        private Vector3 m_knockbackVelocity = Vector3.zero;

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

            m_knockbackVelocity = new Vector3(
                direction.x * m_knockbackForce,
                m_knockbackUpForce,
                0
            );

            m_rigid.AddForce(m_knockbackVelocity, ForceMode.Impulse);

            m_isKnockedBack = true;
            m_knockbackTimer = m_knockbackDuration;

            if (m_playerMovement != null)
            {
                m_playerMovement.BlockInput();
            }

            Debug.Log("[KnockbackSystem] Knockback applied!");
        }

        private void UpdateKnockback()
        {
            if (!m_isKnockedBack) return;

            m_knockbackTimer -= Time.deltaTime;

            if (m_knockbackTimer <= 0f)
            {
                m_isKnockedBack = false;
                var velocity = m_rigid.linearVelocity;
                velocity.y = 0f;
                m_rigid.linearVelocity = velocity;

                if (m_playerMovement != null)
                {
                    m_playerMovement.UnblockInput();
                }

                Debug.Log("[KnockbackSystem] Knockback ended!");
            }
        }

        private void OnCollisionEnter(Collision pCollision)
        {
            if (m_isKnockedBack) return;

            if (pCollision.gameObject.CompareTag(m_enemyTag))
            {
                Debug.Log($"[KnockbackSystem] Collision with {pCollision.gameObject.name}!");
                ApplyKnockback(pCollision.transform.position);
            }
        }

        private void OnCollisionStay(Collision pCollision)
        {
            if (!m_isKnockedBack) return;

            int groundLayer = LayerMask.NameToLayer("Ground");
            if (pCollision.gameObject.layer == groundLayer)
            {
                m_isKnockedBack = false;
                var velocity = m_rigid.linearVelocity;
                velocity.y = 0f;
                m_rigid.linearVelocity = velocity;

                if (m_playerMovement != null)
                {
                    m_playerMovement.UnblockInput();
                }

                Debug.Log("[KnockbackSystem] Hit ground during knockback!");
            }
        }
    }
}