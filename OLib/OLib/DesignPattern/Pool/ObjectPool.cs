using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

namespace OLib
{
    public class ObjectPool {
        private string m_assetName;
        private string m_assetBundleName;
        private GameObject m_poolParent = null;
        private bool m_isUi = false;
        private bool m_isDontDestroy = false;
        private Vector3 m_defaultLocalScale = Vector3.one;
        private Pool<GameObject> m_pool = new Pool<GameObject>();
        /// @brief 중복 체크에 쓰임
        private HashSet<int> m_instanceIds = new HashSet<int>();
        /// <summary>
        /// pop 할때 칼라 값을 초기 상태로 만들어 줘야 되서, 기본 값을 갖고 있는다.
        /// </summary>
        private Dictionary<string, Color> m_defaultColors = new Dictionary<string, Color>();
        private Action<PoolInstantiateData, Action<GameObject, string>> m_instantiateCallback = null;

        public void initialize(GameObject parent, string assetBundleName, string assetName, bool isUi, bool isDontDestroy, 
                                                                             Action<PoolInstantiateData, Action<GameObject, string>> instantiateCallback)
        {
            m_poolParent = parent;
            m_assetName = assetName;
            m_assetBundleName = assetBundleName;
            m_isUi = isUi;
            m_isDontDestroy = isDontDestroy;
            m_instantiateCallback = instantiateCallback;
        }

        public void push(GameObject obj, bool isRemoveUsed = true)
        {
            if (null == obj)
                return;

            if (m_instanceIds.Contains(obj.GetInstanceID()))
            {
                Console.error(string.Format("GameObjectPool push is duplicated, name : {0}, instance id : {1}", obj.name, obj.GetInstanceID()));
                return;
            }

            //Debug.LogFormat("push {0}", obj.name);

            if (m_isUi)
                obj.transform.SetParent(m_poolParent.transform);
            else
                obj.transform.parent = m_poolParent.transform;
            obj.SetActive(false);
            //restoreDefaultColor(obj);

            //Debug.LogFormat("push {0}, instance id {1}", obj.name, obj.GetInstanceID());

            m_pool.push(obj, isRemoveUsed);

            m_instanceIds.Add(obj.GetInstanceID());
        }

        public GameObject pop(GameObject parent)
        {
            GameObject obj = m_pool.pop();
            if (null == obj)
            {
                if (m_pool.isEmptyUnUsed())
                    reallocate();
                obj = m_pool.pop();
                if (null == obj)
                    return null;
            }

            //Vector3 localScale = obj.transform.localScale;

            /**
            *   set active 전에 restore 해줘야 된다. 안그러면 적용이 안됨, 왜 그런건가??
            */
            restoreDefaultColor(obj);
            obj.SetActive(true);
            if (null != parent)
            {
                if (m_isUi)
                    obj.transform.SetParent(parent.transform);
                else
                    obj.transform.parent = parent.transform;
            }
            obj.transform.position = Vector3.zero;
            obj.transform.localScale = m_defaultLocalScale;
            obj.transform.localRotation = Quaternion.identity;

            ObjectBasePool basePool = obj.GetComponent<ObjectBasePool>();
            if (null == basePool)
            {
                Console.error(string.Format("GameObjectBasePool component is null, {0} is not GameObjectBasePool inheritance", m_assetName));
                return obj;
            }

            basePool.initPool();

            //Debug.LogFormat("pop {0}, instance id {1}", obj.name, obj.GetInstanceID());

            m_instanceIds.Remove(obj.GetInstanceID());

            return obj;
        }

        private void reallocate()
        {
            allocate(1);
        }

        private void allocate(int count)
        {
            for (int i = 0; i < count; ++i)
            {
                GameObject obj = null;
                //AssetBundleManager.instance.instantiateAsset(m_assetBundleName, m_assetName, Vector3.one, Vector3.zero, m_poolParent, m_isUi, (inst, error) =>
                m_instantiateCallback(new PoolInstantiateData(m_assetBundleName, m_assetName, m_poolParent, m_isUi), (inst, error) =>
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
                    continue;

                obj.name = m_assetName;// System.IO.Path.GetFileNameWithoutExtension(m_path);
                obj.SetActive(false);

                if (m_pool.isEmpty())
                {
                    m_defaultLocalScale = obj.transform.localScale;
                    saveDefaultColors(obj);
                }

                m_pool.addUnUsed(obj);
            }
        }

        private void saveDefaultColors(GameObject obj)
        {
            GraphicHelper.getSpriteColors(obj.transform, m_defaultColors, m_isUi);
        }

        private void restoreDefaultColor(GameObject obj)
        {
            GraphicHelper.setSpriteColors(obj.transform, m_defaultColors, m_isUi);
        }

        /**
        *   사용 된 것들을 사용 안된것으로 만들어 준다.
        */
        public void clearUsed()
        {
            GameObject[] useds = m_pool.getUseds();
            for (int i = 0; i < useds.Length; ++i)
            {
                push(useds[i], true);
            }

            m_pool.clearUsed();
        }

        public void destroyElapsedAccessTimeObject(float second)
        {
            if (m_isDontDestroy)
                return;
            if (m_pool.isEmpty())
                return;

            if (m_pool.isOverElapsedAccessTime(second))
            {
                destroyGameObject(m_pool.getUnUsedEnumerator());
                destroyGameObject(m_pool.getUsedEnumerator());
                m_pool.clear();
            }
        }

        private void destroyGameObject(List<GameObject>.Enumerator e)
        {
            while (e.MoveNext())
            {
                if (null != e.Current)
                {
                    GameObject.Destroy(e.Current);
                    m_instanceIds.Remove(e.Current.GetInstanceID());
                }
            }
        }
    }
}