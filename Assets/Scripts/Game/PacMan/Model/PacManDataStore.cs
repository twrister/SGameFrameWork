using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Protocol;
using LitJson;

namespace SthGame
{
    public class PacManDataStore : DataStore
    {
        public PacManDataStore()
            : base(DataStoreType.PacMan)
        {
        }

        public List<GridData> GetRandomGrids()
        {
            List<int> numList = new List<int>();
            for (int i = 0; i < 100; i++)
            {
                numList.Add(i);
            }

            List<int> resultList = new List<int>();
            for (int i = 0; i < 50; i++)
            {
                int removeIdx = UnityEngine.Random.Range(0, numList.Count);
                //Logger.Log("{0}\tremoveIdx = {1}", i, removeIdx);
                resultList.Add(numList[removeIdx]);
                numList.RemoveAt(removeIdx);
            }

            List<GridData> gridDatas = new List<GridData>();
            for (int i = 0; i < 100; i++)
            {
                GridData data = Pool<GridData>.Get();
                data.Index = i;
                data.GridType = EGridType.None;
                gridDatas.Add(data);
            }

            for (int i = 0; i < resultList.Count; i++)
                gridDatas[resultList[i]].GridType = EGridType.Bean;

            return gridDatas;
        }

        public int[] GetRandomResolution()
        {
            int[] resolution = new int[81];
            for (int i = 0; i < resolution.Length; i++)
            {
                resolution[i] = UnityEngine.Random.Range(0, 5);
            }
            return resolution;
        }

        int calcTimes = 1000;
        public int CalcResolutionAvgScore(int[] resolution)
        {
            int totalScore = 0;
            for (int i = 0; i < calcTimes; i++)
            {
                List<GridData> randomMap = GetRandomGrids();
                totalScore += CalcResolutionScoreOnce(resolution, randomMap);
            }
            return totalScore / calcTimes;
        }

        int moveTimes = 100;
        public int CalcResolutionScoreOnce(int[] resolution, List<GridData> map)
        {
            int totalScore = 0;
            int totalBean = 0;
            Vector2Int playerPos = Vector2Int.zero;
            bool isWall = false;

            for (int i = 0; i < moveTimes; i++)
            {
                // 第一步默认左上角
                if (i == 0)
                {
                    int randomIdx = UnityEngine.Random.Range(0, 100);
                    playerPos = new Vector2Int(randomIdx % 10, randomIdx / 10);
                }
                else
                {
                    int resIdx = GetResolutionIdx(map, playerPos);
                    EResolution res = (EResolution)resolution[resIdx];
                    Vector2Int moveVec2 = Vector2Int.zero;

                    res = res == EResolution.Random ? (EResolution)UnityEngine.Random.Range(0, 4) : res;

                    switch (res)
                    {
                        case EResolution.Left:
                            moveVec2 = playerPos.x <= 0 ? Vector2Int.zero : Vector2Int.left;
                            break;
                        case EResolution.Right:
                            moveVec2 = playerPos.x >= 9 ? Vector2Int.zero : Vector2Int.right;
                            break;
                        case EResolution.Up:
                            moveVec2 = playerPos.y <= 9 ? Vector2Int.zero : Vector2Int.down;
                            break;
                        case EResolution.Down:
                            moveVec2 = playerPos.y >= 9 ? Vector2Int.zero : Vector2Int.up;
                            break;
                    }
                    isWall = moveVec2 == Vector2Int.zero;
                    playerPos += moveVec2;
                }

                // 吃豆 算分数
                int mapIdx = playerPos.x + playerPos.y * 10;
                if (isWall)
                {
                    totalScore -= 5;
                }
                else if (map[mapIdx].GridType == EGridType.Bean)
                {
                    totalBean++;
                    totalScore += 10;
                    map[mapIdx].GridType = EGridType.None;
                }
                else
                {
                    totalScore--;
                }

                // 豆吃完了 提前结束
                if (totalBean >= 50) break;
            }
            return totalScore;
        }

        private int GetResolutionIdx(List<GridData> gridDatas, Vector2Int curPos)
        {
            int left, right, up, down = 0;
            int curIdx = curPos.x + curPos.y * 10;

            int posX = curIdx % 10;
            int posY = curIdx / 10;

            if (posX <= 0)
            {
                left = 2;
                right = gridDatas[curIdx + 1].GridType == EGridType.Bean ? 1 : 0;
            }
            else if (posX >= 9)
            {
                left = gridDatas[curIdx - 1].GridType == EGridType.Bean ? 1 : 0;
                right = 2;
            }
            else
            {
                left = gridDatas[curIdx - 1].GridType == EGridType.Bean ? 1 : 0;
                right = gridDatas[curIdx + 1].GridType == EGridType.Bean ? 1 : 0;
            }

            if (posY <= 0)
            {
                up = 2;
                down = gridDatas[curIdx + 10].GridType == EGridType.Bean ? 1 : 0;
            }
            else if (posY >= 9)
            {
                up = gridDatas[curIdx - 10].GridType == EGridType.Bean ? 1 : 0;
                down = 2;
            }
            else
            {
                up = gridDatas[curIdx - 10].GridType == EGridType.Bean ? 1 : 0;
                down = gridDatas[curIdx + 10].GridType == EGridType.Bean ? 1 : 0;
            }

            int result = left + right * 3 + up * 9 + down * 27;

            return result;
        }

        public int[] MergeTwoResolution(int[] res1, int[] res2)
        {
            //Logger.Log("res1 {0} res2 {1}", res1.Length, res2.Length);
            int[] resolution = new int[81];
            int midIdx = UnityEngine.Random.Range(0, 81);
            for (int i = 0; i < resolution.Length; i++)
            {
                resolution[i] = i <= midIdx ? res1[i] : res2[i];
            }
            return resolution;
        }


        public int[] GetRandomModifyResolution(int[] resolution, int seed = -1)
        {
            int[] newRes = new int[resolution.Length];
            for (int i = 0; i < resolution.Length; i++)
            {
                if (seed >= 0)
                {
                    if (i == seed % 81)
                    {
                        newRes[seed % 81] = seed / 81;
                    }
                    else
                    {
                        newRes[i] = resolution[i];
                    }
                }
                else
                {
                    newRes[i] = resolution[i];
                }
                //if (UnityEngine.Random.Range(0, 5) == 2)
                //{
                //    newRes[i] = UnityEngine.Random.Range(0, 5);
                //}
                //else
                //{
                //    newRes[i] =  resolution[i];
                //}
            }
            return newRes;
        }
    }

    public enum EGridType
    {
        None,
        Bean,
        Wall,
    }

    public enum EResolution
    {
        Left,
        Right,
        Up,
        Down,
        Random,
    }

    [Serializable]
    public class GridData
    {
        private int index;
        public int Index { 
            get {
                return index;
            }
            set {
                index = value;
                pos.x = value % 10;
                pos.y = value / 10;
            }
        }

        private Vector2Int pos;
        public Vector2Int Pos { get { return pos; } }
        public EGridType GridType { get; set; }
    }

    public class Generation
    {
        public int[] Resolution;
        public int AvgScore;
        public int Index;

        public override string ToString()
        {
            return JsonMapper.ToJson(this);
        }
    }

    public class VirtualPlayer
    {
        private Vector2Int curPos;
        public void InitFirstPos()
        {
            curPos = Vector2Int.zero;
            //SetPos(curPos);

            
        }

        //private void SetPos(Vector2Int pos)
        //{
        //    float posX = pos.x * PacManGridView.GridSize - PacManGridView.GridSize * 4.5f;
        //    float posY = -pos.y * PacManGridView.GridSize + PacManGridView.GridSize * 4.5f;
        //    transform.localPosition = new Vector3(posX, posY);
        //    DoEatBean();
        //}
    }
}
