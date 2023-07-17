using System.Collections.Generic;
using UnityEngine;
using System;
using System.Diagnostics;
using Sirenix.OdinInspector;
using Debug = UnityEngine.Debug;

namespace Next.BoyanFX
{
    [DisallowMultipleComponent, ExecuteInEditMode]
    public partial class NextBoyanFx : AbstractNextBoyanFx
    {
        private bool m_initDelayData;
        [NonSerialized]public bool manualUpdateMode = false;
#if UNITY_EDITOR
        [NonSerialized] public bool lockActive = false;
        //标记当前特效是原始版本还是发布版本，readonly是release版本
        [HideInInspector]public bool readOnly;
        //特效的原始版本
        public GameObject referenceFxSrc;
        
#endif

        [HideInInspector] public List<ModelShell> modelShells = new List<ModelShell>();
        [HideInInspector] public List<ParticleShell> psShells = new List<ParticleShell>();
        [HideInInspector] public List<NodeShell> nodeShells = new List<NodeShell>();
        protected Morefun.LockStep.EventHandler<float> _creatorTimeScaleChanged;
        protected Morefun.LockStep.EventHandler<bool> _creatorPaused;
        protected Morefun.LockStep.EventHandler<bool> _creatorVisibleChanged;

        #region ------ 控制参数 ------

        [HideInInspector] public bool looping;
        [HideInInspector] public bool autoStop;
        [HideInInspector] public float duration = 1.0f;
        [HideInInspector] public float fadeTime = 0.5f;
        
        [ShowInInspector] public float timeScale = 1.0f;
        [LabelText("淡出时脱离")]
        public bool ceaseStopFollow = true;

        private float m_delayStopFollow = -1f;

        [LabelText("自动播放")]
        public bool autoPlay = false;
        [NonSerialized] public int id;
        [NonSerialized] public string fxTag;
        [NonSerialized] public int groupId;
                
        [OnValueChanged("SortingOrderChanged")]
        public int sortingOrder;

        /// <summary>
        /// 被击方处于格挡时看到的特效
        /// </summary>
        [NonSerialized] public bool block;

        // 特效状态
        protected Status m_status;
        // 外部获取状态
        public Status CurrentStatus => m_status;
        public float stateTimer => m_stateTimer;
        public float processTimer => m_processTimer;

        // 创建该特效的 目标
        public IFxTarget Creator { get; protected set; }
        // 状态 随源
        public bool SyncWithCreator { get; protected set; }

        // 状态计时器
        protected float m_stateTimer;
        [ShowInInspector]
        private float m_speed = 1;

        // 过程计时器
        protected float m_processTimer;
        //强制贴地
        private bool m_forceGrounded;
        private float m_offsetY;

        // 默认缩放值
        private static readonly Vector3 s_defaultScale = new Vector3(1.0f, 1.0f, 1.0f);

        // 特效播放结束的回调
        private Action<NextBoyanFx> m_onStopCallback;

        private Transform m_cacheTrans;

        // 是否需要随创建者一起暂停
        protected bool m_needPauseWithCreator = false;
        //跳过渲染
        private bool m_skipRendering = false;
        
        protected BaseShell.FadeOutType m_fadeOutType;

        [HideInInspector]
        // 特效关联的animator，用来做时间缩放，不在子节点实现了，减少字段
        public Animator[] animators = null;
        #endregion

        void Awake()
        {
            Init();
            if (autoPlay)
            {
                Play();
            }
        }
        
        #region ------ 接口 ------

        // 初始化方法
        public virtual void Init()
        {
            _creatorTimeScaleChanged = OnCreatorTimeScaleChanged;
            _creatorPaused = OnCreatorPaused;
            _creatorVisibleChanged = OnCreatorVisibleChanged;
            
            
            m_cacheTrans = transform;
            if (this.m_status == Status.AWAIT) return;
            
            // 如初始化时状态不对，则先尝试反初始化一次
            if (this.m_status != Status.UNINIT)
            {
                this.Uninit();
            }

            // 初始化操作
            ShellUtils.InitShells(psShells);
            ShellUtils.InitShells(modelShells);
            ShellUtils.InitShells(nodeShells);

            this.m_status = Status.AWAIT;
            this.m_stateTimer = 0.0f;
            if (this.sortingOrder != 0)
            {
                SetSortingOrder(this.sortingOrder);    
            }
            //先隐藏，lod控制显示
            if (Application.isPlaying)
            {
                DeactiveAll();
            }
        }

        /// <summary>
        /// 设置 SortingOrder
        /// </summary>
        /// <param name="order"></param>
        public void SetSortingOrder(int order)
        {
            ShellUtils.ShellsSetSortingOrder(psShells,order);
            ShellUtils.ShellsSetSortingOrder(modelShells,order);
            ShellUtils.ShellsSetSortingOrder(nodeShells,order);
        }

        private void SortingOrderChanged()
        {
            SetSortingOrder(sortingOrder);
        }

        public void SetDelayToStopFollow(float time)
        {
            this.m_delayStopFollow = time;
        }
        

        private void DeactiveAll()
        {
            #if UNITY_EDITOR
            if (lockActive) return;
            #endif
            
            ShellUtils.SetShellNodeActive(psShells,false);
            ShellUtils.SetShellNodeActive(modelShells,false);
            ShellUtils.SetShellNodeActive(nodeShells,false);
        }

        // 反初始化方法
        public void Uninit()
        {
            if (this.m_status == Status.UNINIT) return;

            // 反初始化操作
            ShellUtils.UninitShells(psShells);
            ShellUtils.UninitShells(modelShells);
            ShellUtils.UninitShells(nodeShells);

            // 重置时不要重置位置
            // this.m_cacheTrans.localRotation = Quaternion.identity;
            // this.m_cacheTrans.localScale = s_defaultScale;
            this.m_status = Status.UNINIT;
            this.m_stateTimer = 0.0f;
            this.m_processTimer = 0.0f;

            // 清空回调
            this.m_onStopCallback = null;
        }
        
        // 播放方法
        protected void PlayInner()
        {
            if (this.m_status != Status.AWAIT) return;

            ShellUtils.ResetShells(this.modelShells);
            ShellUtils.ResetShells(this.nodeShells);
            ShellUtils.ResetShells(this.psShells);

            // 播放操作
            this.m_status = Status.PLAYING;
            this.m_stateTimer = 0.0f;
            this.m_processTimer = 0.0f;
        }

        // 停止方法，如果传入了callback != null表示要覆盖掉结束回调
        public virtual void Cease( BaseShell.FadeOutType fadeOutType = BaseShell.FadeOutType.NORMAL)
        {
            // 非Looping模式等同Stop()
            if (!this.looping)
            {
                this.Stop(true);
                return;
            }
            
            if (this.ceaseStopFollow)
            {
                this.m_cacheTrans.SetParent(null);
            }

            if (this.m_status != Status.PLAYING && m_status != Status.PAUSE)
            {
                return;
            }
            m_fadeOutType = fadeOutType;
            // 停止操作
            ShellUtils.CeaseShells(psShells);
            ShellUtils.CeaseShells(modelShells);
            ShellUtils.CeaseShells(nodeShells);

            this.m_status = Status.FADING;
            this.m_stateTimer = 0.0f;
        }

        // 更新方法
        private void Update()
        {
            if (manualUpdateMode) return;
            
            try
            {
                this.UpdateImpl(Time.deltaTime);
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message + e.StackTrace);
            }
        }

        public void SetForceGrounded(bool enable, float offsetY)
        {
            m_forceGrounded = enable;
            m_offsetY = offsetY;
        }

#if !CP_PROJECT
        private void LateUpdate()
        {
            if (m_forceGrounded && Creator != null)
            {
                var pos = m_cacheTrans.position;
                pos.y = Creator.GetTerrainY() + m_offsetY;
                m_cacheTrans.position = pos;
            }
        }
#endif

        public void Simulate(float deltaTime)
        {
            this.UpdateImpl(deltaTime);
        }

        public virtual  void UpdateImpl(float inputDeltaTime)
        {
            // 运行条件判定
            if (!this.IsPlaying()) return;
            // 更新状态 更新组件
            var deltaTime = inputDeltaTime * this.timeScale;

            // 非循环动画，最后一帧的更新要刚好把特效更新完
            if (!this.looping && this.m_stateTimer + deltaTime >= this.duration)
            {
                deltaTime = this.duration - this.m_stateTimer;
            }

            this.m_stateTimer += deltaTime;
            switch (this.m_status)
            {
                case Status.PLAYING:
                    this.m_processTimer += deltaTime;
                    this.UpdatePlayingShells();
                    break;
                case Status.FADING:
                    if (this.looping)
                    {
                        this.UpdateFadingShells();
                    }
                    break;
            }

            if (m_delayStopFollow > 0 && this.m_processTimer > m_delayStopFollow)
            {
                this.transform.SetParent(null);
                m_delayStopFollow = -1;
            }
        }

        // 更新子节点淡出
        private void UpdateFadingShells()
        {
            UpdateFadingShells(nodeShells);
            UpdateFadingShells(psShells);
            UpdateFadingShells(modelShells);

            if (this.m_stateTimer >= this.fadeTime)
            {
                this.Stop(true);
            }
        }
        
        private void UpdateFadingShells<T>(List<T> shells) where T : BaseShell
        {
            if (shells != null)
            {
                foreach (var shell in shells)
                {
                    UpdateFadingShellItem(shell);
                }
            }
        }

        private bool m_hasRename = false;
        private void OnDisable()
        {
            //TODO 待动作那边修复这个bug
            //parent deactive,重新激活的时候，如果存在重名，那么骨骼在建立的时候会有报错
            //这里给重新分配下名字
            if (this.gameObject.activeSelf && m_hasRename == false && Application.isPlaying && manualUpdateMode == false)
            {
                #if UNITY_EDITOR
                    //如果不是在角色身上，则跳过
                    if (Creator == null) return;
                #endif
                m_hasRename = true;
                this.gameObject.name = this.gameObject.name + this.gameObject.GetInstanceID();
                ShellUtils.RenameShellsName(this.nodeShells);
                ShellUtils.RenameShellsName(this.modelShells);
                ShellUtils.RenameShellsName(this.psShells);

            }
        }

        public T GetExtension<T>() where T:ExtensionBase
        {
            if (nodeShells.Count > 0)
            {
                foreach (var shell in nodeShells)
                {
                    if (shell.scriptFieldModule.MonoScript is T target)
                    {
                        return target;
                    }
                }
            }

            if (modelShells.Count > 0)
            {
                foreach (var shell in modelShells)
                {
                    if (shell.scriptFieldModule.MonoScript is T target)
                    {
                        return target;
                    }
                }
            }

            return null;
        }
        

        protected void UpdateFadingShellItem(BaseShell shell,float move_time = 0.0f)
        {
            if (shell.IsAwaiting && (shell.fadeOutPlay && (m_fadeOutType & shell.fadeOutType) != 0) 
                                && shell.lodLevel <= EffectLODManager.LodLevel 
                                && this.m_stateTimer >= shell.delay
                                && (!block || !shell.disableWhenBlock))
            {
                PlayShell(shell);
            }
            if (shell.IsPlaying)
            {
                shell.Update(this.m_stateTimer - move_time, this.m_processTimer - shell.delay - move_time,this.timeScale);
            }

            if (shell.fadeOutPlay)
            {
                if (this.m_stateTimer - move_time >= shell.duration)
                {
                    StopShell(shell);
                }
            }else
            { 
                if (this.m_stateTimer - move_time >= shell.fadeOutTime)
                {
                    StopShell(shell);
                }
                else
                {
                    var actProcess = m_processTimer - shell.delay -  move_time;
                    if (actProcess >= shell.duration && shell.endCut)
                    {
                        
                        StopShell(shell);
                    }
                }
                
            }
        }

        private void StopShells<T>(List<T> shells) where T : BaseShell
        {
            if (shells != null)
            {
                foreach (var shell in shells)
                {
                    StopShell(shell);
                }
            }
        }

        protected void UpdatePlayingShellItem(BaseShell shell)
        {
            if (shell.IsAwaiting && shell.fadeOutPlay == false 
                                && shell.lodLevel <= EffectLODManager.LodLevel 
                                && this.m_stateTimer >= shell.delay
                                && (!block || !shell.disableWhenBlock))
            {
                PlayShell(shell);
            }

            if (shell.IsPlaying)
            {
                float realStateTimer = this.m_stateTimer - shell.delay;
                shell.Update(realStateTimer, this.m_processTimer - shell.delay,this.timeScale);
                
                // 非循环空节点停止条件
                if (shell.endCut && realStateTimer >= shell.duration)
                {
                    shell.Cease();
                    // StopShell(shell);
                }
            }
        }
        
        private void UpdatePlayingShells<T>(List<T> shells) where T : BaseShell
        {
            if (shells != null)
            {
                foreach (var shell in shells)
                {
                    UpdatePlayingShellItem(shell);
                }
            }
        }

        // 子节点播放状态更新
        private void UpdatePlayingShells()
        {
            UpdatePlayingShells(nodeShells);
            UpdatePlayingShells(psShells);
            UpdatePlayingShells(modelShells);

            // 不循环或者，循环且勾选自动结束的特效，在这里停止
            if (!this.looping || this.autoStop)
            {
                if (this.m_stateTimer >= this.duration)
                {
                    this.Cease();
                }
            }
        }

        public void ActiveAllEnabled()
        {
            ShellUtils.ActiveEnabledNode(this.modelShells);
            ShellUtils.ActiveEnabledNode(this.nodeShells);
            ShellUtils.ActiveEnabledNode(this.psShells);
        }

        /// <summary>t
        /// 对于标准化 特效的 周期设置
        /// 特效本身 淡入淡出为1， 循环周期为1
        /// </summary>
        public void SetShellsDuration(float fadeInTime, float loopSpeed, float fadeOuTime)
        {
            this.fadeTime = fadeOuTime;
            ShellUtils.SetShellDuration(this.modelShells,fadeInTime,loopSpeed,fadeOuTime);
            ShellUtils.SetShellDuration(this.nodeShells,fadeInTime,loopSpeed,fadeOuTime);
            ShellUtils.SetShellDuration(this.psShells,fadeInTime,loopSpeed,fadeOuTime);
        }

        // 设置TimeScale方法
        protected void UpdateTimeScale()
        {
            this.timeScale = Creator == null?m_speed:Creator.GetTimeScale() * m_speed;

                //更新animator速度
            if (this.animators != null)
            {
                foreach (var animator in animators)
                {
                    if (animator)
                    {
                        animator.speed = this.timeScale;
                    }
                    else
                    {
                        Debug.LogWarning("Animator IsNull:" + gameObject.name);
                    }
                }
            }
        }

        private void OnEnable()
        {
            if (autoPlay)
            {
                if( IsPlaying())Stop(true);
                Play();
            }
        }

        protected void SetSkipRendering(bool value)
        {
            if (m_skipRendering != value)
            {
                m_skipRendering = value;
                ShellUtils.SetSkipRendering(this.psShells,value);
                ShellUtils.SetSkipRendering(this.modelShells,value);
            }
        }

        protected void PlayShell(BaseShell shell)
        {
            if (shell.disabled == false)
            {
                SetGameObjectActive(shell, true);
                shell.Play();
            }
        }

        protected void StopShell(BaseShell shell)
        {
            SetGameObjectActive(shell, false);
            shell.Stop();
        }

        protected void SetGameObjectActive(BaseShell shell, bool active)
        {
#if UNITY_EDITOR
            if (lockActive) return;
#endif
            if (shell.transform)
            {
                shell.transform.gameObject.SetActive(active);
            }
        }
        
        // 是否播放中
        public bool IsPlaying()
        {
            return this.m_status == Status.PLAYING || this.m_status == Status.FADING;
        }

        #endregion

        public virtual void Play(IFxTarget creator = null,bool syncWithCreator = false)
        {
            this.Creator = creator;
            SyncWithCreator = syncWithCreator;
            this.PlayInner();
 #if UNITY_EDITOR
            EditorPlayInner();
#endif

            if (creator != null )
            {
                creator.AddTimeScaleListener(_creatorTimeScaleChanged);
                if (m_needPauseWithCreator)
                    creator.AddPauseListener(_creatorPaused);
                if (syncWithCreator)
                {
                    SetSkipRendering(!creator.IsVisible());
                    creator.AddVisibleChangedListener(_creatorVisibleChanged);
                }
            }
            UpdateTimeScale();
            // UpdateImpl(0);
        }

        private void OnCreatorTimeScaleChanged(short eventId, float scale)
        {
            UpdateTimeScale();
        }

        private void OnCreatorPaused(short eventId, bool isPaused)
        {
            if (isPaused)
                timeScale = 0;
            else
                timeScale = this.Creator != null ? this.Creator.GetTimeScale() : 1;
        }

        private void OnCreatorVisibleChanged(short eventId,bool visible)
        {
            SetSkipRendering(!visible);
        }

        public void Stop(bool forceImmediate = false)
        {
            if (!this.IsPlaying() || this.m_status == Status.PAUSE) return;

            //非强制停止，走cease
            if (forceImmediate == false)
            {
                this.Cease();
                return;
            }

            // 终止操作
            StopShells(psShells);
            StopShells(modelShells);
            StopShells(nodeShells);

            this.m_status = Status.AWAIT;
            this.m_stateTimer = 0.0f;
            this.m_processTimer = 0.0f;
            this.m_delayStopFollow = -1f;
            
            if (this.m_skipRendering)
            {
                SetSkipRendering(false);
            }

            if (this.Creator != null)
            {
                this.Creator.RemoveTimeScaleListener(_creatorTimeScaleChanged);
                if (m_needPauseWithCreator)
                    this.Creator.RemovePauseListener(_creatorPaused);
                if (SyncWithCreator)
                {
                    this.Creator.RemoveVisibleChangedListener(_creatorVisibleChanged);
                }
            }

            if (this.m_onStopCallback != null)
            {
                // 先置空后调用
                var tmpCallback = this.m_onStopCallback;
                this.m_onStopCallback = null;
                tmpCallback(this);
            }
        }

        public void SetElapsedTime(float elapse)
        {
            //生命周期内，直接改时间
            m_processTimer += elapse;
            m_stateTimer += elapse;
        }

        public void SetSpeed(float speed)
        {
            m_speed = speed;
            UpdateTimeScale();
        }

        public virtual void Pause()
        {
            if (this.m_status == Status.PLAYING)
            {
                m_status = Status.PAUSE;
                foreach (var psShell in this.psShells)
                {
                    if( psShell.IsPlaying) psShell.Pause();
                }
            }
        }

        public virtual void Resume()
        {
            if (this.m_status == Status.PAUSE)
            {
                m_status = Status.PLAYING;
                foreach (var psShell in this.psShells)
                {
                    if (psShell.IsPlaying) psShell.Resume();
                }
            }
        }

        public void SetOnFinishAction(Action<NextBoyanFx> func)
        {
            this.m_onStopCallback = func;
        }

        public void SetNeedPausedWithCreator(bool needPause)
        {
            this.m_needPauseWithCreator = needPause;
        }
        
    }
}