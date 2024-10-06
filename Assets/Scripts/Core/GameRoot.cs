using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using System.Text;

namespace SthGame
{
    public class GameRoot : MonoBehaviour
    {
        public Camera m_UICamera;

        public bool showDebugInfo = true;
        public static GameRoot Instance { get; private set; }

        private List<ClientSystem> systemList = new List<ClientSystem>();

        static bool IsDoAwake = false;
        void Awake()
        {
            Instance = this;

            if (!IsDoAwake)
            {
                GameObject.DontDestroyOnLoad(this.gameObject);
            }
            IsDoAwake = true;

            guiStyle = new GUIStyle() { stretchWidth = true, stretchHeight = true, wordWrap = false, normal = new GUIStyleState() { textColor = Color.blue } };

            // 设置屏幕作为横屏的自动旋转
            Screen.orientation = ScreenOrientation.AutoRotation;
            Screen.autorotateToLandscapeLeft = true;
            Screen.autorotateToLandscapeRight = true;
            Screen.autorotateToPortrait = false;
            Screen.autorotateToPortraitUpsideDown = false;
        }

        void OnDestroy()
        {
            Instance = null;
            for (int i = systemList.Count - 1; i >= 0; i--)
            {
                systemList[i].ShutDown();
            }
            systemList.Clear();
        }

        void Start()
        {
            Logger.Instance.Init();

            systemList.Clear();
            systemList.Add(new GlobalEventSystem());
            systemList.Add(new GlobalTimerManager());
            systemList.Add(new NetworkSystem());
            systemList.Add(new DataStoreManager());
            systemList.Add(new GameSceneManager());
            systemList.Add(new GUIManager());
            systemList.Add(new RedPointManager());

            // Game Logic Module
            // systemList.Add(new UserInfoSystem()); 
            // systemList.Add(new ChatSystem());

            StartCoroutine(StartGame());
        }

        private IEnumerator StartGame()
        {
            yield return null;

            Logger.Log("StartGame");

            //NetworkSystem.Instance.InitClientPeer();
            OnStartGame();
        }

        protected virtual void OnStartGame()
        {
            RedPointManager.Instance.SetRedPointNum(ERedPointType.RedPointDemoSub1, 1);
            RedPointManager.Instance.SetRedPointNum(ERedPointType.RedPointDemoSub2, 1);
            RedPointManager.Instance.SetRedPointNum(ERedPointType.RedPointDemoSub3, 1);
            
            GUIManager.Instance.Open<MainuiController>();
        }

        void Update()
        {
            CalculateFPSTimings();

            for (int i = 0; i < systemList.Count; i++)
            {
                systemList[i].Tick(Time.deltaTime);
            }

            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                GlobalEventSystem.Instance.Fire(EventId.onClickKeyboardEnter);
            }

            if (Input.GetKeyDown(KeyCode.LeftArrow))
                GlobalEventSystem.Instance.Fire(EventId.onClickKeyboardLeft);
            if (Input.GetKeyDown(KeyCode.RightArrow))
                GlobalEventSystem.Instance.Fire(EventId.onClickKeyboardRight);
            if (Input.GetKeyDown(KeyCode.UpArrow))
                GlobalEventSystem.Instance.Fire(EventId.onClickKeyboardUp);
            if (Input.GetKeyDown(KeyCode.DownArrow))
                GlobalEventSystem.Instance.Fire(EventId.onClickKeyboardDown);

            UpdateDeviceInfo();
        }

        public void DoReLogin()
        {
            for (int i = systemList.Count - 1; i >= 0; i--)
            {
                systemList[i].ReLogin();
            }

            GameSceneManager.Instance.LoadLoginScene();
        }

        #region FPS
        StringBuilder deviceInfoSb = new StringBuilder();
        public static int AverageFPS = 0;
        public static float AverageTime = 0.0f;
        GUIStyle guiStyle;
        void CalculateFPSTimings()
        {
            // A 3/4, 1/4 split gets close to a simple 10 frame moving average
            AverageTime = AverageTime * 0.75f + Time.deltaTime * 0.25f;
            // Calculate average framerate.
            AverageFPS = Mathf.CeilToInt(1.0f / AverageTime);
        }

        void UpdateDeviceInfo()
        {
            deviceInfoSb.Clear();
            deviceInfoSb.AppendFormat("FPS: {0} ", AverageFPS);
        }

        public void OnGUI()
        {
            if (showDebugInfo)
            {
                GUI.Label(new Rect(110, 5, 400, 20), deviceInfoSb.ToString(), guiStyle);
            }   
        }

        void OnApplicationPause(bool pauseStatus)
        {
            string log = string.Format("OnApplicationPause - {0}", pauseStatus.ToString());
            if (GUIManager.Instance != null) GUIManager.Instance.ShowFloatTips(log);
            Logger.Log(log);
        }
        #endregion
    }
}
