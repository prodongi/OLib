using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OLib
{
    public abstract class Goal {
        private Goal m_next = null;
        private bool m_isBegin = false;

        public Goal next { get { return m_next; } set { m_next = value; } }
        public bool isBegin { get { return m_isBegin; } }

        public virtual void begin(int entityUdid, ref bool end)
        {
            end = false;
            m_isBegin = true;
        }

        public virtual void end(int entityUdid)
        {
        }

        public virtual void resume()
        {

        }

        public virtual void update(int entityUdid, ref bool end)
        {
            end = false;
        }

        public abstract int type { get; }
    }
}
