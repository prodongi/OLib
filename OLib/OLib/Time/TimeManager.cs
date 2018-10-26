using System;
using System.Collections.Generic;
using UnityEngine;

namespace OLib
{
    public class TimeManager<T> : MonoSingleton<T> where T : MonoBehaviour {
        private float m_oldTimeScale = 1.0f;
        private bool m_pause = false;
        private float m_timeSpeed = 1.0f;

        public float timeSpeed { get { return m_timeSpeed; } set { m_timeSpeed = value; } }
        public float deltaTime { get { return Time.deltaTime * timeSpeed; } }
        public float smoothTime { get { return Time.smoothDeltaTime * timeSpeed; } }
        public float unscaledDeltaTime { get { return Time.unscaledDeltaTime; } }
        public bool pause
        {
            get
            {
                return m_pause;
            }

            set
            {
                if (m_pause != value)
                {
                    m_pause = value;
                    if (m_pause)
                    {
                        m_oldTimeScale = Time.timeScale;
                        Time.timeScale = 0.0f;
                    }
                    else
                    {
                        Time.timeScale = m_oldTimeScale;
                    }
                }
            }
        }
        
        public void togglePause()
        {
            pause = !pause;
        }
    }
}
