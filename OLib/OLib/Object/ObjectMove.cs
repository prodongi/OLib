using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace OLib
{
    public class ObjectMove
    {
        private Coroutine m_coMove = null;
        private Coroutine m_coForward = null;
        private MonoBehaviour m_mono = null;
        private bool m_isArrive = false;

        public bool isArrive { get { return m_isArrive; } }

        public void start(MonoBehaviour mono, float moveV, Vector3 destPosition, Action arriveCallback)
        {
            OLib.Console.assert(null != mono, "mono is null");

            m_mono = mono;
            m_isArrive = false;

            CoroutineHelper.start(mono, coMove(moveV, destPosition, arriveCallback), ref m_coMove);
        }

        public void stop()
        {
            if (null == m_mono)
                return;

            CoroutineHelper.stop(m_mono, ref m_coMove);
            CoroutineHelper.stop(m_mono, ref m_coForward);
        }

        IEnumerator coMove(float moveV, Vector3 destPosition, Action arriveCallback)
        {
            GameObject gameObject = m_mono.gameObject;

            Vector3 curPosition = gameObject.transform.position;
            Vector3 dir = destPosition - curPosition;
            float moveLen = dir.magnitude;
            dir.Normalize();

            float moveTime = moveLen / moveV;
            forward(dir, moveTime);

            float elapsedLen = 0.0f;
            while (elapsedLen < moveLen)
            {
                float s = moveV * Time.deltaTime;
                if (elapsedLen + s > moveLen)
                {
                    s = moveLen - elapsedLen;
                }

                elapsedLen += s;
                curPosition += dir * s;

                gameObject.transform.position = curPosition;

                yield return null;
            }

            m_coMove = null;
            m_isArrive = true;

            if (null != arriveCallback)
                arriveCallback();
        }

        private void forward(Vector3 destDir, float moveTime)
        {
            CoroutineHelper.start(m_mono, coForward(destDir, moveTime), ref m_coForward);
        }

        IEnumerator coForward(Vector3 destDir, float moveTime)
        {
            GameObject gameObject = m_mono.gameObject;
            Vector3 forward = gameObject.transform.forward;

            float dot = Vector3.Dot(forward, destDir);
            dot += 1.0f;    // 0 ~ 2
            dot /= 2.0f;    // 0 ~ 1
            dot = 1.0f - dot;   // 1 ~ 0

            float maxRotationTime = 1.0f;
            float maxRotationLen = 1.0f;

            float rotationLen = dot;
            float rotationTime = rotationLen * (maxRotationTime / maxRotationLen);

            if (rotationTime > moveTime)
            {
                rotationTime = moveTime;
            }

            float elapsedTime = 0.0f;
            while (elapsedTime < rotationTime)
            {
                elapsedTime += Time.deltaTime;
                elapsedTime = Mathf.Min(rotationTime, elapsedTime);
                float t = elapsedTime / rotationTime;

                //Debug.LogFormat("t {0}, v {1}, moveTime {2}, roatationTime {3}", t, v, moveTime, rotationTime);

                Vector3 r = Vector3.Slerp(forward, destDir, t);
                gameObject.transform.forward = r;

                yield return null;
            }

            gameObject.transform.forward = destDir;
            m_coForward = null;
        }
    }
}