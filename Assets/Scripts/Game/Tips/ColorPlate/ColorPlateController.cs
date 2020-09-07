using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Protocol;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace SthGame
{
    public class ColorPlateController : UIBasePopupController
    {
        ColorPlateView view;
        Color m_Color;
        UnityAction<Color> callback;
        
        bool isInit;
        protected override string GetResourcePath()
        {
            return "Prefabs/ColorPlateView";
        }

        EventTrigger _rawImageTrigger;
        public override void Init()
        {
            base.Init();

            view = UINode as ColorPlateView;

            view.slider_R.onValueChanged.AddListener(OnSliderValueChanged_R);
            view.slider_G.onValueChanged.AddListener(OnSliderValueChanged_G);
            view.slider_B.onValueChanged.AddListener(OnSliderValueChanged_B);
            view.slider_A.onValueChanged.AddListener(OnSliderValueChanged_A);

            _rawImageTrigger = view.rawImage.gameObject.AddComponent<EventTrigger>();
            view.colorInput.SetOnPointerEvent(OnPointerEvent);
            view.hueColorInput.SetOnPointerEvent(OnHuePointerEvent);
        }

        public void SetColor(Color color, UnityAction<Color> cal)
        {
            m_Color = new Color(color.r, color.g, color.b, color.a);
            callback = cal;
            InitView();
        }

        //protected override void OpenCallBack()
        //{
        //    InitView();
        //}

        Texture2D tex2d;
        Color[] colorArray;
        Color[] hueColors = new Color[6] { Color.red, new Color(1, 1, 0), Color.green, Color.cyan, Color.blue, Color.magenta };
        private void InitView()
        {
            if (isInit) return;

            colorArray = new Color[256 * 256];
            tex2d = new Texture2D(256, 256, TextureFormat.RGB24, true);
            view.rawImage.texture = tex2d;

            var hueColorArray = new Color[360];
            var hueTex2d = new Texture2D(1, 360, TextureFormat.RGB24, true);
            
            view.hueRawImage.texture = hueTex2d;
            for (int i = 0; i < 360; i++)
            {
                int index = i / 60;
                int mod = i % 60;
                hueColorArray[i] = Color.Lerp(hueColors[index], hueColors[(index + 1) % 6], mod / 60f);
            }
            hueTex2d.SetPixels(hueColorArray);
            hueTex2d.Apply();

            view.nowColor.color = m_Color;
            view.slider_R.value = m_Color.r;
            view.slider_G.value = m_Color.g;
            view.slider_B.value = m_Color.b;
            view.slider_A.value = m_Color.a;

            isInit = true;
            RefreshView();
        }

        Color _tempColor = Color.white;
        Color _tempMaxColor = Color.white;
        Vector2 _cursorPos = Vector2.zero;
        private void RefreshRawImage()
        {
            float max = Mathf.Max(m_Color.r, m_Color.g, m_Color.b);
            _tempMaxColor = max == 0 ? Color.red : m_Color / max;
            float min = Mathf.Min(_tempMaxColor.r, _tempMaxColor.g, _tempMaxColor.b);
            if (min == 1)
            {
                _tempMaxColor = Color.red;
            }
            else
            {
                float v = 1f / (1 - min);
                _tempMaxColor = new Color(
                    Mathf.LerpUnclamped(1, _tempMaxColor.r, v),
                    Mathf.LerpUnclamped(1, _tempMaxColor.g, v),
                    Mathf.LerpUnclamped(1, _tempMaxColor.b, v));
            }

            _cursorPos.x = 1 - min;
            _cursorPos.y = max;

            for (int i = 0; i < 256; i++)
            {
                _tempColor = Color.Lerp(Color.white, _tempMaxColor, i / 255f);
                for (int j = 0; j < 256; j++)
                {
                    colorArray[j * 256 + i] = Color.Lerp(Color.black, _tempColor, j / 255f);
                }
            }

            view.cursorObj.transform.localPosition = new Vector3(view.rawImage.rectTransform.sizeDelta.x * _cursorPos.x,
                view.rawImage.rectTransform.sizeDelta.y * _cursorPos.y);

            tex2d.SetPixels(colorArray);
            tex2d.Apply();

            // hue cursor
            
            // 找出_tempMaxColor在hueColors中最相近的颜色
            int similarIdx = 0;
            float minDifValue = 3;
            float tempDifValue = 0;
            for (int i = 0; i < hueColors.Length; i++)
            {
                tempDifValue = Mathf.Abs(hueColors[i].r - _tempMaxColor.r) +
                    Mathf.Abs(hueColors[i].g - _tempMaxColor.g) +
                    Mathf.Abs(hueColors[i].b - _tempMaxColor.b);
                if (tempDifValue < minDifValue)
                {
                    minDifValue = tempDifValue;
                    similarIdx = i;
                }
            }
            // 求出_tempMaxColor与相近颜色的偏差

            float offset = (_tempMaxColor.r - hueColors[similarIdx].r) +
                (_tempMaxColor.g - hueColors[similarIdx].g) +
                (_tempMaxColor.b - hueColors[similarIdx].b);
            float val = similarIdx + offset;
            //float offset = (_tempMaxColor.r - hueColors[similarIdx].r) +
            //    (_tempMaxColor.g - hueColors[similarIdx].g) +
            //    (_tempMaxColor.b - hueColors[similarIdx].b);
            //float val = similarIdx % 2 == 0 ? similarIdx + offset : similarIdx - offset;
            view.hueCursorObj.transform.localPosition = new Vector3(0, val * view.hueRawImage.rectTransform.sizeDelta.y / 5f);
        }

        private void RefreshView()
        {
            view.nowColor.color = new Color(m_Color.r, m_Color.g, m_Color.b, 1);
            view.slider_Alpha.value = m_Color.a;

            view.valueTxt_R.text = (m_Color.r * 255).ToString("0");
            view.valueTxt_G.text = (m_Color.g * 255).ToString("0");
            view.valueTxt_B.text = (m_Color.b * 255).ToString("0");
            view.valueTxt_A.text = (m_Color.a * 255).ToString("0");

            RefreshRawImage();
        }

        protected override void HideCallBack()
        {
            if (callback != null)
            {
                callback(m_Color);
            }
        }

        private void OnSliderValueChanged_R(float value)
        {
            if (Mathf.Approximately(m_Color.r, value)) return;
            m_Color.r = value;
            RefreshView();
        }
        private void OnSliderValueChanged_G(float value)
        {
            if (Mathf.Approximately(m_Color.g, value)) return;
            m_Color.g = value;
            RefreshView();
        }
        private void OnSliderValueChanged_B(float value)
        {
            if (Mathf.Approximately(m_Color.b, value)) return;
            m_Color.b = value;
            RefreshView();
        }
        private void OnSliderValueChanged_A(float value)
        {
            if (Mathf.Approximately(m_Color.a, value)) return;
            m_Color.a = value;
            RefreshView();
        }

        private void OnPointerEvent(Vector2 localPos)
        {
            float x = Mathf.Clamp01(localPos.x / view.rawImage.rectTransform.sizeDelta.x);
            float y = Mathf.Clamp01(localPos.y / view.rawImage.rectTransform.sizeDelta.y);

            Color tmpColor = Color.Lerp(_tempMaxColor, Color.white, 1 - x);
            m_Color = Color.Lerp(Color.black, tmpColor, y);
            RefreshView();

            view.slider_R.value = m_Color.r;
            view.slider_G.value = m_Color.g;
            view.slider_B.value = m_Color.b;
            view.slider_A.value = m_Color.a;
        }

        private void OnHuePointerEvent(Vector2 localPos)
        {
            float y = Mathf.Clamp01(localPos.y / view.hueRawImage.rectTransform.sizeDelta.y);

        }
    }
}