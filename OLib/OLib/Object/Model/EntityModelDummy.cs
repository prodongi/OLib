using System;
using System.Collections.Generic;
using UnityEngine;

namespace OLib
{
    [Serializable]
    public class EntityModelDummy {
        [SerializeField] List<GameObject> m_dummyList = new List<GameObject>();
        private Dictionary<string, GameObject> m_dummyDic = new Dictionary<string, GameObject>();

        public void initialize(GameObject parent)
        {
            listToDic();
        }

        private void listToDic()
        {
            m_dummyDic.Clear();

            foreach (GameObject obj in m_dummyList)
            {
                m_dummyDic.Add(obj.name, obj);
            }
        }

        public GameObject getDummy(string dummyName)
        {
            GameObject gameObject;
            if (m_dummyDic.TryGetValue(dummyName, out gameObject))
                return gameObject;

            return null;
        }
    }
}
