using System.Linq;
using UnityEngine;

namespace Game.Player.Movement
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerMovement : MonoBehaviour
    {
        //==================================================||Constants 
        private const float JUMP_PEEK_HEIGHT = 4f;
        private const float JUMP_PEEK_INTERVAL = 0.3f;
        private const float GROUND_CHECK_OFFSET = 0.95f;
        private const float GROUND_CHECKER_HEIGHT = 0.01f;

        //==================================================||Fields - Movement
        private Rigidbody m_rigid = null;
        private bool m_isGround = false;
        private bool m_slowFalling = false;
        private float m_currentMoveInput = 0f;
        private float m_lastMoveDirection = 1f;

        //==================================================||Fields - Jump & Gravity
        private float m_jumpScale;
        private float m_gravityScale;

        [SerializeField] private float m_maxMoveSpeed = 10f;
        [SerializeField] private float m_moveAcceleration = 25f;
        [SerializeField] private float m_moveDeceleration = 20f;
        [SerializeField] private float m_airAcceleration = 15f;
        [SerializeField] private float m_slowFallingPower = 0.5f;

        //==================================================||Fields - Dash
        [SerializeField] private float m_dashSpeed = 25f;
        [SerializeField] private float m_dashDuration = 0.3f;
        [SerializeField] private float m_dashCooldown = 0.5f;

        private bool m_isDashing = false;
        private float m_dashTimer = 0f;
        private float m_dashCooldownTimer = 0f;
        private float m_dashDirection = 1f;

        //==================================================||Jump & Gravity 
        public void SetActiveSlowFalling(bool pOn, float pPower = 0.3f)
        {
            m_slowFallingPower = pPower;
            m_slowFalling = pOn;
        }

        public void Jump()
        {
            var velocity = m_rigid.linearVelocity;
            velocity.y += m_jumpScale;
            m_rigid.linearVelocity = velocity;
        }

        private bool IsGround(float pGravity, out float pLength)
        {
            pLength = 0;

            var halfScale = transform.localScale / 2f;
            var center = transform.position;
            center.y += halfScale.y;
            halfScale *= GROUND_CHECK_OFFSET;
            halfScale.y = GROUND_CHECKER_HEIGHT / 2;

            var contacts = Physics.BoxCastAll(
                center,
                halfScale,
                Vector3.down,
                Quaternion.identity,
                -pGravity * Time.timeScale * Time.deltaTime + transform.localScale.y,
                LayerMask.GetMask("Ground")
            );
            if (contacts.Length == 0)
                return false;

            pLength = contacts.Min(hit => hit.distance) - transform.localScale.y;
            return true;
        }

        private void GravityProcess()
        {
            var velocity = m_rigid.linearVelocity;

            if (velocity.y <= 0 && m_slowFalling)
                velocity.y += m_gravityScale * Time.deltaTime * m_slowFallingPower;
            else
                velocity.y += m_gravityScale * Time.deltaTime;
            m_isGround = IsGround(velocity.y, out var length);

            if (m_isGround)
            {
                velocity.y = 0;
                var pos = transform.position;
                pos.y -= length;
                transform.position = pos;
                m_slowFalling = false;
            }
            m_rigid.linearVelocity = velocity;
        }

        //==================================================||Movement
        public void ApplyMovement(float pDirection)
        {
            m_currentMoveInput = pDirection;
            if (Mathf.Abs(pDirection) > 0.01f)
            {
                m_lastMoveDirection = Mathf.Sign(pDirection);
            }
        }

        //==================================================||Dash
        public bool TryDash()
        {
            if (m_isDashing || m_dashCooldownTimer > 0f)
                return false;

            StartDash(m_lastMoveDirection);
            return true;
        }

        private void StartDash(float pDirection)
        {
            m_isDashing = true;
            m_dashTimer = 0f;
            m_dashDirection = pDirection;
        }

        private void UpdateDash()
        {
            if (m_isDashing)
            {
                m_dashTimer += Time.deltaTime;

                if (m_dashTimer >= m_dashDuration)
                {
                    m_isDashing = false;
                    m_dashCooldownTimer = m_dashCooldown;
                }
            }

            if (m_dashCooldownTimer > 0f)
            {
                m_dashCooldownTimer -= Time.deltaTime;
            }
        }

        //==================================================||Enhancement
        public void ApplyFallSpeedMultiplier(float pMultiplier)
        {
            m_gravityScale = -2 * JUMP_PEEK_HEIGHT / JUMP_PEEK_INTERVAL / JUMP_PEEK_INTERVAL * pMultiplier;
        }

        public void ApplyJumpStrengthMultiplier(float pMultiplier)
        {
            m_jumpScale = 2 * JUMP_PEEK_HEIGHT / JUMP_PEEK_INTERVAL * pMultiplier;
        }

        public void ApplyMoveSpeedMultiplier(float pMultiplier)
        {
            m_maxMoveSpeed = 10f * pMultiplier;
        }

        //==================================================||Properties
        public bool IsDashing => m_isDashing;
        public bool CanDash => m_dashCooldownTimer <= 0f && !m_isDashing;
        public float DashCooldownProgress => 1f - Mathf.Clamp01(m_dashCooldownTimer / m_dashCooldown);
        public bool IsGrounded => m_isGround;
        public float GetCurrentMoveInput() => m_currentMoveInput;

        //==================================================||Movement Process
        private void MoveProcess()
        {
            var velocity = m_rigid.linearVelocity;

            if (m_isDashing)
            {
                velocity.x = m_dashDirection * m_dashSpeed;
                m_rigid.linearVelocity = velocity;
                return;
            }

            float acceleration = m_isGround ? m_moveAcceleration : m_airAcceleration;
            float targetSpeed = m_currentMoveInput * m_maxMoveSpeed;

            if (Mathf.Abs(m_currentMoveInput) > 0.01f)
            {
                velocity.x = Mathf.Lerp(velocity.x, targetSpeed, acceleration * Time.deltaTime);
            }
            else
            {
                velocity.x = Mathf.Lerp(velocity.x, 0f, m_moveDeceleration * Time.deltaTime);
            }

            m_rigid.linearVelocity = velocity;
        }

        //==================================================||Unity
        private void Awake()
        {
            m_rigid = GetComponent<Rigidbody>();
            m_rigid.useGravity = false;

            m_rigid.constraints = RigidbodyConstraints.FreezeRotationX |
                                  RigidbodyConstraints.FreezeRotationY |
                                  RigidbodyConstraints.FreezeRotationZ;

            m_jumpScale = 2 * JUMP_PEEK_HEIGHT / JUMP_PEEK_INTERVAL;
            m_gravityScale = -m_jumpScale / JUMP_PEEK_INTERVAL;
        }

        private void Update()
        {
            UpdateDash();
            GravityProcess();
        }

        private void FixedUpdate()
        {
            MoveProcess();
        }

        /*It shows box which checking if player has contacted ground
        #if UNITY_EDITOR
                private void OnDrawGizmos() {
                    var scale = transform.localScale;
                    var center = transform.position;
                    center.y -= scale.y / 2;
                    scale *= GROUND_CHECK_OFFSET;
                    scale.y = m_rigid != null ? m_rigid.linearVelocity.y * Time.deltaTime : 0;
                    center.y += scale.y / 2;

                    Gizmos.color = Color.red;
                    Gizmos.DrawCube(center, scale);
                }
        #endif*/
    }
}