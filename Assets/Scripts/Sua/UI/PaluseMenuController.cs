using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class PauseMenuUIController : MonoBehaviour
    {
        [SerializeField] private Image[] m_menuButtons = new Image[4];
        [SerializeField] private Color m_selectedColor = Color.yellow;
        [SerializeField] private Color m_deselectedColor = Color.white;

        private PauseMenuManager m_pauseMenuManager = null;
        private int m_lastMenuIndex = -1;

        private void Start()
        {
            m_pauseMenuManager = GetComponent<PauseMenuManager>();
            if (m_pauseMenuManager == null)
            {
                m_pauseMenuManager = FindAnyObjectByType<PauseMenuManager>();
            }

            if (m_pauseMenuManager == null)
            {
                Debug.LogError("[PauseMenuUIController] PauseMenuManager not found!");
                enabled = false;
            }
        }

        private void Update()
        {
            if (m_pauseMenuManager == null || m_menuButtons == null) return;

            if (m_pauseMenuManager.CurrentMenuIndex != m_lastMenuIndex)
            {
                UpdateButtonHighlight();
                m_lastMenuIndex = m_pauseMenuManager.CurrentMenuIndex;
            }
        }

        private void UpdateButtonHighlight()
        {
            if (m_menuButtons == null || m_menuButtons.Length < 4)
            {
                Debug.LogWarning("[PauseMenuUIController] Menu buttons not properly assigned!");
                return;
            }

            for (int i = 0; i < m_menuButtons.Length; i++)
            {
                if (m_menuButtons[i] == null)
                {
                    Debug.LogWarning($"[PauseMenuUIController] Menu button {i} is null!");
                    continue;
                }

                m_menuButtons[i].color = i == m_pauseMenuManager.CurrentMenuIndex
                    ? m_selectedColor
                    : m_deselectedColor;
            }
        }
    }
}