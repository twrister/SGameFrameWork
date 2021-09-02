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
        public Button gridButton;
        public Image gridImage;
        public Text indexText;

        static Color blockColor = new Color(0.4f, 0.4f, 0.4f);

        public int posX { get; private set; }
        public int posY { get; private set; }
        public int Index
        {
            get { return posX * MapHeight + posY; }
        }

        EWayFindingGridType _gridType;
        public EWayFindingGridType GridType
        {
            get { return _gridType; }
            set
            {
                _gridType = value;
                switch (_gridType)
                {
                    case EWayFindingGridType.Normal:
                        gridImage.color = Color.white;
                        break;
                    case EWayFindingGridType.Block:
                        gridImage.color = blockColor;
                        break;
                }
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

        public void InitPos(int x, int y, int inEdge, int tX, int tY, EWayFindingGridType type = EWayFindingGridType.Normal)
        {
            posX = x;
            posY = y;
            GridEdge = inEdge;
            MapWidth = tX;
            MapHeight = tY;
            GridType = type;
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