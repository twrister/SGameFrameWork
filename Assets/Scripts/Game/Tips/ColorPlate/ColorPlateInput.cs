using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace SthGame
{
    public class ColorPlateInput : MonoBehaviour, IDragHandler, IPointerDownHandler
    {
        RectTransform _rt;
        Vector2 _localPos;
        UnityAction<Vector2> _onPointerEvent;
        
        public void OnDrag(PointerEventData eventData)
        {
            OnPointerEvent(eventData);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            OnPointerEvent(eventData);
        }

        private void OnPointerEvent(PointerEventData eventData)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_rt, eventData.position, Camera.main, out _localPos);

            if (_onPointerEvent != null) _onPointerEvent(_localPos);
        }

        public void SetOnPointerEvent(UnityAction<Vector2> _event)
        {
            _onPointerEvent = _event;
        }

        void Start()
        {
            _rt = GetComponent<RectTransform>();
        }
    }
}