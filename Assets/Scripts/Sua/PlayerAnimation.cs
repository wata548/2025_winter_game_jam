using UnityEngine;
using Extension.Test;

namespace Game.Player
{
    public class PlayerAnimator : MonoBehaviour
    {

        private Animator m_animator = null;
        private Movement.PlayerMovement m_playerMovement = null;
        private Combat.AttackSystem m_attackSystem = null;

        private readonly int HASH_SPEED = Animator.StringToHash("Speed");
        private readonly int HASH_JUMP = Animator.StringToHash("Jump");
        private readonly int HASH_FALL = Animator.StringToHash("Fall");
        private readonly int HASH_LANDING = Animator.StringToHash("Landing");
        private readonly int HASH_ATTACK1 = Animator.StringToHash("Attack1");
        private readonly int HASH_ATTACK2 = Animator.StringToHash("Attack2");
        private readonly int HASH_AIR_ATTACK = Animator.StringToHash("AirAttack");

        private bool m_isInAir = false;
        private bool m_wasInAir = false;
        private bool m_isAttacking = false;
        private bool m_wasAttacking = false;
        private bool m_canCombo = false;


        private void Awake()
        {
            m_animator = GetComponent<Animator>();
            m_playerMovement = GetComponent<Movement.PlayerMovement>();
            m_attackSystem = GetComponent<Combat.AttackSystem>();

            if (m_animator == null) Debug.LogError("[PlayerAnimator] Animator missing!");
            if (m_playerMovement == null) Debug.LogError("[PlayerAnimator] PlayerMovement missing!");
            if (m_attackSystem == null) Debug.LogError("[PlayerAnimator] AttackSystem missing!");
        }

        private void Update()
        {
            UpdateMovementAnimation();
            UpdateAttackAnimation();
            UpdateJumpFallAnimation();
        }

        private void UpdateMovementAnimation()
        {
            float moveInput = m_playerMovement.GetCurrentMoveInput();
            m_animator.SetFloat(HASH_SPEED, Mathf.Abs(moveInput));
        }

        private void UpdateJumpFallAnimation()
        {
            bool isGrounded = m_playerMovement.IsGrounded;
            m_wasInAir = m_isInAir;
            m_isInAir = !isGrounded;

            Vector3 velocity = m_playerMovement.GetVelocity();

            if (!m_wasInAir && m_isInAir && velocity.y > 0)
            {
                if (!m_isAttacking)
                {
                    m_animator.SetTrigger(HASH_JUMP);
                    Debug.Log("[PlayerAnimator] Jump triggered!");
                }
            }

            if (m_isInAir && velocity.y < 0)
            {
                if (!m_isAttacking)
                {
                    m_animator.SetTrigger(HASH_FALL);
                    Debug.Log("[PlayerAnimator] Fall triggered!");
                }
            }

            if (m_wasInAir && !m_isInAir)
            {
                if (!m_isAttacking)
                {
                    m_animator.SetTrigger(HASH_LANDING);
                    Debug.Log("[PlayerAnimator] Landing triggered!");
                }
            }
        }

        private void UpdateAttackAnimation()
        {
            m_wasAttacking = m_isAttacking;
            bool normalAttackActive = !m_attackSystem.CanAttack(Combat.AttackType.Normal);
            bool aerialAttackActive = !m_attackSystem.CanAttack(Combat.AttackType.Aerial);
            m_isAttacking = normalAttackActive || aerialAttackActive;

            if (m_isAttacking && !m_wasAttacking)
            {
                if (aerialAttackActive)
                {
                    m_animator.SetTrigger(HASH_AIR_ATTACK);
                    Debug.Log("[PlayerAnimator] Air Attack triggered!");
                    m_canCombo = false;
                }
                else if (normalAttackActive)
                {
                    if (m_canCombo)
                    {
                        m_animator.SetTrigger(HASH_ATTACK2);
                        Debug.Log("[PlayerAnimator] Attack2 (combo) triggered!");
                        m_canCombo = false;
                    }
                    else
                    {
                        m_animator.SetTrigger(HASH_ATTACK1);
                        Debug.Log("[PlayerAnimator] Attack1 triggered!");
                        m_canCombo = true;
                    }
                }
            }
        }

        public void OnAttack1End()
        {
            Debug.Log("[PlayerAnimator] Attack1 animation ended!");

            CancelInvoke(nameof(ResetCombo));
            Invoke(nameof(ResetCombo), 0.3f);
        }

        public void OnAttack2End()
        {
            Debug.Log("[PlayerAnimator] Attack2 animation ended!");
            ResetCombo();
        }

        public void OnLandingEnd()
        {
            Debug.Log("[PlayerAnimator] Landing animation ended!");
        }

        private void ResetCombo()
        {
            m_canCombo = false;
            Debug.Log("[PlayerAnimator] Combo reset!");
        }

        //==================================================||Test Methods

        [TestMethod("Test Idle")]
        private void TestIdle()
        {
            m_animator.ResetTrigger(HASH_ATTACK1);
            m_animator.ResetTrigger(HASH_ATTACK2);
            m_animator.SetFloat(HASH_SPEED, 0f);
            Debug.Log("[PlayerAnimator] Test Idle");
        }

        [TestMethod("Test Walk")]
        private void TestWalk()
        {
            m_animator.SetFloat(HASH_SPEED, 1f);
            Debug.Log("[PlayerAnimator] Test Walk");
        }

        [TestMethod("Test Jump")]
        private void TestJump()
        {
            m_animator.SetTrigger(HASH_JUMP);
            Debug.Log("[PlayerAnimator] Test Jump");
        }

        [TestMethod("Test Fall")]
        private void TestFall()
        {
            m_animator.SetTrigger(HASH_FALL);
            Debug.Log("[PlayerAnimator] Test Fall");
        }

        [TestMethod("Test Landing")]
        private void TestLanding()
        {
            m_animator.SetTrigger(HASH_LANDING);
            Debug.Log("[PlayerAnimator] Test Landing");
        }

        [TestMethod("Test Attack1")]
        private void TestAttack1()
        {
            m_animator.SetTrigger(HASH_ATTACK1);
            Debug.Log("[PlayerAnimator] Test Attack1");
        }

        [TestMethod("Test Attack2")]
        private void TestAttack2()
        {
            m_animator.SetTrigger(HASH_ATTACK2);
            Debug.Log("[PlayerAnimator] Test Attack2");
        }

        [TestMethod("Test Air Attack")]
        private void TestAirAttack()
        {
            m_animator.SetTrigger(HASH_AIR_ATTACK);
            Debug.Log("[PlayerAnimator] Test Air Attack");
        }
    }
}