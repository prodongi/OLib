using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System;

namespace OLib
{
    public struct PoolInstantiateData
    {
        public string assetBundleName;
        public string assetName;
        public GameObject parent;
        public bool isUi;

        public PoolInstantiateData(string _assetBundleName, string _assetName, GameObject _parent, bool _isUi)
        {
            assetBundleName = _assetBundleName;
            assetName = _assetName;
            parent = _parent;
            isUi = _isUi;
        }
    }

    [ComVisible(false)]
    public class ObjectPoolManager<T, POOL> : MonoSingleton<T> where T : MonoBehaviour where POOL : ObjectPool, new() {
        private class DelayPoolInfo
        {
            public float m_elapsedTime = 0.0f;
            public float m_delay = 0.0f;
            public GameObject m_obj = null;
        }

        public int m_destroyElapsedAccessSecond = 0;

        private bool m_usePool = false;
        private Dictionary<string, POOL> m_pools = new Dictionary<string, POOL>();
        private List<DelayPoolInfo> m_delayPoolInfo = new List<DelayPoolInfo>();

        void Update()
        {
            updateDelayPoolInfo();
        }

        public void initialize(bool usePool)
        {
            m_usePool = usePool;
        }

        public void finalize()
        {
            pushAllDelayPoolInfo();
            destroyElapsedAccessTimeObject(m_destroyElapsedAccessSecond);
            clearUsedAll();
        }

        private void updateDelayPoolInfo()
        {
            for (int i = m_delayPoolInfo.Count - 1; i >= 0; --i)
            {
                DelayPoolInfo info = m_delayPoolInfo[i];

                info.m_elapsedTime += Time.deltaTime;
                if (info.m_elapsedTime >= info.m_delay)
                {
                    push(info.m_obj);
                    m_delayPoolInfo.RemoveAt(i);
                }
            }
        }

        private void pushAllDelayPoolInfo()
        {
            for (int i = m_delayPoolInfo.Count - 1; i >= 0; --i)
            {
                push(m_delayPoolInfo[i].m_obj);
            }

            m_delayPoolInfo.Clear();
        }

        public void preLoading(string assetBundleName, string assetName, bool isUi, bool isDontDestroy)
        {
            POOL pool = createPool(assetBundleName, assetName, isUi, isDontDestroy);
            instantiatePool(new PoolInstantiateData(assetBundleName, assetName, gameObject, isUi), (inst, error) =>
            {
                if (string.IsNullOrEmpty(error))
                {
                    pool.push(inst, true);
                }
                else
                {
                    OLib.Console.warning(string.Format("Failed preloading, not resource getInstance {0}, error {1}", assetName, error));
                }
            });
        }

        private POOL createPool(string assetBundleName, string assetName, bool isUi, bool isDontDestroy)
        {
            POOL objPool;
            if (m_pools.TryGetValue(assetName, out objPool))
                return objPool;

            objPool = new POOL();
            objPool.initialize(gameObject, assetBundleName, assetName, isUi, isDontDestroy, instantiatePool);
            m_pools.Add(assetName, objPool);

            return objPool;
        }

        public P pop<P>(GameObject parent, string assetBundleName, string assetName, bool isUi, bool isDontDestroy) where P : class
        {
            GameObject obj = pop(parent, assetBundleName, assetName, isUi, isDontDestroy);
            if (null == obj)
                return null;

            return obj.GetComponent<P>();
        }

        public GameObject pop(GameObject parent, string assetBundleName, string assetName, bool isUi, bool isDontDestroy = false)
        {
            if (m_usePool)
            {
                POOL pool;
                if (!m_pools.TryGetValue(assetName, out pool))
                {
                    pool = createPool(assetBundleName, assetName, isUi, isDontDestroy);
                }

                if (null == pool)
                    return null;

                return pool.pop(parent);
            }
            else
            {
                GameObject obj = null;
                instantiatePool(new PoolInstantiateData(assetBundleName, assetName, parent, isUi), (inst, error) =>
                {
                    if (string.IsNullOrEmpty(error))
                    {
                        obj = inst;
                    }
                    else
                    {
                        OLib.Console.error(error);
                    }
                });
                
                if (null == obj)
                    return null;

                obj.name = name;
                ObjectBasePool basePool = obj.GetComponent<ObjectBasePool>();
                if (null == basePool)
                {
                    Console.error(string.Format("ObjectBasePool component is null, {0} is not ObjectBasePool inheritance", obj.name));
                    return null;
                }

                basePool.initPool();

                return obj;
            }
        }

        protected virtual void instantiatePool(PoolInstantiateData data, Action<GameObject, string> callback)
        {
            if (null != callback)
                callback(null, "need override instantiate");
        }

        public void push(GameObject obj)
        {
            if (null == obj)
            {
                Console.warning("Failed push gameobject, obj is null");
                return;
            }

            POOL pool;
            if (!m_pools.TryGetValue(obj.name, out pool))
            {
                obj.SetActive(false);
                GameObject.Destroy(obj);
            }
            else
            {
                pool.push(obj, true);
            }
        }

        /**
         *  @brief delay후에 push를 한다.
         * */
        public void push(GameObject obj, float delay)
        {
            DelayPoolInfo info = new DelayPoolInfo();
            info.m_obj = obj;
            info.m_delay = delay;

            m_delayPoolInfo.Add(info);
        }

        protected virtual void instanceObject()
        {

        }

        private void clearUsedAll()
        {
            foreach (KeyValuePair<string, POOL> pair in m_pools)
            {
                pair.Value.clearUsed();
            }
        }

        /// <summary>
        /// 마지막 억세스 시간이 m분 이상이면 destroy한다.
        /// </summary>
        private void destroyElapsedAccessTimeObject(float second)
        {
            foreach (KeyValuePair<string, POOL> pair in m_pools)
            {
                pair.Value.destroyElapsedAccessTimeObject(second);
            }
        }
    }
}