﻿using System.Collections;
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
        public const int PATH = 4;

        public Button gridButton;
        public Image gridImage;
        public Text indexText;

        static Dictionary<int, Color> GRID_COLOR_DICT = new Dictionary<int, Color>()
        {
            { NORMAL,   Color.white},
            { BLOCK,    new Color(0.4f, 0.4f, 0.4f)},
            { REACHED,  new Color(0.8f, 0.8f, 0.8f)},
            { FRONTIER, new Color(0.9f, 0.6f, 0.6f)},
            { PATH,     new Color(0.2f, 0.9f, 0.7f)},
        };

        public int posX { get; private set; }
        public int posY { get; private set; }
        public int Index
        {
            get { return posX * MapHeight + posY; }
        }

        public void SetGridState(int state, bool isBlock)
        {
            state = isBlock ? BLOCK : state;
            gridImage.color = GRID_COLOR_DICT.ContainsKey(state) ? GRID_COLOR_DICT[state] : Color.white;
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
            GlobalEventSystem.Instance.Fire(EventId.aStarOnClickGrid, Index);
        }
    }
}