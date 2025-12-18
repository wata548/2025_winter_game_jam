using UnityEngine;
using Game.Input;
using UInput = UnityEngine.Input;

namespace Game.UI
{
    public class PauseMenuManager : MonoBehaviour
    {
        [SerializeField] private GameObject m_pauseMenuPanel = null;
        [SerializeField] private GameObject m_settingsPanel = null;
        [SerializeField] private GameObject m_enhancementPanel = null;
        [SerializeField] private GameObject m_confirmPanel = null;

        private bool m_isPaused = false;
        private int m_currentMenuIndex = 0;
        private const int MENU_COUNT = 4;
        private float m_navigationCooldown = 0f;
        private const float NAVIGATION_DELAY = 0.2f;

        private void Update()
        {
            if (InputManager.Instance.GetActionDown(InputAction.Menu))
            {
                TogglePause();
            }

            if (!m_isPaused) return;

            UpdateNavigationCooldown();
            HandleMenuNavigation();
            HandleMenuSelection();
        }

        private void UpdateNavigationCooldown()
        {
            if (m_navigationCooldown > 0f)
            {
                m_navigationCooldown -= Time.unscaledDeltaTime;
            }
        }

        private void TogglePause()
        {
            m_isPaused = !m_isPaused;
            Time.timeScale = m_isPaused ? 0f : 1f;

            if (m_pauseMenuPanel != null)
                m_pauseMenuPanel.SetActive(m_isPaused);
            CloseAllPanels();
            m_currentMenuIndex = 0;
        }

        private void CloseAllPanels()
        {
            if (m_settingsPanel != null) m_settingsPanel.SetActive(false);
            if (m_enhancementPanel != null) m_enhancementPanel.SetActive(false);
            if (m_confirmPanel != null) m_confirmPanel.SetActive(false);
        }

        private void HandleMenuNavigation()
        {
            if (m_navigationCooldown > 0f) return;

            float moveInput = InputManager.Instance.GetMoveInput();

            if (moveInput > 0.5f)
            {
                m_currentMenuIndex = (m_currentMenuIndex + 1) % MENU_COUNT;
                m_navigationCooldown = NAVIGATION_DELAY;
                Debug.Log($"[PauseMenuManager] Menu Index: {m_currentMenuIndex}");
            }
            else if (moveInput < -0.5f)
            {
                m_currentMenuIndex = (m_currentMenuIndex - 1 + MENU_COUNT) % MENU_COUNT;
                m_navigationCooldown = NAVIGATION_DELAY;
                Debug.Log($"[PauseMenuManager] Menu Index: {m_currentMenuIndex}");
            }
        }

        private void HandleMenuSelection()
        {
            bool isSelectPressed = InputManager.Instance.GetActionDown(InputAction.Attack) ||
                                  UInput.GetKeyDown(KeyCode.Return) ||
                                  UInput.GetKeyDown(KeyCode.U);

            if (!isSelectPressed) return;

            switch (m_currentMenuIndex)
            {
                case 0:
                    Resume();
                    break;
                case 1:
                    OpenEnhancement();
                    break;
                case 2:
                    OpenSettings();
                    break;
                case 3:
                    OpenConfirmDialog();
                    break;
            }
        }

        private void Resume()
        {
            m_isPaused = false;
            Time.timeScale = 1f;
            m_pauseMenuPanel.SetActive(false);
            CloseAllPanels();
            Debug.Log("[PauseMenuManager] Game resumed!");
        }

        private void OpenEnhancement()
        {
            if (m_enhancementPanel != null)
            {
                m_enhancementPanel.SetActive(true);
                CanvasGroup canvasGroup = m_enhancementPanel.GetComponent<CanvasGroup>();
                if (canvasGroup != null) canvasGroup.alpha = 1f;
                Debug.Log("[PauseMenuManager] Enhancement menu opened!");
            }
            else
            {
                Debug.LogError("[PauseMenuManager] Enhancement panel not assigned!");
            }
        }

        private void OpenSettings()
        {
            if (m_settingsPanel != null)
            {
                m_settingsPanel.SetActive(true);
                CanvasGroup canvasGroup = m_settingsPanel.GetComponent<CanvasGroup>();
                if (canvasGroup != null) canvasGroup.alpha = 1f;
                Debug.Log("[PauseMenuManager] Settings menu opened!");
            }
            else
            {
                Debug.LogError("[PauseMenuManager] Settings panel not assigned!");
            }
        }

        private void OpenConfirmDialog()
        {
            m_pauseMenuPanel.SetActive(false);
            m_confirmPanel.SetActive(true);
            Debug.Log("[PauseMenuManager] Confirm dialog opened!");
        }

        public void OnConfirmYes()
        {
            Time.timeScale = 1f;
            m_isPaused = false;
            UnityEngine.SceneManagement.SceneManager.LoadScene("Title");
            Debug.Log("[PauseMenuManager] Loading title scene!");
        }

        public void OnConfirmNo()
        {
            m_confirmPanel.SetActive(false);
            m_pauseMenuPanel.SetActive(true);
            m_currentMenuIndex = 0;
            Debug.Log("[PauseMenuManager] Confirm canceled!");
        }

        public void OnSettingsBack()
        {
            m_settingsPanel.SetActive(false);
        }

        public void OnEnhancementBack()
        {
            m_enhancementPanel.SetActive(false);
        }

        public int CurrentMenuIndex => m_currentMenuIndex;
    }
}