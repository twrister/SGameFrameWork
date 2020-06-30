using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CircularScrollView;

namespace SthGame
{
    public class PacManGridController : UIChildController
    {
        PacManGridView view;
        GridData gridData;

        protected override string GetResourcePath()
        {
            return "Prefabs/PeaManGrid";
        }

        public override void Init()
        {
            base.Init();

            view = UINode as PacManGridView;
        }

        public void SetData(GridData data)
        {
            if (gridData == null || data.Index != gridData.Index)
                InitPos(data.Pos);

            gridData = data;
            UpdateView();
        }

        public bool TryEatBean()
        {
            if (gridData.GridType == EGridType.Bean)
            {
                gridData.GridType = EGridType.None;
                UpdateView();
                return true;
            }
            return false;
        }

        private void InitPos(Vector2Int pos)
        {
            view.InitPos(pos);
        }

        private void UpdateView()
        {
            view.beanObj.SetActive(gridData.GridType == EGridType.Bean);
        }
    }
}
