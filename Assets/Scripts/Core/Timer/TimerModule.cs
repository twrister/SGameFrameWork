using UnityEngine;
using System.Collections.Generic;

namespace SthGame
{
    public class TimerModule
    {
        protected List<Timer> m_timers = new List<Timer>();

        private uint m_unUseTimerID = 0;

        private bool m_canBePaused = true;
        public bool CanBePaused
        {
            get { return m_canBePaused; }
            set { m_canBePaused = value; }
        }

        public Timer SetTimer(float inInterval, bool bLoop, TimerDelegate inTimerDelegate, bool inAutoStart = true)
        {
            // Search for an existing timer first
            Timer timer = null;
            bool foundExist = false;
            for (int i = 0; i < m_timers.Count; i++)
            {
                if (m_timers[i].onTimer == inTimerDelegate)
                {
                    m_timers[i].Interval = inInterval;
                    m_timers[i].onTimer = inTimerDelegate;
                    m_timers[i].AutoStart = inAutoStart;
                    m_timers[i].Loop = bLoop;

                    foundExist = true;
                    timer = m_timers[i];
                }
            }

            if (foundExist == false)
            {
                timer = new Timer(m_unUseTimerID++, this);
                timer.Interval = inInterval;
                timer.onTimer = inTimerDelegate;
                timer.AutoStart = inAutoStart;
                timer.Loop = bLoop;
                AddTimer(timer);
            }

            if (inAutoStart)
            {
                timer.Start();
            }

            return timer;
        }

        protected void AddTimer(Timer timer)
        {
            m_timers.Add(timer);
        }

        public void ClearTimer(uint timerID)
        {
            for (int i = 0; i < m_timers.Count; i++)
            {
                if (m_timers[i].TimerID == timerID)
                {
                    //set the Interval to 0.f and let updateTimers clear it
                    m_timers[i].Interval = 0;
                    break;
                }
            }
        }

        public void ClearTimer(Timer timer)
        {
            if (timer != null)
            {
                ClearTimer(timer.TimerID);
            }
        }

        public void ClearTimer(TimerDelegate inTimerDelegate)
        {
            for (int i = 0; i < m_timers.Count; i++)
            {
                if (m_timers[i].onTimer == inTimerDelegate)
                {
                    //set the Interval to 0.f and let updateTimers clear it
                    m_timers[i].Interval = 0;
                }
            }
        }

        public bool IsTimerActive(TimerDelegate inTimerDelegate)
        {
            for (int i = 0; i < m_timers.Count; i++)
            {
                if (m_timers[i].onTimer == inTimerDelegate && m_timers[i].Interval > 0f)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsTimerNullOrDisable(Timer timer)
        {
            return timer == null || timer.Interval <= 0;
        }

        public static bool IsTimerActive(Timer timer)
        {
            return timer != null && timer.Interval > 0;
        }

        public void ClearAllTimers()
        {
            for (int i = 0; i < m_timers.Count; i++)
            {
                //set the Interval to 0.f and let updateTimers clear it
                m_timers[i].Interval = 0;
            }
        }

        public void UpdateTimers()
        {
            float currentTime = this.CanBePaused ? Time.time : Time.realtimeSinceStartup;

            for (int i = m_timers.Count - 1; i >= 0; i--)
            {
                Timer timer = m_timers[i];
                if (timer.Interval == 0)
                {
                    m_timers.RemoveAt(i);
                }
                else if (timer.AutoStart && currentTime - timer.LastTriggerTime >= timer.Interval)
                {
                    if (!timer.Loop)
                    {
                        m_timers.RemoveAt(i);
                    }

                    timer.LastTriggerTime = currentTime;
                    if (null != timer.onTimer)
                    {
                        timer.onTimer();
                    }
                }
            }
        }
    }
}