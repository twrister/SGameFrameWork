using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SthGame;
using UnityEngine.UI;

namespace SthGame
{
    public class PacManGridView : UIBaseView
    {
        public GameObject beanObj;

        public static int GridSize = 60;

        public void InitPos(Vector2Int pos)
        {
            //ResetAnchor();
            float posX = pos.x * GridSize - GridSize * 4.5f;
            float posY = -pos.y * GridSize + GridSize * 4.5f;
            transform.localPosition = new Vector3(posX, posY);
        }

        private void ResetAnchor()
        {
            RectTransform rectTrans = chcheTransform as RectTransform;
            rectTrans.pivot = Vector2.up;
            rectTrans.anchorMin = Vector2.up;
            rectTrans.anchorMax = Vector2.up;
        }
    }
}