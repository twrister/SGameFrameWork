using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;
using System.Text;
using System.IO;

namespace SthGame
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Graphic))]
    [DisallowMultipleComponent]
    public class UIToneEffect : UIBehaviour, IMaterialModifier
    {
        private static string shaderName = "UIToneEffect";
        private static readonly StringBuilder s_StringBuilder = new StringBuilder();
        Graphic _graphic;
        public Graphic graphic
        {
            get { return _graphic ? _graphic : _graphic = GetComponent<Graphic>(); }
        }

        [SerializeField]
        UIToneEffectMode m_EffectMode = UIToneEffectMode.None;

        [SerializeField]
        [Range(0, 1)]
        float m_EffectFactor = 1;

        protected override void OnValidate()
        {
            if (!isActiveAndEnabled) return;

            SetParams();
        }

        protected void SetParams()
        {
            if (newMaterial)
            {
                newMaterial.SetFloat("_Factor", m_EffectFactor);
            }
        }

        protected override void OnEnable()
        {
            SetMaterialDirty();
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            SetMaterialDirty();
        }

        public UIToneEffectMode effectMode
        {
            get { return m_EffectMode; }
            set
            {
                if (m_EffectMode == value) return;
                m_EffectMode = value;
                SetMaterialDirty();
            }
        }

        public float effectFactor
        {
            get { return m_EffectFactor; }
            set
            {
                value = Mathf.Clamp(value, 0, 1);
                if (Mathf.Approximately(m_EffectFactor, value)) return;
                m_EffectFactor = value;
                SetParams();
            }
        }

        public void SetMaterialDirty()
        {
            if (graphic) graphic.SetMaterialDirty();
        }

        Material newMaterial;
        public Material GetModifiedMaterial(Material baseMaterial)
        {
            if (!isActiveAndEnabled) return baseMaterial;

            newMaterial = new Material(baseMaterial);
            newMaterial.shader = Shader.Find("Hidden/UIToneEffect");
            SetShaderVariants(newMaterial, m_EffectMode);
            SetParams();
            return newMaterial;
        }

        protected void SetShaderVariants(Material newMaterial, params object[] variants)
        {
            // Set shader keywords as variants
            List<string> variantList = new List<string>();
            for (int i = 0; i < variants.Length; i++)
            {
                if ((int)variants[i] > 0)
                {
                    variantList.Add(variants[i].ToString().ToUpper());
                }
            }
            newMaterial.shaderKeywords = variantList.ToArray();

            // Add variant name
            s_StringBuilder.Length = 0;
            s_StringBuilder.Append(shaderName);
            foreach (var keyword in variantList)
            {
                s_StringBuilder.Append("-");
                s_StringBuilder.Append(keyword);
            }

            newMaterial.name = s_StringBuilder.ToString();
        }
    }
}