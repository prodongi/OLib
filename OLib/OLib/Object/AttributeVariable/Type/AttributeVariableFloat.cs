using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OLib
{
    public class AttributeVariableFloat : AttributeVariableType {
        private float m_float;

        public override T getValue<T>()
        {
            return (T)(object)m_float;
        }

        public override void setValue<T>(T value)
        {
            m_float = (float)(object)value;
        }

        public override bool equal<T>(T value)
        {
            return Mathf.Approximately(m_float, (float)(object)value);
        }
    }
}