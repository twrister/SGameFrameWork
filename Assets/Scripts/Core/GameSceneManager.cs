using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Protocol;
using LitJson;

namespace SthGame
{
    public class GameSceneManager : ClientSystem
    {
        private Dictionary<string, SceneInfo> sceneInfoDict = new Dictionary<string, SceneInfo>();
        private Dictionary<int, string> sceneIdNamePairs = new Dictionary<int, string>();
        private const string sceneConfigPath = "Config/SceneConfig";
        private BaseScene currentScene;

        //UserInfoDataStore userInfoDataStore;

        public static GameSceneManager Instance { get; private set; }

        public override void Init()
        {
            Instance = this;

            SceneManager.sceneLoaded += OnSceneLoaded;

            InitSceneConfig();

            NetworkSystem.Instance.RegisterNetworkEvent(Common.CSEnterSceneNtf, OnCSEnterSceneNtf);
            //userInfoDataStore = DataStoreManager.Instance.FindOrBindDataStore<UserInfoDataStore>();
        }

        public override void ShutDown()
        {
            currentScene = null;
            sceneInfoDict.Clear();

            SceneManager.sceneLoaded -= OnSceneLoaded;

            NetworkSystem.Instance.UnRegisterNetworkEvent(Common.CSEnterSceneNtf);
        }

        private void InitSceneConfig()
        {
            sceneInfoDict.Clear();
            SceneConfig sceneCfg = ConfigManager.LoadConfig<SceneConfig>(sceneConfigPath);
            for (int i = 0; i < sceneCfg.GameScenes.Length; i++)
            {
                sceneInfoDict.Add(sceneCfg.GameScenes[i].SceneName, sceneCfg.GameScenes[i]);
                sceneIdNamePairs.Add(sceneCfg.GameScenes[i].SceneId, sceneCfg.GameScenes[i].SceneName);
            }
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Logger.Log(string.Format("OnSceneLoaded, name = {0}", scene.name));

            if (sceneInfoDict.ContainsKey(scene.name))
            {
                currentScene = new BaseScene(sceneInfoDict[scene.name]);
            }

            if (currentScene != null)
            {
                currentScene.OnEnterScene();
            }
        }

        public BaseScene GetCurrentScene()
        {
            return currentScene;
        }

        public void LoadLoginScene()
        {
            LoadScene("Login");
        }

        public void LoadMainScene()
        {
            LoadScene("Main");
        }

        public void LoadScene(string sceneName)
        {
            if (string.IsNullOrEmpty(sceneName)) return;
            if (sceneInfoDict.ContainsKey(sceneName))
            {
                SceneManager.LoadScene(sceneName);
            }
        }

        private void LoadScene(int sceneId)
        {
            if (sceneIdNamePairs.ContainsKey(sceneId))
            {
                LoadScene(sceneIdNamePairs[sceneId]);
            }
        }

        private void OnCSEnterSceneNtf(string jsonStr)
        {
            CSEnterSceneNtf res = JsonMapper.ToObject<CSEnterSceneNtf>(jsonStr);
            if (res.IsFirstLogin)
            {
                GUIManager.Instance.CloseAllUI();
            }
            if (res.SceneId > 0)
            {
                LoadScene(res.SceneId);
            }
        }
    }
}
