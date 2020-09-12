using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SthGame
{
    public class UIHSVModifier : UIBehaviour, IMaterialModifier
    {
        [SerializeField]
        [Range(0, 1)]
        float m_Range = 0.1f;

        [SerializeField]
        [ColorUsage(false)] // false:不显示alpha通道
        Color m_TargetColor = Color.red;

        [SerializeField]
        [Range(-0.5f, 0.5f)]
        float m_Hue;

        [SerializeField]
        [Range(-0.5f, 0.5f)]
        float m_Saturation;

        [SerializeField]
        [Range(-0.5f, 0.5f)]
        float m_Value;

        Material newMaterial;
        public Material GetModifiedMaterial(Material baseMaterial)
        {
            if (!isActiveAndEnabled) return baseMaterial;

            newMaterial = new Material(baseMaterial);
            newMaterial.shader = Shader.Find("Hidden/UIHSVModifier");
            SetParams();
            return newMaterial;
        }

        public float range
        {
            get { return m_Range; }
            set
            {
                value = Mathf.Clamp(value, 0, 1);
                if (Mathf.Approximately(m_Range, value)) return;
                m_Range = value;
                SetParams();
            }
        }

        public Color targetColor
        {
            get { return m_TargetColor; }
            set
            {
                m_TargetColor = value;
                SetParams();
            }
        }

        public float hue
        {
            get { return m_Hue; }
            set
            {
                value = Mathf.Clamp(value, -0.5f, 0.5f);
                if (Mathf.Approximately(m_Hue, value)) return;
                m_Hue = value;
                SetParams();
            }
        }

        public float saturation
        {
            get { return m_Saturation; }
            set
            {
                value = Mathf.Clamp(value, -0.5f, 0.5f);
                if (Mathf.Approximately(m_Saturation, value)) return;
                m_Saturation = value;
                SetParams();
            }
        }

        public float value
        {
            get { return m_Value; }
            set
            {
                value = Mathf.Clamp(value, -0.5f, 0.5f);
                if (Mathf.Approximately(m_Value, value)) return;
                m_Value = value;
                SetParams();
            }
        }

        protected override void OnValidate()
        {
            if (!isActiveAndEnabled) return;

            SetParams();
        }

        protected void SetParams()
        {
            if (newMaterial)
            {
                float h, s, v;
                Color.RGBToHSV(m_TargetColor, out h, out s, out v);

                var param1 = new Vector4(h, s, v, m_Range);
                newMaterial.SetVector("_Param1", param1);

                // 传入的范围必须是0~1，所以右移0.5
                var param2 = new Vector4(m_Hue + 0.5f, m_Saturation + 0.5f, m_Value + 0.5f, 0);
                newMaterial.SetVector("_Param2", param2);
            }
        }
    }
}
