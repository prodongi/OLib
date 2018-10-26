using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OLib
{
    public class Timer {
        private float m_interval = 0.0f;
        private float m_remainingTime = 0.0f;

        public virtual void initialize(float interval)
        {
            m_interval = interval;
            m_remainingTime = interval;
        }

        public void reset()
        {
            m_remainingTime = m_interval;
        }

        public virtual bool countDown(float dt, bool loop)
        {
            m_remainingTime -= dt;

            bool end = false;
            if (0.0f >= m_remainingTime)
            {
                end = true;

                if (loop)
                {
                    m_remainingTime += m_interval;
                }
            }

            return end;
        }
    }
}
