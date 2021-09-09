using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace SthGame
{
    public class PathFindingMovableItem : MonoBehaviour, IDragHandler
    {
        int gridEdge = 0;
        int mapWidth = 0;
        int mapHeight = 0;
        Transform gridParent;

        Vector2Int _gridPos;
        public Vector2Int GridPos
        {
            set
            {
                if (_gridPos != value)
                {
                    _gridPos = value;
                    PathFindingBaseController.SetLocalPosByGridPos(this.transform, _gridPos.x, _gridPos.y, gridEdge, mapWidth, mapHeight);
                    if (onPosChanged != null)
                    {
                        onPosChanged();
                    }
                }
            }
            get { return _gridPos; }
        }

        public int Index
        {
            get { return _gridPos.x * mapHeight + _gridPos.y; }
        }

        RectTransform _rt;
        UnityAction onPosChanged;

        RectTransform Rect
        {
            get
            {
                if (_rt == null)
                {
                    _rt = gameObject.GetComponent<RectTransform>();
                }
                return _rt;
            }
        }

        public void InitMovableItem(Vector2Int gridPos, int inEdge, int tX, int tY, Transform parent, UnityAction callback = null)
        {
            gridEdge = inEdge;
            mapWidth = tX;
            mapHeight = tY;
            gridParent = parent;
            onPosChanged = callback;
            GridPos = gridPos;
        }

        public void OnDrag(PointerEventData eventData)
        {
            Vector3 worldMousePos;
            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(Rect, eventData.position, eventData.pressEventCamera, out worldMousePos))
            {
                var localPos = gridParent.InverseTransformVector(worldMousePos);
                GridPos = PathFindingBaseController.GetGridPosByLocalPos(localPos.x, localPos.y, gridEdge, mapWidth, mapHeight);
            }
        }
    }
}