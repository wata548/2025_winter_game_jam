using System.Linq;
using UnityEngine;

namespace Game.Player.Movement
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerMovement : Physic.Movement
    {
        private const float WALL_CHECK_OFFSET = 1.1f;

        private float m_currentMoveInput = 0f;
        private float m_lastMoveDirection = 1f;

        private float m_jumpScale = 1;
        private float m_gravityScale = 1;

        [SerializeField] private float m_maxMoveSpeed = 10f;
        [SerializeField] private float m_moveAcceleration = 25f;
        [SerializeField] private float m_moveDeceleration = 20f;
        [SerializeField] private float m_airAcceleration = 15f;
        [SerializeField] private float m_rotationSpeed = 10f;

        [SerializeField] private float m_dashSpeed = 25f;
        [SerializeField] private float m_dashDuration = 0.3f;
        [SerializeField] private float m_dashCooldown = 0.5f;

        private bool m_isDashing = false;
        private float m_dashTimer = 0f;
        private float m_dashCooldownTimer = 0f;
        private float m_dashDirection = 1f;

        private bool m_inputBlocked = false;

        public void Jump()
        {
            if (m_inputBlocked) return;
            base.Jump(m_jumpScale);
        }

        public void SetActiveSlowFalling(bool pOn)
        {
            SetActiveSlowFalling(pOn, m_gravityScale);
        }

        public void ApplyMovement(float pDirection)
        {
            if (m_inputBlocked) return;

            m_currentMoveInput = pDirection;
            if (Mathf.Abs(pDirection) > 0.01f)
            {
                m_lastMoveDirection = Mathf.Sign(pDirection);
            }
        }

        private void OffsetXMovement()
        {
            var velocity = _rigid.linearVelocity;
            if (velocity.x == 0)
                return;

            var bounds = Collider.bounds;
            var halfSize = new Vector3(WALL_CHECK_OFFSET * bounds.size.x, GROUND_CHECK_OFFSET * bounds.size.y, bounds.size.z) / 2;
            var temp = Physics.OverlapBox(
                bounds.center,
                halfSize,
                Quaternion.identity,
                LayerMask.GetMask("Ground")
            );
            
            var wall = Physics.OverlapBox(
                bounds.center,
                halfSize,
                Quaternion.identity,
                LayerMask.GetMask("Ground")
            ).Any(collider => {
                var interval = collider.bounds.center.x - bounds.center.x;
                if (interval == 0)
                    return false;
                return Mathf.Sign(velocity.x) == Mathf.Sign(interval);
            });
            if (wall)
                velocity.x = 0;

            _rigid.linearVelocity = velocity;
        }

        private void UpdateRotation()
        {
            float targetRotationY = m_lastMoveDirection > 0 ? 90f : 270f;
            float currentRotationY = transform.eulerAngles.y;

            if (currentRotationY > 180f)
            {
                currentRotationY -= 360f;
            }

            float newRotationY = Mathf.LerpAngle(currentRotationY, targetRotationY, m_rotationSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Euler(0, newRotationY, 0);
        }

        public bool TryDash()
        {
            if (m_inputBlocked) return false;
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

        public void ApplyFallSpeedMultiplier(float pMultiplier) =>
            m_gravityScale = pMultiplier;

        public void ApplyJumpStrengthMultiplier(float pMultiplier) =>
            m_jumpScale = pMultiplier;

        public void ApplyMoveSpeedMultiplier(float pMultiplier) =>
            m_maxMoveSpeed = 10f * pMultiplier;

        public void BlockInput()
        {
            m_inputBlocked = true;
            m_currentMoveInput = 0f;
        }

        public void UnblockInput()
        {
            m_inputBlocked = false;
        }

        public bool IsInputBlocked => m_inputBlocked;
        public bool IsDashing => m_isDashing;
        public bool CanDash => m_dashCooldownTimer <= 0f && !m_isDashing;
        public float DashCooldownProgress => 1f - Mathf.Clamp01(m_dashCooldownTimer / m_dashCooldown);
        public bool IsGrounded => IsGround;
        public float GetCurrentMoveInput() => m_currentMoveInput;
        public Vector3 GetVelocity() => _rigid.linearVelocity;

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

        protected override void Awake()
        {
            base.Awake();
            _rigid.constraints = RigidbodyConstraints.FreezeRotationX |
                                 RigidbodyConstraints.FreezeRotationY |
                                 RigidbodyConstraints.FreezeRotationZ;
        }

        protected override void Update()
        {
            base.Update();
            UpdateDash();
            UpdateRotation();
        }

        private void FixedUpdate()
        {
            MoveProcess();
            OffsetXMovement();
        }


    }
}