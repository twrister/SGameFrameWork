using UnityEngine;

namespace SthGame
{
    public delegate void TimerDelegate();
    public class Timer
    {
        private TimerModule m_ownerModule;

        public TimerDelegate onTimer;

        public float Interval { get; set; }
        public uint TimerID { get; set; }
        protected internal float LastTriggerTime { get; set; }
        protected internal bool AutoStart { get; set; }
        public bool Loop { get; set; }

        public Timer(uint inTimerID, TimerModule ownerModule)
        {
            TimerID = inTimerID;
            this.m_ownerModule = ownerModule;
        }

        public void Start()
        {
            AutoStart = true;
            LastTriggerTime = m_ownerModule.CanBePaused ? Time.time : Time.realtimeSinceStartup;
        }

        public void Stop()
        {
            AutoStart = false;
        }

        public override string ToString()
        {
            if (onTimer == null)
            {
                return string.Format("timer id:{0}, interval:{1}", TimerID, Interval);
            }
            else
            {
                return string.Format("timer id:{0}, interval:{1}, callback:{2}-{3}", TimerID, Interval, onTimer.Target, onTimer.Method);
            }
        }
    }
}