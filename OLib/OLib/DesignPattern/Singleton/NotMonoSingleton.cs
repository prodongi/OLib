using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OLib
{
    public class NotMonoSingleton<T> where T : class, new()
    {
        protected static T _instance;

        public static T instance
        {
            get
            {
                if (null == _instance)
                {
                    _instance = new T();
                }
                return _instance;
            }
        }
    }
}
