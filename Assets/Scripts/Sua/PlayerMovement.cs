using System.Linq;
using UnityEngine;

namespace Game.Player
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerMovement : MonoBehaviour
    {
        //==================================================||Constants 
        private const float JUMP_PEEK_HEIGHT = 4f;
        private const float JUMP_PEEK_INTERVAL = 0.3f;
        private const float JUMP_SCALE = 2 * JUMP_PEEK_HEIGHT / JUMP_PEEK_INTERVAL;
        private const float GRAVITY_SCALE = -JUMP_SCALE / JUMP_PEEK_INTERVAL;
        private const float GROUND_CHECK_OFFSET = 0.95f;
        private const float GROUND_CHECKER_HEIGHT = 0.01f;

        private const float MAX_MOVE_SPEED = 10f;
        private const float MOVE_ACCELERATION = 25f;
        private const float MOVE_DECELERATION = 20f;
        private const float AIR_ACCELERATION = 15f;

        private const float DASH_SPEED = 25f;
        private const float DASH_DURATION = 0.3f;
        private const float DASH_COOLDOWN = 0.5f;

        //==================================================||Fields 
        private Rigidbody m_rigid = null;
        private bool m_isGround = false;
        private bool m_slowFalling = false;
        private float m_slowFallingPower = 0.5f;
        private float m_currentMoveInput = 0f;

        private bool m_isDashing = false;
        private float m_dashTimer = 0f;
        private float m_dashCooldown = 0f;
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
            velocity.y += JUMP_SCALE;
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
                velocity.y += GRAVITY_SCALE * Time.deltaTime * m_slowFallingPower;
            else
                velocity.y += GRAVITY_SCALE * Time.deltaTime;
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
        public void ApplyMovement(float p_direction)
        {
            m_currentMoveInput = p_direction;
        }

        //==================================================||Dash
        public bool TryDash()
        {
            if (m_isDashing || m_dashCooldown > 0f)
                return false;

            float dashDir = Mathf.Abs(m_currentMoveInput) > 0.01f ?
                            Mathf.Sign(m_currentMoveInput) :
                            m_dashDirection;

            StartDash(dashDir);
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

                if (m_dashTimer >= DASH_DURATION)
                {
                    m_isDashing = false;
                    m_dashCooldown = DASH_COOLDOWN;
                }
            }

            if (m_dashCooldown > 0f)
            {
                m_dashCooldown -= Time.deltaTime;
            }
        }

        public bool IsDashing => m_isDashing;
        public bool CanDash => m_dashCooldown <= 0f && !m_isDashing;
        public float DashCooldownProgress => 1f - Mathf.Clamp01(m_dashCooldown / DASH_COOLDOWN);
        public bool IsGrounded => m_isGround;
        public float GetCurrentMoveInput() => m_currentMoveInput;

        private void MoveProcess()
        {
            var velocity = m_rigid.linearVelocity;

            if (m_isDashing)
            {
                velocity.x = m_dashDirection * DASH_SPEED;
                m_rigid.linearVelocity = velocity;
                return;
            }

            float acceleration = m_isGround ? MOVE_ACCELERATION : AIR_ACCELERATION;
            float targetSpeed = m_currentMoveInput * MAX_MOVE_SPEED;

            if (Mathf.Abs(m_currentMoveInput) > 0.01f)
            {
                velocity.x = Mathf.Lerp(velocity.x, targetSpeed, acceleration * Time.deltaTime);
            }
            else
            {
                velocity.x = Mathf.Lerp(velocity.x, 0f, MOVE_DECELERATION * Time.deltaTime);
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