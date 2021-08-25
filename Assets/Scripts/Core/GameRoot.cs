using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

namespace SthGame
{
    public class GameRoot : MonoBehaviour
    {
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
            systemList.Add(new UserInfoSystem()); 
            systemList.Add(new ChatSystem());

            StartCoroutine(StartGame());
        }

        private IEnumerator StartGame()
        {
            yield return null;

            Logger.Log("StartGame");

            //NetworkSystem.Instance.InitClientPeer();

            RedPointManager.Instance.SetRedPointNum(ERedPointType.RedPointDemoSub1, 1);
            RedPointManager.Instance.SetRedPointNum(ERedPointType.RedPointDemoSub2, 1);
            RedPointManager.Instance.SetRedPointNum(ERedPointType.RedPointDemoSub3, 1);
            GUIManager.Instance.Open<MainuiController>();
        }

        void Update()
        {
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
        public static int AverageFPS = 0;
        public static float AverageTime = 0.0f;

        void CalculateFPSTimings()
        {
            // A 3/4, 1/4 split gets close to a simple 10 frame moving average
            AverageTime = AverageTime * 0.75f + Time.deltaTime * 0.25f;
            // Calculate average framerate.
            AverageFPS = Mathf.CeilToInt(1.0f / AverageTime);
        }
        #endregion
    }
}
