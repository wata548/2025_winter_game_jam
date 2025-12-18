using UnityEngine;
using System;
using Game.Player.Stats;

namespace Game.Item
{
    public class Magnet : MonoBehaviour
    {

        public event Action<int> OnItemCollected;

        private Rigidbody m_rigid = null;
        private Collider m_collider = null;
        private MoltSystem m_moltSystem = null;

        [SerializeField] private float m_magnetRadius = 5f;
        [SerializeField] private float m_moveSpeed = 15f;
        [SerializeField] private string m_playerTag = "Player";
        [SerializeField] private int m_itemValue = 1;

        private Transform m_playerTransform = null;
        private bool m_isMovingToPlayer = false;

        private void Awake()
        {
            m_rigid = GetComponent<Rigidbody>();
            m_collider = GetComponent<Collider>();

            if (m_rigid == null)
            {
                m_rigid = gameObject.AddComponent<Rigidbody>();
                m_rigid.useGravity = false;
                m_rigid.constraints = RigidbodyConstraints.FreezeRotationX |
                                     RigidbodyConstraints.FreezeRotationY |
                                     RigidbodyConstraints.FreezeRotationZ;
            }
            if (m_collider == null)
            {
                Debug.LogError("[Magnet] Collider is missing!");
            }
        }

        private void Update()
        {
            DetectPlayer();
            MoveToPlayer();
        }

        private void DetectPlayer()
        {
            if (m_playerTransform != null) return;

            Collider[] colliders = Physics.OverlapSphere(transform.position, m_magnetRadius);
            foreach (Collider col in colliders)
            {
                if (col.CompareTag(m_playerTag))
                {
                    m_playerTransform = col.transform;
                    m_moltSystem = col.GetComponent<MoltSystem>();
                    m_isMovingToPlayer = true;
                    Debug.Log("[Magnet] Player detected! Moving towards player...");
                    break;
                }
            }
        }

        private void MoveToPlayer()
        {
            if (!m_isMovingToPlayer || m_playerTransform == null) return;

            Vector3 directionToPlayer = (m_playerTransform.position - transform.position).normalized;
            m_rigid.linearVelocity = directionToPlayer * m_moveSpeed;
        }

        private void OnTriggerEnter(Collider pCollider)
        {
            if (pCollider.CompareTag(m_playerTag))
            {
                CollectItem();
            }
        }

        private void CollectItem()
        {
            if (m_moltSystem != null)
            {
                m_moltSystem.AddMolt(m_itemValue);
                Debug.Log($"[Magnet] Item collected! Molt +{m_itemValue} ^^");
            }
            else
            {
                Debug.LogWarning("[Magnet] MoltSystem not found!");
            }

            OnItemCollected?.Invoke(m_itemValue);
            Destroy(gameObject);
        }

        public void SetItemValue(int pValue)
        {
            m_itemValue = pValue;
        }

        public int GetItemValue() => m_itemValue;
    }
}