using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OLib
{
    public class UuidCreator : Disposable {
        private int m_uuid = 0;

        public int make()
        {
            return ++m_uuid;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                m_uuid = 0;
            }
        }
    }
}
