using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Protocol;
using UnityEngine.Events;

namespace SthGame
{
    public class ColorPlateController : UIBasePopupController
    {
        ColorPlateView view;
        Color m_Color;
        UnityAction<Color> callback;
        Texture2D tex2d;
        Color[] colorArray;
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

        private void InitView()
        {
            if (isInit) return;

            colorArray = new Color[256 * 256];
            tex2d = new Texture2D(256, 256, TextureFormat.RGB24, true);
            view.rawImage.texture = tex2d;

            RefreshRawImage();

            view.nowColor.color = m_Color;
            view.slider_R.value = m_Color.r;
            view.slider_G.value = m_Color.g;
            view.slider_B.value = m_Color.b;
            view.slider_A.value = m_Color.a;

            isInit = true;
            RefreshView();
        }

        Color _tempColor = Color.white;

        private void RefreshRawImage()
        {
            Color maxColor =  m_Color / Mathf.Max(m_Color.r, m_Color.g, m_Color.b);

            for (int i = 0; i < 256; i++)
            {
                _tempColor = Color.Lerp(Color.white, maxColor, i / 255f);
                for (int j = 0; j < 256; j++)
                {
                    colorArray[j * 256 + i] = Color.Lerp(Color.black, _tempColor, j / 255f);
                }
            }

            tex2d.SetPixels(colorArray);
            tex2d.Apply();
        }

        private void RefreshView()
        {
            view.nowColor.color = new Color(m_Color.r, m_Color.g, m_Color.b, 1);

            view.valueTxt_R.text = (m_Color.r * 255).ToString();
            view.valueTxt_G.text = (m_Color.g * 255).ToString();
            view.valueTxt_B.text = (m_Color.b * 255).ToString();
            view.valueTxt_A.text = (m_Color.a * 255).ToString();
        }

        protected override void HideCallBack()
        {
            if (callback != null)
            {
                callback(view.nowColor.color);
            }
        }

        private void OnSliderValueChanged_R(float value)
        {
            m_Color.r = value;
            RefreshView();
        }
        private void OnSliderValueChanged_G(float value)
        {
            m_Color.g = value;
            RefreshView();
        }
        private void OnSliderValueChanged_B(float value)
        {
            m_Color.b = value;
            RefreshView();
        }
        private void OnSliderValueChanged_A(float value)
        {
            m_Color.a = value;
            RefreshView();
        }
    }
}