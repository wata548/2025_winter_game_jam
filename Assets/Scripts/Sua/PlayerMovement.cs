using System.Linq;
using UnityEngine;

namespace Game.Player.Movement
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerMovement : Physic.Movement
    {
        //==================================================||Constants 
        private const float WALL_CHECK_OFFSET = 1.1f;

        //==================================================||Fields - Movement
        private float m_currentMoveInput = 0f;
        private float m_lastMoveDirection = 1f;

        //==================================================||Fields - Jump & Gravity
        private float m_jumpScale = 1;
        private float m_gravityScale = 1;

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
        
        //==================================================||Jump & Falling
        public void Jump() {
            base.Jump(m_jumpScale);
        }
        public void SetActiveSlowFalling(bool pOn) {
            SetActiveSlowFalling(pOn, m_gravityScale);
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
        
        private void OffsetXMovement() {
            var velocity = _rigid.linearVelocity;
            if (velocity.x == 0)
                return;
            
            var halfSize = new Vector3(WALL_CHECK_OFFSET, GROUND_CHECK_OFFSET, transform.localScale.z) / 2;
            var wall = Physics.OverlapBox(
                transform.position,
                halfSize,
                Quaternion.identity,
                LayerMask.GetMask("Ground")
            ).Any(collider => {
                var interval = collider.transform.position.x - transform.position.x;
                if (interval == 0)
                    return false;
                return Mathf.Sign(velocity.x) == Mathf.Sign(interval);
            });
            if (wall)
                velocity.x = 0;

            _rigid.linearVelocity = velocity;
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
        public void ApplyFallSpeedMultiplier(float pMultiplier) =>
            m_gravityScale = pMultiplier;

        public void ApplyJumpStrengthMultiplier(float pMultiplier) =>
            m_jumpScale = pMultiplier;

        public void ApplyMoveSpeedMultiplier(float pMultiplier) =>
            m_maxMoveSpeed = 10f * pMultiplier;

        //==================================================||Properties
        public bool IsDashing => m_isDashing;
        public bool CanDash => m_dashCooldownTimer <= 0f && !m_isDashing;
        public float DashCooldownProgress => 1f - Mathf.Clamp01(m_dashCooldownTimer / m_dashCooldown);
        public bool IsGrounded => IsGround;
        public float GetCurrentMoveInput() => m_currentMoveInput;

        //==================================================||Movement Process
        private void MoveProcess()
        {
            var velocity = _rigid.linearVelocity;

            if (m_isDashing)
            {
                velocity.x = m_dashDirection * m_dashSpeed;
                _rigid.linearVelocity = velocity;
                return;
            }

            float acceleration = IsGround ? m_moveAcceleration : m_airAcceleration;
            float targetSpeed = m_currentMoveInput * m_maxMoveSpeed;

            if (Mathf.Abs(m_currentMoveInput) > 0.01f)
            {
                velocity.x = Mathf.Lerp(velocity.x, targetSpeed, acceleration * Time.deltaTime);
            }
            else
            {
                velocity.x = Mathf.Lerp(velocity.x, 0f, m_moveDeceleration * Time.deltaTime);
            }

            _rigid.linearVelocity = velocity;
        }

        //==================================================||Unity
        protected override void Awake() {
            base.Awake();
            _rigid.constraints = RigidbodyConstraints.FreezeRotationX |
                                 RigidbodyConstraints.FreezeRotationY |
                                 RigidbodyConstraints.FreezeRotationZ;
        }

        protected override void Update() {
            base.Update();
            UpdateDash();
        }

        private void FixedUpdate()
        {
            MoveProcess();
            OffsetXMovement();
        }
    }
}