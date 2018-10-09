using System;
using System.Collections.Generic;

namespace OLib
{
    public class Disposable : IDisposable
    {
        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {

        }
    }
}
