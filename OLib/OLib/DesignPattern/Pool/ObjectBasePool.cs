using UnityEngine;
using System.Collections;

namespace OLib
{
    public class ObjectBasePool : MonoBehaviour, System.IDisposable {
        public virtual void initPool()
        {

        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                //GameObjectPoolManager.instance.push(gameObject);
            }
        }
    }
}