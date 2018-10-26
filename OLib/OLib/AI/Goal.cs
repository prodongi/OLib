using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OLib
{
    public abstract class Goal {
        private Goal m_next = null;
        private bool m_isBegin = false;
        private int m_entityUuid = 0;
        private AI m_ai = null;

        public Goal next { get { return m_next; } set { m_next = value; } }
        public bool isBegin { get { return m_isBegin; } }
        protected int entityUuid { get { return m_entityUuid; } }

        public abstract int type { get; }

        public virtual void initialize(int entityUuid, AI ai, params object[] args)
        {
            OLib.Console.assert(0 < entityUuid, "invalid entity uuid {0}", entityUuid);
            OLib.Console.assert(null != ai, "ai is null");

            m_entityUuid = entityUuid;
            m_ai = ai;
            m_isBegin = false;
        }

        public virtual void begin(ref bool end, params object[] args)
        {
            end = false;
            m_isBegin = true;
        }

        public virtual void end()
        {
        }

        public virtual void resume()
        {

        }

        public virtual void update(ref bool end)
        {
            end = false;
        }

        protected void addGoal(Goal goal, params object[] args)
        {
            OLib.Console.assert(null != goal, "goal is null");

            m_ai.addGoal(goal, args);
        }
    }
}
