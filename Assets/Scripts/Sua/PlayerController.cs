using UnityEngine;
using UInput = UnityEngine.Input;
using Game.Input;
using Game.Player.Movement;
using Game.Player.Combat;
using Game.Player.Stats;

namespace Game.Player
{
    public class PlayerController : MonoBehaviour
    {

        private PlayerMovement m_playerMovement = null;
        private AttackSystem m_attackSystem = null;
        private HealingSystem m_healingSystem = null;

        private void Awake()
        {
            m_playerMovement = GetComponent<PlayerMovement>();
            m_attackSystem = GetComponent<AttackSystem>();
            m_healingSystem = GetComponent<HealingSystem>();

            if (m_playerMovement == null)
            {
                Debug.LogError("[PlayerController] PlayerMovement is missing!");
            }
            if (m_attackSystem == null)
            {
                Debug.LogError("[PlayerController] AttackSystem is missing!");
            }
            if (m_healingSystem == null)
            {
                Debug.LogError("[PlayerController] HealingSystem is missing!");
            }
        }

        private void Update()
        {
            HandleMovementInput();
            HandleJumpInput();
            HandleDashInput();
            HandleAttackInput();
            HandleHealingInput();
        }

        private void HandleMovementInput()
        {
            float moveInput = InputManager.Instance.GetMoveInput();
            m_playerMovement.ApplyMovement(moveInput);
        }

        private void HandleJumpInput()
        {
            if (InputManager.Instance.GetActionDown(InputAction.Jump))
            {
                if (m_playerMovement.IsGrounded)
                {
                    m_playerMovement.Jump();
                }
                else
                {
                    m_playerMovement.SetActiveSlowFalling(true);
                }
            }
        }

        private void HandleDashInput()
        {
            if (InputManager.Instance.GetActionDown(InputAction.Dash))
            {
                m_playerMovement.TryDash();
            }
        }

        private void HandleAttackInput()
        {
            if (InputManager.Instance.GetActionDown(InputAction.Attack))
            {
                float moveInput = InputManager.Instance.GetMoveInput();
                m_attackSystem.TryAttack(moveInput);
            }
        }

        private void HandleHealingInput()
        {
            if (InputManager.Instance.GetAction(InputAction.Heal))
            {
                m_healingSystem.StartHealing();
            }
            if (InputManager.Instance.GetActionUp(InputAction.Heal))
            {
                m_healingSystem.StopHealing();
            }
        }
    }
}