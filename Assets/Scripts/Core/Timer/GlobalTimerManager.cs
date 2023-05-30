using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SthGame
{
    public class GlobalTimerManager : ClientSystem
    {
        #region 单例

        public static GlobalTimerManager Instance { get; private set; }
        
        #endregion

        #region Timer Pool

        /// <summary>
        /// Timer池
        /// </summary>
        private ObjectPool<Timer> m_TimerPool;
        private ObjectPool<Timer> TimerPool
        {
            get
            {
                if (m_TimerPool == null)
                {
                    m_TimerPool = new ObjectPool<Timer>(null, OnTimerRelease);
                }
                return m_TimerPool;
            }
        }
        
        private void OnTimerRelease(Timer timer)
        {
            timer.OnRelease();
        }

        #endregion

        #region Timer

        public delegate void TimerDelegate();
        public class Timer
        {
            public TimerDelegate m_OnTimer;
        
            /// <summary>
            /// 唯一Id
            /// </summary>
            public uint m_TimerId;
        
            /// <summary>
            /// 是否循环
            /// </summary>
            public bool m_Loop;
                
            /// <summary>
            /// 计时间隔
            /// </summary>
            public float m_Interval;
        
            /// <summary>
            /// 上次触发时间
            /// </summary>
            internal float m_LastTriggerTime;
                
            /// <summary>
            /// 是否执行过
            /// </summary>
            public bool m_HasExecuted;
        
            /// <summary>
            /// 是否受暂停影响
            /// </summary>
            public bool m_CanBePaused;
        
            public Timer(uint inMTimerID)
            {
                m_TimerId = inMTimerID;
            }
        
            public Timer()
            {
            }
        
            public void Start()
            {
                m_LastTriggerTime = m_CanBePaused ? Time.time : Time.realtimeSinceStartup;
            }
        
            public override string ToString()
            {
                if (m_OnTimer == null)
                {
                    return string.Format("timer id:{0}, interval:{1}", m_TimerId, m_Interval);
                }
                else
                {
                    return string.Format("timer id:{0}, interval:{1}, callback:{2}-{3}", m_TimerId, m_Interval, m_OnTimer.Target, m_OnTimer.Method);
                }
            }
        
            public void OnRelease()
            {
                m_TimerId = 0;
                m_HasExecuted = false;
            }
        }

        #endregion
        
        #region 内部数据

        /// <summary>
        /// 递增计时器Id
        /// </summary>
        private static uint s_UniqueTimerId = 0u;
        
        /// <summary>
        /// 能否暂停
        /// </summary>
        private bool m_CanBePause;
        
        /// <summary>
        /// 计时器列表
        /// </summary>
        protected List<Timer> m_Timers = new List<Timer>();

        #endregion
        
        #region 基类实现

        public override void Init()
        {
            base.Init();
            
            Instance = this;
        }

        public override void ShutDown()
        {
            s_UniqueTimerId = 0u;
            _ClearAllTimers();
        }

        public override void Tick(float deltaTime)
        {
            UpdateTimers();
        }

        #endregion

        #region 计时器实现

        /// <summary>
        /// 帧刷新
        /// </summary>
        private void UpdateTimers()
        {
            float currentTime = m_CanBePause ? Time.time : Time.realtimeSinceStartup;

            for (int i = m_Timers.Count - 1; i >= 0; i--)
            {
                Timer timer = m_Timers[i];
                if (timer.m_HasExecuted && timer.m_Interval == 0)
                {
                    m_Timers.RemoveAt(i);
                    TimerPool.Release(timer);
                }
                else if (currentTime - timer.m_LastTriggerTime >= timer.m_Interval)
                {
                    if (!timer.m_Loop)
                    {
                        m_Timers.RemoveAt(i);
                        TimerPool.Release(timer);
                    }

                    timer.m_LastTriggerTime = currentTime;
                    if (null != timer.m_OnTimer)
                    {
                        timer.m_OnTimer();
                    }
                    timer.m_HasExecuted = true;
                }
            }
        }
        
        private uint _StartTimer(float interval, bool loop, TimerDelegate timerDelegate)
        {
            Timer timer = TimerPool.Get();
            m_Timers.Add(timer);

            s_UniqueTimerId++;
            timer.m_TimerId = s_UniqueTimerId;
            timer.m_Interval = interval;
            timer.m_OnTimer = timerDelegate;
            timer.m_Loop = loop;
            
            timer.Start();

            return timer.m_TimerId;
        }
        
        private void _ClearTimer(uint timerId, bool executeImmediately)
        {
            for (int i = 0; i < m_Timers.Count; i++)
            {
                if (m_Timers[i].m_TimerId == timerId)
                {
                    // 设置Interval为0f，在随后的UpdateTimers中销毁
                    m_Timers[i].m_Interval = 0;
                    m_Timers[i].m_HasExecuted = true;
                    break;
                }
            }
        }
        
        private void _ClearAllTimers()
        {
            m_Timers.Clear();
        }

        #endregion

        /// <summary>
        /// 开始一个计时器
        /// </summary>
        /// <param name="interval">间隔</param>
        /// <param name="loop">是否循环</param>
        /// <param name="timerDelegate">计时器回调</param>
        /// <returns></returns>
        public uint StartTimer(float interval, bool loop, TimerDelegate timerDelegate)
        {
            return _StartTimer(interval, loop, timerDelegate);
        }

        /// <summary>
        /// 根据timerId销毁计时器
        /// 通常用于循环计时器的销毁，非循环计时器执行后会自动销毁
        /// </summary>
        /// <param name="timerId">唯一的计时器id</param>
        /// <param name="executeImmediately">是否销毁前马上执行一次</param>
        public void ClearTimer(uint timerID, bool executeImmediately = false)
        {
            _ClearTimer(timerID, executeImmediately);
        }
        
        /// <summary>
        /// 清除所有计时器
        /// </summary>
        public void ClearAllTimers()
        {
            _ClearAllTimers();
        }
    }
}