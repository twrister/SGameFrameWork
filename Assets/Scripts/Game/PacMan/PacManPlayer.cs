using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SthGame
{
    public class PacManPlayer : MonoBehaviour
    {
        private Vector2Int curPos;
        void Start()
        {

        }

        void Update()
        {

        }

        public void InitFirstPos()
        {
            SetPos(Vector2Int.zero);
        }

        public void SetPos(Vector2Int pos)
        {
            curPos = pos;
            float posX = pos.x * PacManGridView.GridSize - PacManGridView.GridSize * 4.5f;
            float posY = -pos.y * PacManGridView.GridSize + PacManGridView.GridSize * 4.5f;
            transform.localPosition = new Vector3(posX, posY);
            DoEatBean();
        }

        public void DoMove(Vector2Int moveDir)
        {
            if (curPos.x + moveDir.x < 0 || curPos.x + moveDir.x > 9) return;
            if (curPos.y + moveDir.y < 0 || curPos.y + moveDir.y > 9) return;

            curPos += moveDir;
            SetPos(curPos);
        }

        public void MoveByResolution(EResolution res)
        {
            res = res == EResolution.Random ? (EResolution)UnityEngine.Random.Range(0, 4) : res;
            switch (res)
            {
                case EResolution.Left:
                    DoMove(Vector2Int.left);
                    break;
                case EResolution.Right:
                    DoMove(Vector2Int.right);
                    break;
                case EResolution.Up:
                    DoMove(Vector2Int.down);
                    break;
                case EResolution.Down:
                    DoMove(Vector2Int.up);
                    break;
            }
        }

        private void DoEatBean()
        {
            GlobalEventSystem.Instance.Fire(EventId.onPacManTryToEatBean, curPos);
        }

        // 0空 1豆 2墙
        public int GetResolutionIdx(List<GridData> gridDatas)
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
    }
}
