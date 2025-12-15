using UnityEngine;
using Game.Input;
using Game.Player;

namespace Game.Player
{
    public class PlayerController : MonoBehaviour
    {

        private PlayerMovement m_playerMovement = null;

        private void Awake()
        {
            m_playerMovement = GetComponent<PlayerMovement>();
        }

        private void Update()
        {
            float moveInput = InputManager.Instance.GetMoveInput();
            m_playerMovement.ApplyMovement(moveInput);

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

            if (InputManager.Instance.GetActionDown(InputAction.Dash))
            {
                m_playerMovement.TryDash();
            }
        }
    }
}