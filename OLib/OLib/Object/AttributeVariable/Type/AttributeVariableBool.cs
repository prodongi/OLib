using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OLib
{
    public class AttributeVariableBool : AttributeVariableType {
        private bool m_bool;

        public override T getValue<T>()
        {
            return (T)(object)m_bool;
        }

        public override void setValue<T>(T value)
        {
            m_bool = (bool)(object)value;
        }

        public override bool equal<T>(T value)
        {
            return m_bool == (bool)(object)value;
        }
    }
}