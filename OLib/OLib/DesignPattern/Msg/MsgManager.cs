using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OLib
{
    public class MsgManager<T> : MonoSingleton<T> where T : MonoBehaviour {
        /// <summary>
        /// queue가 처리되는 중에 push가 되면 안되기 때문에, 서로 구분해 놓는다.
        /// </summary>
        private List<Msg> m_executeQueues = new List<Msg>();
        private List<Msg> m_pushQueues = new List<Msg>();

        void Awake()
        {
            StartCoroutine(coExecute());
        }

        public void push(Msg msg)
        {
            m_pushQueues.Add(msg);
        }

        IEnumerator coExecute()
        {
            while (true)
            {
                yield return new WaitForEndOfFrame();

                if (0 < m_executeQueues.Count)
                {
                    foreach (Msg msg in m_executeQueues)
                    {
                        if (0.0f < msg.delay)
                        {
                            StartCoroutine(coDelayExecute(msg));
                        }
                        else
                        {
                            try
                            {
                                msg.execute();
                            }
                            catch (System.Exception e)
                            {
                                OLib.Console.exception(e);
                            }
                        }
                    }

                    m_executeQueues.Clear();
                }

                swapQueue();
            }
        }

        private void swapQueue()
        {
            if (0 < m_pushQueues.Count)
            {
                List<Msg> temp = m_executeQueues;
                m_executeQueues = m_pushQueues;
                m_pushQueues = temp;
            }
        }

        IEnumerator coDelayExecute(Msg msg)
        {
            yield return new WaitForSeconds(msg.delay);

            try
            {
                msg.execute();
            }
            catch (System.Exception e)
            {
                OLib.Console.exception(e);
            }
        }
    }
}
