using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OLib
{
    public abstract class Msg {
        private float m_delay = 0.0f;

        public float delay { get { return m_delay; } }
        public abstract void execute();

        public Msg(float _delay)
        {
            m_delay = _delay;
        }
    }
}
