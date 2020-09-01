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
    public class UIEffect : UIBehaviour, IMaterialModifier
    {
        private static string shaderName = "UIToneEffect";
        private static readonly StringBuilder s_StringBuilder = new StringBuilder();
        Graphic _graphic;
        public Graphic graphic
        {
            get { return _graphic ? _graphic : _graphic = GetComponent<Graphic>(); }
        }

        [SerializeField]
        EffectMode m_EffectMode = EffectMode.None;

        [SerializeField]
        [Range(0, 1)]
        float m_EffectFactor = 1;

        protected override void OnValidate()
        {
            if (!isActiveAndEnabled) return;

            Logger.Log("OnValidate");
            //SetMaterialDirty();
            SetParamsDirty();
        }

        //public override ParameterTexture paramTex
        //{
        //    get { return paramTex; }
        //}

        protected void SetParamsDirty()
        {

            //paramTex.SetData(this, 0, m_EffectFactor); // param.x : effect factor
        }

        protected override void OnEnable()
        {
            //base.OnEnable();

            SetMaterialDirty();

            Initialize();
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            SetMaterialDirty();
        }

        void Initialize()
        {

        }

        public void SetMaterialDirty()
        {
            Logger.Log("SetMaterialDirty");
            if (graphic) graphic.SetMaterialDirty();
        }

        public Material GetModifiedMaterial(Material baseMaterial)
        {
            if (!isActiveAndEnabled) return baseMaterial;

            var newMaterial = new Material(baseMaterial);
            newMaterial.shader = Shader.Find("2D Shader/UIToneEffect");
            //newMaterial.name = "GRAYSCALE";
            SetShaderVariants(newMaterial, m_EffectMode);
            //SetMaterialTexture(newMaterial);

            return newMaterial;
        }

        Texture2D texture;
        string propertyName = "paranTex";
        private void SetMaterialTexture(Material mat)
        {
            if (mat)
            {
                int propertyId = Shader.PropertyToID(propertyName);
                mat.SetTexture(propertyId, texture);
            }
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

            //var keywords = variants.Where(x => 0 < (int)x)
            //    .Select(x => x.ToString().ToUpper())
            //    .Concat(newMaterial.shaderKeywords)
            //    .Distinct()
            //    .ToArray();
            //newMaterial.shaderKeywords = keywords;

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