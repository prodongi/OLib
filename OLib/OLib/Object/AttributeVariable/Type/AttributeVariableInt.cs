using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OLib
{
    public class AttributeVariableInt : AttributeVariableType {
        private int m_int;

        public override T getValue<T>()
        {
            return (T)(object)m_int;
        }

        public override void setValue<T>(T value)
        {
            m_int = (int)(object)value;
        }

        public override bool equal<T>(T value)
        {
            return m_int == (int)(object)value;
        }
    }
}