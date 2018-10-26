using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OLib
{
    public class AI : Disposable {
        private int m_entityUuid = 0;
        private Goal m_defaultGoal = null;
        /// <summary>
        /// m_newGoals에 추가했다가 맨 나중에 m_goals로 옮긴다.
        /// </summary>
        private LinkedList<Goal> m_goals = new LinkedList<Goal>();
        private List<Goal> m_buffers = new List<Goal>();

        public virtual void initialize(int entityUuid, Goal defaultGoal)
        {
            OLib.Console.assert(0 < entityUuid, "invalid entityUuid {0}", entityUuid);
            OLib.Console.assert(null != defaultGoal, "defaultGoal is null");

            m_entityUuid = entityUuid;
            m_defaultGoal = defaultGoal;
        }

        public void addGoal(Goal goal, params object[] args)
        {
            OLib.Console.assert(null != goal, "goal is null");

            goal.initialize(m_entityUuid, this, args);

            m_buffers.Add(goal);
        }
        
        public void update()
        {
            updateGoal();
            transferBuffers();
        }

        private void transferBuffers()
        {
            if (0 < m_buffers.Count)
            {
                foreach (Goal goal in m_buffers)
                {
                    m_goals.AddFirst(goal);
                }

                m_buffers.Clear();
            }
        }

        private void updateGoal()
        {
            if (0 == m_goals.Count)
            {
                if (null == m_defaultGoal)
                {
                    return;
                }
                else
                {
                    m_defaultGoal.initialize(m_entityUuid, this);
                    m_goals.AddFirst(m_defaultGoal);
                }
            }

            Goal goal = m_goals.First<Goal>();
            if (!goal.isBegin)
            {
                bool endBegin = false;
                goal.begin(ref endBegin);
                if (endBegin)
                {
                    popGoal(goal);
                    return;
                }
            }

            bool endUpdate = false;
            goal.update(ref endUpdate);
            if (endUpdate)
            {
                popGoal(goal);
            }
        }

        private void popGoal(Goal goal)
        {
            goal.end();
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
                    goal.end();
                }
                m_goals.Clear();
            }
        }
    }
}
