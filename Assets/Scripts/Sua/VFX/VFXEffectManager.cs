using UnityEngine;
using UnityEngine.VFX;
using System.Collections.Generic;

namespace Game.VFX
{
    public class VFXEffectManager : MonoBehaviour
    {
        public enum EffectType
        {
            Healing,
            Hit,
            PoisonHit,
            NormalAttack,
            NormalAttackAir,
            PoisonAttack,
            PoisonAttackAir,
            PoisonMissile,
            Enhancement,
            Defense,
            OffenseBuff,
            Poison
        }

        [System.Serializable]
        public struct EffectData
        {
            public EffectType m_effectType;
            public GameObject m_effectPrefab;
            [Tooltip("풀 크기 (기본값: 4)")]
            public int m_poolSize;
        }

        [SerializeField]
        private EffectData[] m_effectDatabase;

        private Dictionary<EffectType, Queue<GameObject>> m_effectPool;
        private Dictionary<EffectType, GameObject> m_prefabMap;
        private Transform m_poolParent;

        private static VFXEffectManager m_instance;

        public static VFXEffectManager Instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = FindAnyObjectByType<VFXEffectManager>();
                    if (m_instance == null)
                    {
                        GameObject managerObj = new GameObject("VFXEffectManager");
                        m_instance = managerObj.AddComponent<VFXEffectManager>();
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

#if UNITY_EDITOR
            if (!UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
            {
                return;
            }
#endif

            DontDestroyOnLoad(gameObject);
            InitializePool();
        }

        private void InitializePool()
        {
            m_effectPool = new Dictionary<EffectType, Queue<GameObject>>();
            m_prefabMap = new Dictionary<EffectType, GameObject>();

            GameObject poolParentObj = new GameObject("VFXPool");
            poolParentObj.transform.SetParent(transform);
            m_poolParent = poolParentObj.transform;

            foreach (EffectData data in m_effectDatabase)
            {
                if (data.m_effectPrefab == null)
                {
                    Debug.LogWarning($"[VFXEffectManager] Effect prefab is null for: {data.m_effectType}");
                    continue;
                }

                int poolSize = data.m_poolSize > 0 ? data.m_poolSize : 4;

                m_prefabMap[data.m_effectType] = data.m_effectPrefab;
                Queue<GameObject> queue = new Queue<GameObject>();

                for (int i = 0; i < poolSize; i++)
                {
                    GameObject effect = Instantiate(data.m_effectPrefab, m_poolParent);
                    effect.SetActive(false);
                    queue.Enqueue(effect);
                }

                m_effectPool[data.m_effectType] = queue;
            }

            Debug.Log("[VFXEffectManager] Pool initialized successfully");
        }

        public GameObject PlayEffect(EffectType pEffectType, Vector3 pPosition)
        {
            return PlayEffect(pEffectType, pPosition, Quaternion.identity);
        }

        public GameObject PlayEffect(EffectType pEffectType, Vector3 pPosition, Quaternion pRotation)
        {
            if (!m_prefabMap.ContainsKey(pEffectType))
            {
                Debug.LogError($"[VFXEffectManager] Effect not found: {pEffectType}");
                return null;
            }

            GameObject effect = GetEffectFromPool(pEffectType);

            if (effect == null)
            {
                effect = Instantiate(m_prefabMap[pEffectType], m_poolParent);
            }

            effect.transform.SetPositionAndRotation(pPosition, pRotation);
            effect.SetActive(true);

            VisualEffect vfx = effect.GetComponent<VisualEffect>();
            if (vfx != null)
            {
                vfx.Play();
            }

            ParticleSystem ps = effect.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                ps.Play();
            }

            return effect;
        }

        public GameObject PlayEffectFollow(EffectType pEffectType, Transform pTargetTransform)
        {
            GameObject effect = PlayEffect(pEffectType, pTargetTransform.position);

            if (effect != null)
            {
                VFXFollowTransform follower = effect.GetComponent<VFXFollowTransform>();
                if (follower == null)
                {
                    follower = effect.AddComponent<VFXFollowTransform>();
                }
                follower.Initialize(pTargetTransform);
            }

            return effect;
        }

        public void ReturnEffect(GameObject pEffect, EffectType pEffectType, float pDelay = 0f)
        {
            if (pEffect == null) return;

            if (pDelay > 0)
            {
                StartCoroutine(ReturnEffectCoroutine(pEffect, pEffectType, pDelay));
            }
            else
            {
                ReturnEffectImmediate(pEffect, pEffectType);
            }
        }

        private void ReturnEffectImmediate(GameObject pEffect, EffectType pEffectType)
        {
            if (!m_effectPool.ContainsKey(pEffectType))
            {
                Destroy(pEffect);
                return;
            }

            pEffect.SetActive(false);
            m_effectPool[pEffectType].Enqueue(pEffect);
        }

        private System.Collections.IEnumerator ReturnEffectCoroutine(GameObject pEffect, EffectType pEffectType, float pDelay)
        {
            yield return new WaitForSeconds(pDelay);
            ReturnEffectImmediate(pEffect, pEffectType);
        }

        private GameObject GetEffectFromPool(EffectType pEffectType)
        {
            if (!m_effectPool.ContainsKey(pEffectType) || m_effectPool[pEffectType].Count == 0)
            {
                return null;
            }

            return m_effectPool[pEffectType].Dequeue();
        }

        public void ClearAllPools()
        {
            foreach (var queue in m_effectPool.Values)
            {
                foreach (var effect in queue)
                {
                    Destroy(effect);
                }
                queue.Clear();
            }
            m_effectPool.Clear();
            Debug.Log("[VFXEffectManager] All pools cleared");
        }
    }
}