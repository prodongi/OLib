using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OLib
{
    public class AttributeVariable {

        private Dictionary<string, AttributeVariableType> m_variables = new Dictionary<string, AttributeVariableType>();

        public void finalize()
        {
            m_variables.Clear();
        }

        public bool exist(string variableName)
        {
            if (m_variables.ContainsKey(variableName))
                return true;

            return false;
        }

        private AttributeVariableType find(string variableName)
        {
            AttributeVariableType variable;
            if (m_variables.TryGetValue(variableName, out variable))
                return variable;

            return null;
        }

        public T get<T>(string variableName)
        {
            AttributeVariableType variable = find(variableName);
            if (null == variable)
                return default(T);

            return variable.getValue<T>();
        }

        public void set<T>(string variableName, T value)
        {
            AttributeVariableType variable = find(variableName);
            if (null == variable)
            {
                variable = add<T>(variableName);
                if (null == variable)
                    return;
            }

            variable.setValue<T>(value);
        }

        public bool equal<T>(string variableName, T value)
        {
            AttributeVariableType variable = find(variableName);
            if (null == variable)
                return false;

            return variable.equal<T>(value);
        }

        protected virtual AttributeVariableType add<T>(string variableName)
        {
            if (exist(variableName))
            {
                OLib.Console.warning(string.Format("Failed AttributeVariable add, type {0} is exist", typeof(T).Name));
                return null;
            }
            
            AttributeVariableType variable = null;
            switch (typeof(T).Name)
            {
                case "Int32":   variable = new AttributeVariableInt(); break;
                case "Single":   variable = new AttributeVariableFloat(); break;
                case "Boolean":    variable = new AttributeVariableBool(); break;
                default: OLib.Console.warning(string.Format("Failed AttributeVariable add, invalid type {0}", typeof(T).Name)); break;
            }

            if (null != variable)
                m_variables.Add(variableName, variable);

            return variable;
        }
    }
}