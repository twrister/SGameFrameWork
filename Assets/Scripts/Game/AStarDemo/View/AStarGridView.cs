using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SthGame
{
    public enum EWayFindingGridType
    {
        Normal,
        Block,
        __Count,
    }

    public class AStarGridView : MonoBehaviour
    {
        public const int NORMAL = 0;
        public const int BLOCK = 1;
        public const int REACHED = 2;
        public const int FRONTIER = 3;

        public Button gridButton;
        public Image gridImage;
        public Text indexText;

        static Color block_Color = new Color(0.4f, 0.4f, 0.4f);
        static Color reached_Color = new Color(0.8f, 0.8f, 0.8f);
        static Color frontier_Color = new Color(0.9f, 0.6f, 0.6f);

        public int posX { get; private set; }
        public int posY { get; private set; }
        public int Index
        {
            get { return posX * MapHeight + posY; }
        }

        //EWayFindingGridType _gridType;
        //public EWayFindingGridType GridType
        //{
        //    get { return _gridType; }
        //    set
        //    {
        //        _gridType = value;
        //        switch (_gridType)
        //        {
        //            case EWayFindingGridType.Normal:
        //                gridImage.color = Color.white;
        //                break;
        //            case EWayFindingGridType.Block:
        //                gridImage.color = blockColor;
        //                break;
        //        }
        //    }
        //}
        public void SetGridState(int state, bool isBlock)
        {
            state = isBlock ? BLOCK : state;
            switch (state)
            {
                case NORMAL:
                    gridImage.color = Color.white;
                    break;
                case BLOCK:
                    gridImage.color = block_Color;
                    break;
                case REACHED:
                    gridImage.color = reached_Color;
                    break;
                case FRONTIER:
                    gridImage.color = frontier_Color;
                    break;
            }
        }

        public void Reset()
        {
            indexText.text = "";
            this.transform.localPosition = Vector3.zero;
            this.gameObject.SetActive(false);
        }

        public int GridEdge { get; private set; }
        public int MapWidth { get; private set; }
        public int MapHeight { get; private set; }

        public void InitPos(int x, int y, int inEdge, int tX, int tY)
        {
            posX = x;
            posY = y;
            GridEdge = inEdge;
            MapWidth = tX;
            MapHeight = tY;
            indexText.text = Index.ToString();
            CalcPos();
        }

        void CalcPos()
        {
            AStarDemoController.SetLocalPosByGridPos(transform, posX, posY, GridEdge, MapWidth, MapHeight);
        }

        private void OnEnable()
        {
            if (gridButton != null)
            {
                gridButton.onClick.AddListener(OnClickGrid);
            }
        }

        void OnClickGrid()
        {
            //GridType = (EGridType)(((int)GridType + 1) % (int)EGridType.__Count);

            GlobalEventSystem.Instance.Fire(EventId.aStarOnClickGrid, Index);

            //Logger.Log("grid's local pos = {0}", transform.localPosition.ToString());
        }

        //public 
    }
}