using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OLib.DesignPattern
{
    public class UuidCreator {
        private int m_uuid = 0;

        public int make()
        {
            return ++m_uuid;
        }

        public void initialize()
        {
            m_uuid = 0;
        }
    }
}
