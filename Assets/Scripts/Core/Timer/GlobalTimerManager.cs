using UnityEngine;
using System.Collections;

namespace SthGame
{
    public class GlobalTimerManager : ClientSystem
    {
        TimerModule m_timerModule;
        public static GlobalTimerManager Instance { get; private set; }

        public override void Init()
        {
            Instance = this;

            m_timerModule = new TimerModule();
        }

        public override void ShutDown()
        {
            ClearAllTimers();
            m_timerModule = null;
        }

        public override void Tick(float deltaTime)
        {
            if (m_timerModule != null)
            {
                m_timerModule.UpdateTimers();
            }
        }

        public Timer StartTimer(float interval, bool loop, TimerDelegate timerDelegate, bool autoStart = true)
        {
            return m_timerModule == null ? null : m_timerModule.SetTimer(interval, loop, timerDelegate, autoStart);
        }

        public void ClearTimer(uint timerID)
        {
            if (m_timerModule != null)
            {
                m_timerModule.ClearTimer(timerID);
            }
        }

        public void ClearTimer(Timer timer)
        {
            if (m_timerModule != null)
            {
                m_timerModule.ClearTimer(timer);
            }
        }

        public void ClearTimer(TimerDelegate timerDelegate)
        {
            if (m_timerModule != null)
            {
                m_timerModule.ClearTimer(timerDelegate);
            }
        }

        private void ClearAllTimers()
        {
            Queue q = new Queue();
            if (m_timerModule != null)
            {
                m_timerModule.ClearAllTimers();
            }
        }
    }
}