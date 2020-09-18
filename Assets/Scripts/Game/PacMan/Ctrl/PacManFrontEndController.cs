using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CircularScrollView;
using LitJson;
using System;
using System.IO;

namespace SthGame
{
    public class PacManFrontEndController : UIBaseController
    {
        PacManFrontEndView view;
        PacManDataStore pacManDataStore;
        List<PacManGridController> gridControllerList;
        List<Generation> generationList;

        private int totalBean;
        private int totalStep;
        private int totalScore;

        protected override string GetResourcePath()
        {
            return "Prefabs/PacManFrontEnd";
        }

        public override void Init()
        {
            base.Init();
            view = UINode as PacManFrontEndView;
            pacManDataStore = DataStoreManager.Instance.FindOrBindDataStore<PacManDataStore>();

            view.refreshBtn.onClick.AddListener(OnClickRefresh);
            view.closeBtn.onClick.AddListener(OnClickClose);
            view.button2.onClick.AddListener(OnClickButton2);
            view.button3.onClick.AddListener(OnClickButton3);
            view.button4.onClick.AddListener(OnClickButton4);
            view.button5.onClick.AddListener(OnClickButton5);
            view.button6.onClick.AddListener(OnClickButton6);

            GlobalEventSystem.Instance.Bind(EventId.onClickKeyboardLeft, OnClickKeyboardLeft);
            GlobalEventSystem.Instance.Bind(EventId.onClickKeyboardRight, OnClickKeyboardRight);
            GlobalEventSystem.Instance.Bind(EventId.onClickKeyboardUp, OnClickKeyboardUp);
            GlobalEventSystem.Instance.Bind(EventId.onClickKeyboardDown, OnClickKeyboardDown);
            GlobalEventSystem.Instance.Bind(EventId.onPacManTryToEatBean, OnPacManTryToEatBean);

            gridControllerList = new List<PacManGridController>();

            InitGenerationLog();
            InitGrids();
            RefreshBeansShow();

            OnClickButton2();
        }

        public override void ShutDown()
        {
            base.ShutDown();
            GlobalEventSystem.Instance.UnBind(EventId.onClickKeyboardLeft, OnClickKeyboardLeft);
            GlobalEventSystem.Instance.UnBind(EventId.onClickKeyboardRight, OnClickKeyboardRight);
            GlobalEventSystem.Instance.UnBind(EventId.onClickKeyboardUp, OnClickKeyboardUp);
            GlobalEventSystem.Instance.UnBind(EventId.onClickKeyboardDown, OnClickKeyboardDown);
            GlobalEventSystem.Instance.UnBind(EventId.onPacManTryToEatBean, OnPacManTryToEatBean);
        }

        private void InitGrids()
        {
            gridControllerList = new List<PacManGridController>();
            for (int i = 0; i < 100; i++)
            {
                PacManGridController grid = CreateChildController<PacManGridController>(view.gridRootObj);
                gridControllerList.Add(grid);
            }
        }

        private void OnClickClose()
        {
            Close();
        }

        protected override void OpenCallBack()
        {

        }

        private const string generationPathDir = "Cache/PacMan";
        private string generationPathFile = string.Empty;
        private FileStream generationStream;
        private StreamWriter generationWriter;
        private StreamReader generationReader;

        //private string LAST_GENERATION_KEY = "LAST_GENERATION_KEY";

        private void InitGenerationLog()
        {
            generationPathFile = string.Format("{0}/GenerationMap.log", generationPathDir);

            string dir = Path.Combine(Logger.GetDataFolderPath(), generationPathDir);
            try
            {
                System.IO.Directory.CreateDirectory(dir);
                generationStream = new FileStream(generationPathFile, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                //generationStream.Position = generationStream.Length;
            }
            catch (Exception e)
            {
                Logger.Error(e.ToString());
            }

            generationWriter = new StreamWriter(generationStream);
            generationWriter.AutoFlush = true;

            generationReader = new StreamReader(generationStream);

            //String read = generationReader.ReadToEnd();
            //generationList = JsonMapper.ToObject<List<Generation>>(read);
            //Logger.Log("generationList.count = {0}", generationList.Count);
            //generationReader.Close();
        }

        private void AddNewGeneration(Generation gen)
        {
            if (generationList == null) return;

            generationList.Add(gen);
            try
            {
                if (generationWriter != null)
                {
                    string json = JsonMapper.ToJson(generationList);
                    generationWriter.Write(json);
                    Logger.Log("AddNewGeneration : {0}", json);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e.ToString());
            }
        }

        private void OnClickRefresh()
        {
            RefreshBeansShow();
        }

        private List<GridData> gridDatas = new List<GridData>();
        private void RefreshBeansShow()
        {
            ReleaseGridDatas();

            gridDatas = pacManDataStore.GetRandomGrids();
            totalBean = 0;
            totalStep = 0;
            totalScore = 0;
            isGameOver = false;
            UpdateView();

            //InitPlayer();
        }

        private void ReleaseGridDatas()
        {
            for (int i = 0; i < gridDatas.Count; i++)
                Pool<GridData>.Release(gridDatas[i]);
            gridDatas.Clear();
        }

        private void UpdateView()
        {
            for (int i = 0; i < gridControllerList.Count; i++)
            {
                gridControllerList[i].SetData(gridDatas[i]);
            }
            UpdateScoreDisplay();
        }

        private void InitPlayer()
        {
            view.player.InitFirstPos();
        }

        private void OnClickKeyboardLeft(object[] ps)
        {
            if (isGameOver) return;
            view.player.DoMove(Vector2Int.left);
        }

        private void OnClickKeyboardRight(object[] ps)
        {
            if (isGameOver) return;
            view.player.DoMove(Vector2Int.right);
        }

        private void OnClickKeyboardUp(object[] ps)
        {
            if (isGameOver) return;
            view.player.DoMove(Vector2Int.down);
        }

        private void OnClickKeyboardDown(object[] ps)
        {
            if (isGameOver) return;
            view.player.DoMove(Vector2Int.up);
        }

        bool isGameOver;
        private void OnPacManTryToEatBean(object[] ps)
        {
            Vector2Int pos = (Vector2Int)ps[0];
            int index = pos.x + pos.y * 10;


            if (gridControllerList[index].TryEatBean())
            {
                totalBean++;
                totalScore += 10;
            }
            else
            {
                totalScore--;
            }

            totalStep++;

            if (totalStep >= 200 || totalBean >= 50)
            {
                isGameOver = true;
            }

            UpdateScoreDisplay();
        }

        private void UpdateScoreDisplay()
        {
            view.scoreText.text = string.Format("分数:{0}", totalScore);
            view.stepText.text = string.Format("步数:{0}", totalStep);

            int resolutionIdx = view.player.GetResolutionIdx(gridDatas);

            view.descText.text = string.Format("index = {0}\n左：{1}\n右：{2}\n上：{3}\n下：{4}",
                resolutionIdx, resolutionIdx % 3, resolutionIdx / 3 % 3, resolutionIdx / 9 % 3, resolutionIdx / 27);
        }

        // 重置
        private bool isInitCalcGen = false;
        private void OnClickButton2()
        {

            if (isInitCalcGen) return;
            String read = generationReader.ReadToEnd();
            curCalcGen = JsonMapper.ToObject<Generation>(read);
            Logger.Log("firstGen = {0}", curCalcGen.ToString());
            generationReader.Close();
            isInitCalcGen = true;

            //GameRoot.Instance.StartCoroutine(AutoMoveByResolution());
        }

            // 开始计算
        Generation curCalcGen;
        private void OnClickButton3()
        {
            //String read = generationReader.ReadToEnd();
            //curCalcGen = JsonMapper.ToObject<Generation>(read);
            //Logger.Log("firstGen = {0}", curCalcGen.ToString());
            //generationReader.Close();

            if (curCalcGen == null) return;
            generationIndex = curCalcGen.Index;

            GameRoot.Instance.StartCoroutine(StartCalc());
        }

        // 生成1000个策略
        private void OnClickButton4()
        {
            GameRoot.Instance.StartCoroutine(CreateRandomGeneration());
        }

        int generationIndex = 0;
        int calcTimes = 0;
        private IEnumerator StartCalc()
        {
            List<Generation> grateGenerationList = new List<Generation>();
            
            while (generationIndex < 1000)
            {
                grateGenerationList.Clear();
                calcTimes = 0;
                while (calcTimes < 324)
                {
                    Generation gen = new Generation();
                    gen.Resolution = pacManDataStore.GetRandomModifyResolution(curCalcGen.Resolution, calcTimes);
                    gen.AvgScore = pacManDataStore.CalcResolutionAvgScore(gen.Resolution);
                    gen.Index = generationIndex;

                    if (grateGenerationList.Count == 0)
                    {
                        grateGenerationList.Add(gen);
                        grateGenerationList.Add(gen);
                    }
                    else if (gen.AvgScore > grateGenerationList[grateGenerationList.Count - 1].AvgScore)
                    {
                        grateGenerationList.Add(gen);
                    }
                    calcTimes++;

                    view.calcDescText.text = string.Format("代数：{0}\n运算次数：{1}\n当前最高分数：{2}",
                        generationIndex, calcTimes, curCalcGen.AvgScore);
                    //Logger.Log("代数：{0} \t运算次数：{1}", generationIndex, calcTimes);
                    yield return null;
                }
                //Logger.Log("grateGenerationList.count = {0}", grateGenerationList.Count);

                curCalcGen.Resolution = pacManDataStore.MergeTwoResolution(
                        grateGenerationList[grateGenerationList.Count - 1].Resolution, 
                        grateGenerationList[grateGenerationList.Count - 2].Resolution);
                curCalcGen.AvgScore = pacManDataStore.CalcResolutionAvgScore(curCalcGen.Resolution);
                curCalcGen.Index = generationIndex;

                generationIndex++;

                Logger.Log("第{0}代\t最高分：{1}\t{2}", curCalcGen.Index, curCalcGen.AvgScore, curCalcGen.ToString());
            }
            Logger.Log("all finish");
        }

        int createMaxTimes = 1000;
        int createTimes = 0;
        private IEnumerator CreateRandomGeneration()
        {
            Generation grateGeneration = new Generation() { AvgScore = -500 };
            while (createTimes < createMaxTimes)
            {
                Generation randomGen = CreateOneRandomGeneration();
                if (randomGen.AvgScore > grateGeneration.AvgScore)
                {
                    grateGeneration = randomGen;
                    //Logger.Log("New greateResolution : {0}", randomGen.ToString());
                }
                createTimes++;
                view.calcDescText.text = string.Format("运算次数：{0}\n最高分数：{1}", createTimes, grateGeneration.AvgScore);
                yield return null;
            }

            string json = JsonMapper.ToJson(grateGeneration);
            generationWriter.WriteLine(json);
            Logger.Log("第{0}代\t最高分：{1}\t{2}", grateGeneration.Index);
        }

        private Generation CreateOneRandomGeneration()
        {
            Generation gen = new Generation();
            gen.Resolution = pacManDataStore.GetRandomResolution();
            gen.AvgScore = pacManDataStore.CalcResolutionAvgScore(gen.Resolution);
            return gen;
        }

        // 手动移动一步
        private void OnClickButton5()
        {
            MoveByResolutionOnce();
        }
        
        private void MoveByResolutionOnce()
        {
            if (totalStep == 0) // 第一步随机
            {
                int randomIdx = UnityEngine.Random.Range(0, 100);
                Vector2Int playerPos = new Vector2Int(randomIdx % 10, randomIdx / 10);
                view.player.SetPos(playerPos);
            }
            else
            {
                int resolutionIdx = view.player.GetResolutionIdx(gridDatas);
                EResolution res = (EResolution)curCalcGen.Resolution[resolutionIdx];
                view.player.MoveByResolution(res);
            }

            totalStep++;
        }

        private void OnClickButton6()
        {
            GameRoot.Instance.StartCoroutine(StartAutoMove());
        }

        private IEnumerator StartAutoMove()
        {
            while (totalStep < 100)
            {
                MoveByResolutionOnce();
                yield return new WaitForSeconds(0.3f);
            }
        }
    }
}
