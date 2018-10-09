using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OLib
{
    public class AttributeVariableType {

        public virtual T getValue<T>()
        {
            return default(T);
        }

        public virtual void setValue<T>(T value)
        {
        }

        public virtual bool equal<T>(T value)
        {
            return false;
        }
    }
}