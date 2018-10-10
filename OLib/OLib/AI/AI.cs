using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OLib
{
    public class AI : Disposable {
        private int m_entityUuid = 0;
        private LinkedList<Goal> m_goals = new LinkedList<Goal>();

        public virtual void initialize(int entityUuid)
        {
            OLib.Console.assert(0 < entityUuid, "invalid entityUuid {0}", entityUuid);

            m_entityUuid = entityUuid;
        }

        public void addGoal(Goal goal)
        {
            OLib.Console.assert(null != goal, "goal is null");

            m_goals.AddFirst(goal);
        }
        
        public void update()
        {
            updateGoal();
        }

        private void updateGoal()
        {
            if (0 == m_goals.Count)
                return;

            Goal goal = m_goals.First<Goal>();
            if (!goal.isBegin)
            {
                bool endBegin = false;
                goal.begin(m_entityUuid, ref endBegin);
                if (endBegin)
                {
                    popGoal(goal);
                    return;
                }
            }

            bool endUpdate = false;
            goal.update(m_entityUuid, ref endUpdate);
            if (endUpdate)
            {
                popGoal(goal);
            }
        }

        private void popGoal(Goal goal)
        {
            goal.end(m_entityUuid);
            m_goals.RemoveFirst();

            if (0 < m_goals.Count)
            {
                goal = m_goals.First<Goal>();
                if (goal.isBegin)
                    goal.resume();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (Goal goal in m_goals)
                {
                    goal.end(m_entityUuid);
                }
                m_goals.Clear();
            }
        }
    }
}
