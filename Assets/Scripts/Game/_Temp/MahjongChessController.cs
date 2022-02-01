using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SthGame
{
    public class MahjongChessController : UIBaseController
    {
        public const float ITEM_WIDTH = 90f;
        public const float ITEM_HEIGHT = 126f;
        public const int MAHJONGTYPE_COUNT = 34;
        public const int MAHJONG_TOTAL_COUNT = 34 * 4;
        public static int SelectIndexOne = -1;

        MahjongChessView m_View;
        List<MahjongChessItem> m_ItemList;
        int[] m_OriMahjongArray = new int[MAHJONG_TOTAL_COUNT];
        int[] m_MahjongArray = new int[MAHJONG_TOTAL_COUNT];      // 存放当前的麻将ID
        float m_MahjongScale = 0.6f;

        protected override string GetResourcePath()
        {
            return "Prefabs/MahjongChess";
        }

        public override void Init()
        {
            base.Init();
            m_View = UINode as MahjongChessView;

            m_View.RegistLateUpdate(LateUpdateCallback);
            m_View.m_CloseBtn.onClick.AddListener(OnClickClose);
            m_View.m_RandomBtn.onClick.AddListener(OnClickRandomBtn);
            m_View.m_ResetBtn.onClick.AddListener(OnClickResetBtn);
            m_View.m_RoolbackBtn.onClick.AddListener(OnClickRoolbackBtn);
            m_View.m_TipsBtn.onClick.AddListener(OnClickTipsBtn);
            m_View.m_ViolenceTestBtn.onClick.AddListener(OnClickViolenceTestBtn);

            GlobalEventSystem.Instance.Bind(EventId.OnMahjongPointerDown, OnMahjongPointerDown);
            GlobalEventSystem.Instance.Bind(EventId.OnMahjongPointerUp, OnMahjongPointerUp);
        }

        public override void ShutDown()
        {
            base.ShutDown();

            GlobalEventSystem.Instance.UnBind(EventId.OnMahjongPointerDown, OnMahjongPointerDown);
            GlobalEventSystem.Instance.UnBind(EventId.OnMahjongPointerUp, OnMahjongPointerUp);

            m_View.m_ItemPrefab.DestroyPooled();

            SelectIndexOne = -1;
        }

        void InitMahjongs()
        {
            m_View.m_ItemPrefab.CreatePool();
            m_View.m_ItemPrefab.gameObject.SetActive(true);
            m_View.m_ItemsRoot.localScale = Vector3.one * m_MahjongScale;

            m_ItemList = new List<MahjongChessItem>(MAHJONG_TOTAL_COUNT);
            for (int i = 0; i < MAHJONG_TOTAL_COUNT; i++)
            {
                m_MahjongArray[i] = i;
                MahjongChessItem item = m_View.m_ItemPrefab.Spawn(m_View.m_ItemsRoot);
                item.name = i.ToString();
                item.InitPos(i);
                item.transform.localScale = Vector3.one;
                m_ItemList.Add(item);
            }

            CopyMahjongList(ref m_MahjongArray, ref m_OriMahjongArray);

            m_View.m_ItemPrefab.gameObject.SetActive(false);

            UpdateShow();
        }

        void UpdateShow(bool resetPos = false)
        {
            for (int i = 0; i < m_MahjongArray.Length; i++)
            {
                m_ItemList[i].SetData(m_MahjongArray[i], resetPos);
            }
        }

        protected override void OpenCallBack()
        {
            base.OpenCallBack();

            InitMahjongs();
        }

        void OnClickClose()
        {
            Close();
        }

        void OnClickRandomBtn()
        {
            for (int i = 0; i < MAHJONG_TOTAL_COUNT; i++)
            {
                m_MahjongArray[i] = i;
            }

            for (int i = 0; i < m_MahjongArray.Length - 1; i++)
            {
                int randIdx = Random.Range(i + 1, m_MahjongArray.Length);
                int temp = m_MahjongArray[randIdx];
                m_MahjongArray[randIdx] = m_MahjongArray[i];
                m_MahjongArray[i] = temp;
            }

            CopyMahjongList(ref m_MahjongArray, ref m_OriMahjongArray);

            SelectIndexOne = -1;

            UpdateShow(true);
        }

        void OnClickResetBtn()
        {
            CopyMahjongList(ref m_OriMahjongArray, ref m_MahjongArray);

            SelectIndexOne = -1;
            UpdateShow(true);
        }

        void OnClickRoolbackBtn()
        {
            
        }

        IEnumerator ViolenceTest()
        {
            int times = 0;
            while (times < 10000)
            {
                OnClickRandomBtn();
                //Logger.Log("OnClickRandomBtn");

                while (FindRemovableMahjong(m_MahjongArray, out int resultGridA, out int resultGridB))
                {
                    //Logger.Log($"find! resultGridA = {resultGridA}, resultGridB = {resultGridB}");
                    yield return null;
                    //Logger.Log($"OnClickMahjongItem = {m_MahjongArray[resultGridA].ToString()}");
                    OnClickMahjongItem(m_MahjongArray[resultGridA]);
                    yield return null;
                    //Logger.Log($"OnClickMahjongItem = {m_MahjongArray[resultGridB].ToString()}");
                    OnClickMahjongItem(m_MahjongArray[resultGridB]);
                }

                yield return null;

                times++;
                Logger.Log($"times = {times.ToString()}"); 
            }

            yield return null;
        }

        int[] m_TempArray = new int[MAHJONG_TOTAL_COUNT];

        void OnClickTipsBtn()
        {
            if (FindRemovableMahjong(m_MahjongArray, out int resultGridA, out int resultGridB))
            {
                ShowTips(m_MahjongArray[resultGridA], m_MahjongArray[resultGridB]);
            }
            else
            {
                //for (int i = 0; i < m_MahjongArray.Length; i++)
                //{
                //    if (m_MahjongArray[i] == -1)
                //    {
                //        CopyMahjongList(ref m_MahjongArray, ref m_TempArray);

                //        if (GetNearMahjangGridIdx(m_TempArray, -1, i, out int targetGridIdx, out int offset))
                //        { 
                //        }

                //        FindRemovableMahjong(m_TempArray, out int farResultGridA, out int farResultGridB);

                //    }
                //}
            }
        }

        // offset : gridIdx 相对 target的位置
        bool GetNearMahjangGridIdx(int[] mahjongArray, int unitOffset, int gridIdx, out int targetGridIdx, out int offset)
        {
            targetGridIdx = 0;
            offset = 0;

            //int row = gridIdx / 17;
            //int column = gridIdx % 17;

            if (Mathf.Abs(unitOffset) == 1) // hor
            {

            }

            return false;
        }

        void OnClickViolenceTestBtn()
        {
            GameRoot.Instance.StartCoroutine(ViolenceTest());
        }

        void OnClickMahjongItem(int mahjongId)
        {
            // 点高亮的取消选择
            if (mahjongId == SelectIndexOne)
            {
                SelectIndexOne = -1;
            }
            else
            {
                if (SelectIndexOne == -1)
                {
                    SelectIndexOne = mahjongId;
                }
                else
                {
                    bool isCanRemove = IsCanRemove(SelectIndexOne, mahjongId);
                    //Logger.Log($"isCanRemove = {isCanRemove}");
                    if (isCanRemove)
                    {
                        DisableMahjongGrid(SelectIndexOne);
                        DisableMahjongGrid(mahjongId);
                        SelectIndexOne = -1;
                    }
                    else
                    {
                        SelectIndexOne = mahjongId;
                    }
                }
            }

            UpdateShow();
        }

        int GetGridIdxByMahjongId(int mahjongId)
        {
            for(int i = 0; i < m_MahjongArray.Length; i++)
            {
                if (m_MahjongArray[i] == mahjongId)
                {
                    return i;
                }
            }
            return -1;
        }

        void DisableMahjongGrid(int mahjongId)
        {
            int gridId = GetGridIdxByMahjongId(mahjongId);
            if (gridId != -1) m_MahjongArray[gridId] = -1;
        }

        int GetMahjongGridIdx(int mahjongId)
        {
            for (int i = 0; i < m_MahjongArray.Length; i++)
            {
                if (m_MahjongArray[i] == mahjongId)
                {
                    return i;
                }
            }
            return -1;
        }

        // 相同的牌 并且 相邻或可对望 可以消除
        bool IsCanRemove(int mahjongA, int mahjongB)
        {
            if (!IsSameType(mahjongA, mahjongB)) return false;

            int gridIdxA = GetGridIdxByMahjongId(mahjongA);
            int gridIdxB = GetGridIdxByMahjongId(mahjongB);
            int rowA = gridIdxA / 17;
            int columnA = gridIdxA % 17;
            int rowB = gridIdxB / 17;
            int columnB = gridIdxB % 17;

            if (rowA != rowB && columnA != columnB) return false;

            bool isHorizontal = rowA == rowB;
            int offset = isHorizontal ? (gridIdxA < gridIdxB ? 1 : -1) : (gridIdxA < gridIdxB ? 17 : -17);
            int middleMahjongCount = 0;
            int tempMahjongId = Mathf.Clamp(gridIdxA + offset, 0, MAHJONG_TOTAL_COUNT - 1);
            while (tempMahjongId != gridIdxB)
            {
                if (m_MahjongArray[tempMahjongId] >= 0) middleMahjongCount++;
                tempMahjongId = Mathf.Clamp(tempMahjongId + offset, 0, MAHJONG_TOTAL_COUNT - 1);
            }

            return middleMahjongCount == 0;
        }

        #region Utility

        public static bool IsSameType(int mahjongA, int mahjongB)
        {
            return mahjongA / 4 == mahjongB / 4;
        }

        public static float GetMahjongGridPosX(int column)
        {
            return ((float)column - 8f) * ITEM_WIDTH;
        }

        public static float GetMahjongGridPosY(int row)
        {
            return ((float)row - 3.5f) * ITEM_HEIGHT;
        }

        public static Vector3 GetMahjongGridPos(int row, int column)
        {
            return new Vector3(GetMahjongGridPosX(column), GetMahjongGridPosY(row));
        }

        public static Vector3 GetMahjongGridPos(int gridIdx)
        {
            int row = gridIdx / 17;
            int column = gridIdx % 17;
            return GetMahjongGridPos(row, column);
        }

        void CopyMahjongList(ref int[] copyFromArray, ref int[] copyToArray)
        {
            if (copyFromArray.Length != copyToArray.Length)
            {
                Logger.Error("Array's length are different");
                return;
            }

            copyFromArray.CopyTo(copyToArray, 0);
        }

        bool FindRemovableMahjong(int[] mahjongArray, out int resultGridA, out int resultGridB)
        {
            int gridIdxA = -1;
            int gridIdxNext = -1;
            resultGridA = -1;
            resultGridB = -1;

            // horizontal
            for (int i = 0; i < 8; i++)
            {
                gridIdxA = i * 17;
                while (gridIdxA < (i + 1) * 17)
                {
                    while (mahjongArray[gridIdxA] == -1) gridIdxA++;

                    gridIdxNext = gridIdxA + 1;
                    while (gridIdxNext < (i + 1) * 17 && mahjongArray[gridIdxNext] == -1) gridIdxNext++;
                    if (gridIdxNext >= (i + 1) * 17) break;

                    if (IsSameType(mahjongArray[gridIdxA], mahjongArray[gridIdxNext]))
                    {
                        resultGridA = gridIdxA;
                        resultGridB = gridIdxNext;
                        break;
                    }
                    gridIdxA = gridIdxNext;
                }
                if (resultGridA != -1) break;
            }

            if (resultGridA == -1)
            {
                for (int i = 0; i < 17; i++)
                {
                    gridIdxA = i;
                    while (gridIdxA < 8 * 17 + i)
                    {
                        while (mahjongArray[gridIdxA] == -1) gridIdxA += 17;

                        gridIdxNext = gridIdxA + 17;
                        while (gridIdxNext < 8 * 17 + i && mahjongArray[gridIdxNext] == -1) gridIdxNext += 17;
                        if (gridIdxNext >= 8 * 17 + i) break;

                        if (IsSameType(mahjongArray[gridIdxA], mahjongArray[gridIdxNext]))
                        {
                            resultGridA = gridIdxA;
                            resultGridB = gridIdxNext;
                            break;
                        }
                        gridIdxA = gridIdxNext;
                    }
                    if (resultGridA != -1) break;
                }
            }

            return resultGridA != -1;
        }

        #endregion

        void OnMahjongPointerDown(object[] ps)
        {
            int idx = (int)ps[0];
            m_DragMahjongGridIdx = GetMahjongGridIdx(idx);

            //Logger.Log($"m_DragMahjongGridIdx = {m_DragMahjongGridIdx}");

            m_IsPressing = true;
        }

        Dictionary<int, int> m_TempMahjongDict = new Dictionary<int, int>();

        void OnMahjongPointerUp(object[] ps)
        {
            int idx = (int)ps[0];

            // 点击抬起时不是拖动就是点击
            if (!m_CanDoMove) OnClickMahjongItem(idx);

            m_IsPressing = false;
            m_IsFirstPress = true;
            m_CanDoMove = false;
            m_TempMahjongDict.Clear();

            // 求出新的位置
            Vector2 mousePos = Input.mousePosition;
            Vector3 offset = (mousePos - m_FirstPressPos) * UITools.ScreenScale / m_MahjongScale;
            if (m_IsHorizontalMove)
            {
                float horizontalMin = -m_LeftEmptyCount * ITEM_WIDTH;
                float horizontalMax = m_RightEmptyCount * ITEM_WIDTH;
                int idxOffset = Mathf.FloorToInt(Mathf.Clamp(offset.x, horizontalMin, horizontalMax) / ITEM_WIDTH + 0.5f);

                if (idxOffset < 0)
                {
                    for (int i = 0; i < m_MovableMahjongGridIdxList.Count; i++) // 记录
                    {
                        m_TempMahjongDict[m_MovableMahjongGridIdxList[i] + idxOffset] = m_MahjongArray[m_MovableMahjongGridIdxList[i]];
                    }
                    for (int i = 0; i < m_MovableMahjongGridIdxList.Count; i++) // 清空
                    {
                        m_MahjongArray[m_MovableMahjongGridIdxList[i]] = -1;
                    }
                }
                else if (idxOffset > 0)
                {
                    for (int i = m_MovableMahjongGridIdxList.Count - 1; i >= 0 ; i--) // 记录
                    {
                        m_TempMahjongDict[m_MovableMahjongGridIdxList[i] + idxOffset] = m_MahjongArray[m_MovableMahjongGridIdxList[i]];
                    }
                    for (int i = 0; i < m_MovableMahjongGridIdxList.Count; i++) // 清空
                    {
                        m_MahjongArray[m_MovableMahjongGridIdxList[i]] = -1;
                    }
                }
            }
            else
            {
                float verticalMin = -m_DownEmptyCount * ITEM_HEIGHT;
                float verticalMax = m_UpEmptyCount * ITEM_HEIGHT;
                int idxOffset = Mathf.FloorToInt(Mathf.Clamp(offset.y, verticalMin, verticalMax) / ITEM_HEIGHT + 0.5f);

                if (idxOffset < 0)
                {
                    for (int i = 0; i < m_MovableMahjongGridIdxList.Count; i++) // 记录
                    {
                        m_TempMahjongDict[m_MovableMahjongGridIdxList[i] + idxOffset * 17] = m_MahjongArray[m_MovableMahjongGridIdxList[i]];
                    }
                    for (int i = 0; i < m_MovableMahjongGridIdxList.Count; i++) // 清空
                    {
                        m_MahjongArray[m_MovableMahjongGridIdxList[i]] = -1;
                    }
                }
                else if (idxOffset > 0)
                {
                    for (int i = m_MovableMahjongGridIdxList.Count - 1; i >= 0; i--)
                    {
                        m_TempMahjongDict[m_MovableMahjongGridIdxList[i] + idxOffset * 17] = m_MahjongArray[m_MovableMahjongGridIdxList[i]];
                    }
                    for (int i = 0; i < m_MovableMahjongGridIdxList.Count; i++) // 清空
                    {
                        m_MahjongArray[m_MovableMahjongGridIdxList[i]] = -1;
                    }
                }
            }
            // 赋值
            foreach (var item in m_TempMahjongDict)
            {
                m_MahjongArray[item.Key] = item.Value;
            }

            UpdateShow(true);
        }

        bool m_IsPressing;
        bool m_IsFirstPress = true;
        Vector2 m_FirstPressPos;
        int m_DragMahjongGridIdx;
        bool m_CanDoMove;
        bool m_IsHorizontalMove;
        List<int> m_MovableMahjongGridIdxList = new List<int>();
        
        void LateUpdateCallback()
        {
            if (m_IsPressing)
            {
                Vector2 mousePos = Input.mousePosition;
                if (m_IsFirstPress)
                {
                    m_FirstPressPos = mousePos;
                    m_IsFirstPress = false;
                }

                Vector2 gridPos = GameRoot.Instance.m_UICamera.WorldToScreenPoint(m_ItemList[m_DragMahjongGridIdx].transform.position);

                Vector3 offset = (mousePos - m_FirstPressPos) * UITools.ScreenScale / m_MahjongScale;

                if (m_CanDoMove)
                {
                    for (int i = 0; i < m_MovableMahjongGridIdxList.Count; i++)
                    {
                        Vector3 gridPosV3 = GetMahjongGridPos(m_MovableMahjongGridIdxList[i]);
                        
                        if (m_IsHorizontalMove)
                        {
                            float horizontalMin = -m_LeftEmptyCount * ITEM_WIDTH;
                            float horizontalMax = m_RightEmptyCount * ITEM_WIDTH;
                            m_ItemList[m_MovableMahjongGridIdxList[i]].transform.localPosition = gridPosV3 +  Vector3.right * Mathf.Clamp(offset.x, horizontalMin, horizontalMax);
                        }
                        else
                        {
                            float verticalMin = -m_DownEmptyCount * ITEM_HEIGHT;
                            float verticalMax = m_UpEmptyCount * ITEM_HEIGHT;
                            m_ItemList[m_MovableMahjongGridIdxList[i]].transform.localPosition = gridPosV3 + Vector3.up * Mathf.Clamp(offset.y, verticalMin, verticalMax);
                        }
                        //m_ItemList[m_MovableMahjongList[i]].transform.localPosition = gridPosV3 + (m_IsHorizontalMove ? Vector3.right * offset.x : Vector3.up * offset.y);
                    }
                }
                else if (offset.magnitude > 20) // 拖拽超过阈值，开始移动
                {
                    m_CanDoMove = true;
                    m_IsHorizontalMove = Mathf.Abs(offset.x) > Mathf.Abs(offset.y);
                    CalcConnectMahjongId(m_DragMahjongGridIdx, m_IsHorizontalMove);
                }
            }
        }

        int m_LeftEmptyCount, m_RightEmptyCount, m_UpEmptyCount, m_DownEmptyCount = 0;

        void CalcConnectMahjongId(int gridIdx, bool isHorizontal)
        {
            m_MovableMahjongGridIdxList.Clear();
            m_MovableMahjongGridIdxList.Add(gridIdx);

            m_LeftEmptyCount = 0;
            m_RightEmptyCount = 0;
            m_UpEmptyCount = 0;
            m_DownEmptyCount = 0;

            int row = gridIdx / 17;
            int column = gridIdx % 17;

            bool findEmpty = false;

            if (isHorizontal)
            {
                // 左侧
                int leftCount = column;
                for (int i = 1; i <= leftCount; i++)
                {
                    int nextIdx = gridIdx - i;
                    if (m_MahjongArray[nextIdx] >= 0)
                    {
                        if (findEmpty) break;
                        else m_MovableMahjongGridIdxList.Add(nextIdx);
                    }
                    else
                    {
                        findEmpty = true;
                        m_LeftEmptyCount++;
                    }
                }
                // 右侧
                findEmpty = false;
                int rightCount = 16 - column;
                for (int i = 1; i <= rightCount; i++)
                {
                    int nextIdx = gridIdx + i;
                    if (m_MahjongArray[nextIdx] >= 0)
                    {
                        if (findEmpty) break;
                        else m_MovableMahjongGridIdxList.Add(nextIdx);
                    }
                    else
                    {
                        findEmpty = true;
                        m_RightEmptyCount++;
                    }
                }
            }
            else
            {
                // 上侧
                int upCount = 7 - row;
                findEmpty = false;
                for (int i = 1; i <= upCount; i++)
                {
                    int nextIdx = gridIdx + i * 17;
                    if (m_MahjongArray[nextIdx] >= 0)
                    {
                        if (findEmpty) break;
                        else m_MovableMahjongGridIdxList.Add(nextIdx);
                    }
                    else
                    {
                        findEmpty = true;
                        m_UpEmptyCount++;
                    }
                }

                // 下侧
                int downCount = row;
                findEmpty = false;
                for (int i = 1; i <= downCount; i++)
                {
                    int nextIdx = gridIdx - i * 17;
                    if (m_MahjongArray[nextIdx] >= 0)
                    {
                        if (findEmpty) break;
                        else m_MovableMahjongGridIdxList.Add(nextIdx);
                    }
                    else
                    {
                        findEmpty = true;
                        m_DownEmptyCount++;
                    }
                }
            }
        }

        void ShowTips(int gridIdxA, int gridIdxB)
        {
            if (gridIdxA == -1 || gridIdxB == -1) return;

            for (int i = 0; i < m_ItemList.Count; i++)
            {
                if (gridIdxA > -1 && m_ItemList[i].MahjongId == gridIdxA)
                {
                    m_ItemList[i].DoFlashHighLight();
                    gridIdxA = -1;
                }

                if (gridIdxB > -1 && m_ItemList[i].MahjongId == gridIdxB)
                {
                    m_ItemList[i].DoFlashHighLight();
                    gridIdxB = -1;
                }

                if (gridIdxA == -1 && gridIdxB == -1) break;
            }
        }
    }
}