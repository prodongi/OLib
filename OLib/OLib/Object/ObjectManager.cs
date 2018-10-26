using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

namespace OLib
{
    public abstract class ObjectManager<T> : MonoSingleton<T> where T : MonoBehaviour {
        private struct DeleteInfo
        {
            public float destroyTime;
            public float elapsedTime;

            public DeleteInfo(float _destroyTime, float _elapsedTime = 0.0f)
            {
                destroyTime = _destroyTime;
                elapsedTime = _elapsedTime;
            }
        }

        private Dictionary<int, Entity> m_entities = new Dictionary<int, Entity>();
        /// <summary>
        /// delete는 LateUpdate()에서 한꺼번에 해준다.
        /// </summary>
        private Dictionary<int, DeleteInfo> m_deletes = new Dictionary<int, DeleteInfo>();
        private UuidCreator m_uuidCreator = new UuidCreator();

        protected abstract GameObject instiateObject(GameObject parent, string assetBundleName, string filename);

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                m_uuidCreator.Dispose();

                clear();
            }
        }

        protected virtual void clear()
        {
            foreach (KeyValuePair<int, Entity> pair in m_entities)
            {
                if (null == pair.Value)
                {
                    OLib.Console.warning("Warning EntityManager reset, pair.value is null");
                    continue;
                }
                pair.Value.Dispose();
            }
            m_entities.Clear();

            m_deletes.Clear();
        }

        public virtual E create<E>(EntityCreateData createData) where E : Entity, new()
        {
            OLib.Console.assert(null != createData, "createData is null");

            E t = null;

            try
            {
                GameObject obj = instiateObject(createData.parent, createData.assetBundleName, createData.fileName);
                if (null == obj)
                {
                    return null;
                }

                t = obj.GetComponent<E>();
                if (null == t)
                {
                    OLib.Console.warning(string.Format("Failed create entity, Not exist {0} component", createData.fileName));
                    return null;
                }

                int udid = m_uuidCreator.make();
                if (m_entities.ContainsKey(udid))
                {
                    OLib.Console.warning(string.Format("Failed create entity, entity id {0} is already exists", udid));
                    return null;
                }

                m_entities.Add(udid, t);
                t.initialize(udid, createData);
            }
            catch (Exception e)
            {
                OLib.Console.exception(e);

                if (null != t)
                {
                    t.Dispose();
                    t = null;
                }
            }

            return t;
        }

        public E get<E>(int id) where E : Entity
        {
            if (0 == id)
                return default(E);

            Entity entity = null;
            if (!m_entities.TryGetValue(id, out entity))
                return default(E);

            return entity as E;
        }

        public void del(int udid, float destroyTime = 0.0f)
        {
            if (m_deletes.ContainsKey(udid))
            {
                OLib.Console.warning(string.Format("Failed add entity delete list, exist delete id {0}", udid));
            }
            else
            {
                DeleteInfo deleteInfo = new DeleteInfo(destroyTime);
                m_deletes.Add(udid, deleteInfo);
            }
        }

        public List<int> getKeys()
        {
            List<int> keys = new List<int>(m_entities.Keys);
            return keys;
        }

        public Dictionary<int, Entity>.Enumerator getEnumerator()
        {
            return m_entities.GetEnumerator();
        }

        void LateUpdate()
        {
            deleteIds();
        }

        private void deleteIds()
        {
            if (0 < m_deletes.Count)
            {
                List<int> ids = (from pair in m_deletes
                                 select pair.Key).ToList();

                foreach (int udid in ids)
                {
                    DeleteInfo deleteInfo = m_deletes[udid];
                    /**
                     *  check destroy time
                     * */
                    if (0.0f < deleteInfo.destroyTime)
                    {
                        deleteInfo.elapsedTime += Time.deltaTime;
                        if (deleteInfo.elapsedTime < deleteInfo.destroyTime)
                        {
                            m_deletes[udid] = deleteInfo;
                            continue;
                        }
                    }

                    m_deletes.Remove(udid);

                    Entity entity = get<Entity>(udid);
                    if (null == entity)
                    {
                        OLib.Console.warning(string.Format("Failed delete entity ids {0}", udid));
                        continue;
                    }

                    try
                    {
                        entity.Dispose();
                    }
                    catch (Exception e)
                    {
                        OLib.Console.exception(e);
                    }
                    finally
                    {
                        if (!m_entities.Remove(udid))
                        {
                            OLib.Console.warning(string.Format("Failed remove entity id {0}", udid));
                        }
                    }
                }
            }
        }
    }
}