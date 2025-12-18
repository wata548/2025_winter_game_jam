using UnityEngine;

namespace Game.UI
{
    public class UICloseButton : MonoBehaviour
    {
        [SerializeField] private GameObject m_panelToClose = null;
        [SerializeField] private GameObject m_panelToOpen = null;

        public void OnCloseClick()
        {
            if (m_panelToClose != null)
            {
                m_panelToClose.SetActive(false);
            }

            if (m_panelToOpen != null)
            {
                m_panelToOpen.SetActive(true);
            }

            Debug.Log("[CloseButton] Panel closed!");
        }
    }
}