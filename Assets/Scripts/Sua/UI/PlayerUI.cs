using UnityEngine;
using UnityEngine.UI;
using Game.Player.Stats;

namespace Game.Player.UI
{
    public class PlayerUI : MonoBehaviour
    {
        [SerializeField] private Image[] m_threadImages = new Image[10];
        [SerializeField] private Image[] m_heartImages = new Image[6];
        [SerializeField] private Text m_moltText = null;

        [SerializeField] private Sprite m_threadEmpty = null;
        [SerializeField] private Sprite m_threadHalf = null;
        [SerializeField] private Sprite m_threadFull = null;

        [SerializeField] private Sprite m_heartFull = null;
        [SerializeField] private Sprite m_heartEmpty = null;

        [SerializeField] private GameObject m_playerObject = null;

        private HealthSystem m_healthSystem = null;
        private ThreadSystem m_threadSystem = null;
        private MoltSystem m_moltSystem = null;
        private PlayerStats m_playerStats = null;

        private void Awake()
        {
            if (m_playerObject == null)
            {
                m_playerObject = GameObject.FindGameObjectWithTag("Player");
            }

            if (m_playerObject == null)
            {
                Debug.LogError("[PlayerUIDisplay] Player object not found!");
                return;
            }

            m_healthSystem = m_playerObject.GetComponent<HealthSystem>();
            m_threadSystem = m_playerObject.GetComponent<ThreadSystem>();
            m_moltSystem = m_playerObject.GetComponent<MoltSystem>();
            m_playerStats = m_playerObject.GetComponent<PlayerStats>();

            if (m_healthSystem == null) Debug.LogError("[PlayerUIDisplay] HealthSystem is missing!");
            if (m_threadSystem == null) Debug.LogError("[PlayerUIDisplay] ThreadSystem is missing!");
            if (m_moltSystem == null) Debug.LogError("[PlayerUIDisplay] MoltSystem is missing!");
            if (m_playerStats == null) Debug.LogError("[PlayerUIDisplay] PlayerStats is missing!");
        }

        private void Start()
        {
            m_healthSystem.OnHealthChanged += UpdateHearts;
            m_threadSystem.OnThreadChanged += UpdateThreads;
            m_moltSystem.OnMoltChanged += UpdateMolt;

            UpdateHearts(m_healthSystem.Hp);
            UpdateThreads(m_threadSystem.CurrentThread);
            UpdateMolt(m_moltSystem.CurrentMolt);
        }

        private void OnDestroy()
        {
            if (m_healthSystem != null) m_healthSystem.OnHealthChanged -= UpdateHearts;
            if (m_threadSystem != null) m_threadSystem.OnThreadChanged -= UpdateThreads;
            if (m_moltSystem != null) m_moltSystem.OnMoltChanged -= UpdateMolt;
        }

        private void UpdateThreads(int pCurrentThread)
        {
            for (int i = 0; i < m_threadImages.Length; i++)
            {
                int slotValue = (i + 1) * 20;
                int previousSlotValue = i * 20;

                if (pCurrentThread >= slotValue)
                {
                    m_threadImages[i].sprite = m_threadFull;
                }
                else if (pCurrentThread > previousSlotValue)
                {
                    m_threadImages[i].sprite = m_threadHalf;
                }
                else
                {
                    m_threadImages[i].sprite = m_threadEmpty;
                }
            }
        }

        private void UpdateHearts(int pCurrentHealth)
        {
            int maxHealth = m_playerStats.MaxHealth;

            for (int i = 0; i < m_heartImages.Length; i++)
            {
                if (i < maxHealth)
                {
                    m_heartImages[i].gameObject.SetActive(true);
                    m_heartImages[i].sprite = i < pCurrentHealth ? m_heartFull : m_heartEmpty;
                }
                else
                {
                    m_heartImages[i].gameObject.SetActive(false);
                }
            }
        }

        private void UpdateMolt(int pCurrentMolt)
        {
            m_moltText.text = pCurrentMolt.ToString();
        }
    }
}