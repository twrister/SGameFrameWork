using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Protocol;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;

namespace SthGame
{
    public class ColorPlateController : UIBasePopupController
    {
        ColorPlateView view;
        Color _Color;
        UnityAction<Color> callback;
        RectTransform _rt;

        bool isInit;
        protected override string GetResourcePath()
        {
            return "Prefabs/ColorPlateView";
        }

        public override void Init()
        {
            base.Init();

            view = UINode as ColorPlateView;

            view.slider_R.onValueChanged.AddListener(OnSliderValueChanged_R);
            view.slider_G.onValueChanged.AddListener(OnSliderValueChanged_G);
            view.slider_B.onValueChanged.AddListener(OnSliderValueChanged_B);
            view.slider_A.onValueChanged.AddListener(OnSliderValueChanged_A);

            view.colorInput.SetOnPointerEvent(OnPointerEvent);
            view.hueColorInput.SetOnPointerEvent(OnHuePointerEvent);
        }

        public void SetColor(Color color, UnityAction<Color> cal, Transform attachTrans)
        {
            Vector2 localPos = Vector2.zero;
            Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, attachTrans.position);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(view.transform as RectTransform, screenPos, Camera.main, out localPos);
            bool isLeftSide = screenPos.x < Screen.width;
            view.rootTrans.localPosition = Vector2.right * (localPos.x + (isLeftSide ? -300 : 300));

            _Color = new Color(color.r, color.g, color.b, color.a);
            callback = cal;
            InitView();
        }

        Texture2D _tex2d;
        Color[] _colorArray;
        Color[] _hueColors = new Color[6] { Color.red, new Color(1, 1, 0), Color.green, Color.cyan, Color.blue, Color.magenta };
        private void InitView()
        {
            if (isInit) return;

            _colorArray = new Color[256 * 256];
            _tex2d = new Texture2D(256, 256, TextureFormat.RGB24, true);
            view.rawImage.texture = _tex2d;

            var hueColorArray = new Color[360];
            var hueTex2d = new Texture2D(1, 360, TextureFormat.RGB24, true);
            
            view.hueRawImage.texture = hueTex2d;
            for (int i = 0; i < 360; i++)
            {
                int index = i / 60;
                int mod = i % 60;
                hueColorArray[i] = Color.Lerp(_hueColors[index], _hueColors[(index + 1) % 6], mod / 60f);
            }
            hueTex2d.SetPixels(hueColorArray);
            hueTex2d.Apply();

            view.nowColor.color = _Color;
            view.slider_R.value = _Color.r;
            view.slider_G.value = _Color.g;
            view.slider_B.value = _Color.b;
            view.slider_A.value = _Color.a;

            isInit = true;
            RefreshView();
        }

        Color _tempColor = Color.white;
        Color _tempMaxColor = Color.white;
        Vector2 _cursorPos = Vector2.zero;
        List<Tuple<int, float>> _valueList = new List<Tuple<int, float>>();
        private void RefreshRawImage()
        {
            float max = Mathf.Max(_Color.r, _Color.g, _Color.b);
            _tempMaxColor = max == 0 ? Color.red : _Color / max;
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
                    _colorArray[j * 256 + i] = Color.Lerp(Color.black, _tempColor, j / 255f);
                }
            }

            view.cursorObj.transform.localPosition = new Vector3(view.rawImage.rectTransform.sizeDelta.x * _cursorPos.x,
                view.rawImage.rectTransform.sizeDelta.y * _cursorPos.y);

            _tex2d.SetPixels(_colorArray);
            _tex2d.Apply();

            // hue cursor
            
            // 找出_tempMaxColor在hueColors中最相近的颜色
            _valueList.Clear();
            float tempDifValue = 0;
            for (int i = 0; i < _hueColors.Length; i++)
            {
                tempDifValue = Mathf.Abs(_hueColors[i].r - _tempMaxColor.r) +
                    Mathf.Abs(_hueColors[i].g - _tempMaxColor.g) +
                    Mathf.Abs(_hueColors[i].b - _tempMaxColor.b);
                _valueList.Add(new Tuple<int, float>(i, tempDifValue));
            }
            _valueList.Sort((t1, t2) => t1.Item2.CompareTo(t2.Item2));

            // 求出_tempMaxColor与相近颜色的偏差
            int similarIdx = _valueList[0].Item1;
            float minDifValue = _valueList[0].Item2;
            int secondSimilarIdx = _valueList[1].Item1;
            if (Mathf.Abs(secondSimilarIdx - similarIdx) > 1)
            {
                similarIdx = similarIdx == 0 ? 6 : similarIdx;
                secondSimilarIdx = secondSimilarIdx == 0 ? 6 : secondSimilarIdx;
            }
            float val = secondSimilarIdx > similarIdx ? similarIdx + minDifValue : similarIdx - minDifValue;
            view.hueCursorObj.transform.localPosition = new Vector3(0, val * view.hueRawImage.rectTransform.sizeDelta.y / 6f);
        }

        private void RefreshView()
        {
            view.nowColor.color = new Color(_Color.r, _Color.g, _Color.b, 1);
            view.slider_Alpha.value = _Color.a;

            view.valueTxt_R.text = (_Color.r * 255).ToString("0");
            view.valueTxt_G.text = (_Color.g * 255).ToString("0");
            view.valueTxt_B.text = (_Color.b * 255).ToString("0");
            view.valueTxt_A.text = (_Color.a * 255).ToString("0");

            RefreshRawImage();

            if (callback != null) callback(_Color);
        }

        protected override void HideCallBack()
        {
            if (callback != null) callback(_Color);
        }

        private void OnSliderValueChanged_R(float value)
        {
            if (Mathf.Approximately(_Color.r, value)) return;
            _Color.r = value;
            RefreshView();
        }
        private void OnSliderValueChanged_G(float value)
        {
            if (Mathf.Approximately(_Color.g, value)) return;
            _Color.g = value;
            RefreshView();
        }
        private void OnSliderValueChanged_B(float value)
        {
            if (Mathf.Approximately(_Color.b, value)) return;
            _Color.b = value;
            RefreshView();
        }
        private void OnSliderValueChanged_A(float value)
        {
            if (Mathf.Approximately(_Color.a, value)) return;
            _Color.a = value;
            RefreshView();
        }

        private void OnPointerEvent(Vector2 localPos)
        {
            float x = Mathf.Clamp01(localPos.x / view.rawImage.rectTransform.sizeDelta.x);
            float y = Mathf.Clamp01(localPos.y / view.rawImage.rectTransform.sizeDelta.y);

            Color tmpColor = Color.Lerp(_tempMaxColor, Color.white, 1 - x);
            _Color = Color.Lerp(Color.black, tmpColor, y);
            RefreshView();

            SyncSliders();
        }

        private void OnHuePointerEvent(Vector2 localPos)
        {
            float y = Mathf.Clamp01(localPos.y / view.hueRawImage.rectTransform.sizeDelta.y);
            int index1 = Mathf.FloorToInt(y * 6) % 6;
            int index2 = Mathf.CeilToInt(y * 6) % 6;
            Color tmpMaxColor = Color.Lerp(_hueColors[index1], _hueColors[index2], y * 6 - index1);
            Color tmpColor = Color.Lerp(Color.white, tmpMaxColor, _cursorPos.x);
            _Color = Color.Lerp(Color.black, tmpColor, _cursorPos.y);
            RefreshView();

            SyncSliders();
        }

        private void SyncSliders()
        {
            view.slider_R.value = _Color.r;
            view.slider_G.value = _Color.g;
            view.slider_B.value = _Color.b;
            view.slider_A.value = _Color.a;
        }
    }
}