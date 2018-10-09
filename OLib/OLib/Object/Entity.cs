using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OLib
{
    public class Entity : ObjectBasePool {
        private int m_uuid = 0;

        public int uuid { get { return m_uuid; } }

        public virtual void initialize(int _uuid, EntityCreateData createData)
        {
            OLib.Console.assert(0 < _uuid, "invalid uuid {0}", _uuid);
                
            m_uuid = _uuid;
        }
    }
}