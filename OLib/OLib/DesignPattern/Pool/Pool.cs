using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace OLib
{
    public class Pool<T> where T : class {
        /// @brief 아직 사용 안된 리스트
        private List<T> m_unUsedList = new List<T>();
        /// @brief 사용 된 리스트
        private List<T> m_usedList = new List<T>();
        private System.DateTime m_lastAccessTime = new System.DateTime();

        public int unusedCount { get { return m_unUsedList.Count; } }
        public int usedCount { get { return m_usedList.Count; } }

        public void addUnUsed(T t)
        {
            m_unUsedList.Add(t);
        }

        /**
        *   @brief 사용 안된걸 팝해서 사용된 리스트에 넣어 준다.
        */
        public T pop()
        {
            if (0 == m_unUsedList.Count)
            {
                return null;
            }

            T t = m_unUsedList[0];
            m_unUsedList.RemoveAt(0);

            m_usedList.Add(t);

            m_lastAccessTime = System.DateTime.Now;

            return t;
        }

        /**
        *   @brief t를 사용 안된 리스트에 넣어 준다.
        */
        public void push(T t, bool isRemoveUsed = true)
        {
            m_unUsedList.Add(t);

            if (isRemoveUsed)
            {
                int index = m_usedList.IndexOf(t);
                if (-1 != index)
                    m_usedList.RemoveAt(index);
            }
        }

        public bool isEmptyUnUsed()
        {
            return (0 == m_unUsedList.Count) ? true : false;
        }

        public bool isEmpty()
        {
            if (0 == m_unUsedList.Count && 0 == m_usedList.Count)
                return true;
            return false;
        }

        public List<T>.Enumerator getUsedEnumerator()
        {
            return m_usedList.GetEnumerator();
        }

        public List<T>.Enumerator getUnUsedEnumerator()
        {
            return m_unUsedList.GetEnumerator();
        }

        public T[] getUseds()
        {
            return m_usedList.ToArray();
        }

        public void clearUsed()
        {
            m_usedList.Clear();
        }

        public void clear()
        {
            m_unUsedList.Clear();
            m_usedList.Clear();
        }

        public bool isOverElapsedAccessTime(float second)
        {
            System.TimeSpan timeSpan = System.DateTime.Now - m_lastAccessTime;
            if (timeSpan.TotalSeconds >= second)
                return true;

            return false;
        }
    }
}