using UnityEngine;
using UInput = UnityEngine.Input;
using GInput = Game.Input;

namespace Game.Input
{
    public enum InputAction
    {
        MoveLeft,
        MoveRight,
        Jump,
        Dash,
    }

    public class InputManager : MonoBehaviour
    {
        private static InputManager m_instance = null;

        //==================================================||Basic Input Bindings
        private KeyCode m_moveLeftKey = KeyCode.A;
        private KeyCode m_moveRightKey = KeyCode.D;
        private KeyCode m_jumpKey = KeyCode.Space;
        private KeyCode m_dashKey = KeyCode.LeftShift;

        //==================================================||Singleton
        public static InputManager Instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = FindAnyObjectByType<InputManager>();
                    if (m_instance == null)
                    {
                        var obj = new GameObject("InputManager");
                        m_instance = obj.AddComponent<InputManager>();
                    }
                }
                return m_instance;
            }
        }

        private void Awake()
        {
            if (m_instance != null && m_instance != this)
            {
                Destroy(gameObject);
                return;
            }
            m_instance = this;
            DontDestroyOnLoad(gameObject);
        }

        //==================================================||Key Binding
        public void SetKeyBinding(InputAction pAction, KeyCode pKeyCode)
        {
            switch (pAction)
            {
                case InputAction.MoveLeft:
                    m_moveLeftKey = pKeyCode;
                    break;
                case InputAction.MoveRight:
                    m_moveRightKey = pKeyCode;
                    break;
                case InputAction.Jump:
                    m_jumpKey = pKeyCode;
                    break;
                case InputAction.Dash:
                    m_dashKey = pKeyCode;
                    break;
            }
        }

        public KeyCode GetKeyBinding(InputAction pAction)
        {
            return pAction switch
            {
                InputAction.MoveLeft => m_moveLeftKey,
                InputAction.MoveRight => m_moveRightKey,
                InputAction.Jump => m_jumpKey,
                InputAction.Dash => m_dashKey,
                _ => KeyCode.None,
            };
        }

        //==================================================||Input Check Methods

        public bool GetActionDown(InputAction pAction)
        {
            return UInput.GetKeyDown(GetKeyBinding(pAction));
        }

        public bool GetAction(InputAction pAction)
        {
            return UInput.GetKey(GetKeyBinding(pAction));
        }

        public bool GetActionUp(InputAction pAction)
        {
            return UInput.GetKeyUp(GetKeyBinding(pAction));
        }

        //==================================================||Movement Input
        public float GetMoveInput()
        {
            float input = 0f;
            if (GetAction(InputAction.MoveLeft))
                input -= 1f;
            if (GetAction(InputAction.MoveRight))
                input += 1f;
            return Mathf.Clamp(input, -1f, 1f);
        }
    }
}