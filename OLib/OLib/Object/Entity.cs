using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OLib
{
    public class Entity : MonoBehaviour {
        private int m_uuid = 0;

        public int uuid { get { return m_uuid; } }

        public virtual void initialize(int _uuid)
        {
            m_uuid = _uuid;
        }
    }
}