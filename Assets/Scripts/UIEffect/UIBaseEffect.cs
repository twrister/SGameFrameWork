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
    public abstract class UIBaseEffect : UIBehaviour, IMaterialModifier, IMeshModifier
    {
        private static readonly StringBuilder _StringBuilder = new StringBuilder();
        Graphic _graphic;
        protected Material newMaterial;
        public Graphic graphic
        {
            get { return _graphic ? _graphic : _graphic = GetComponent<Graphic>(); }
        }

        public Material GetModifiedMaterial(Material baseMaterial)
        {
            if (!isActiveAndEnabled) return baseMaterial;

            newMaterial = new Material(baseMaterial);

            if (!string.IsNullOrEmpty(ShaderPath()))
            {
                newMaterial.shader = Shader.Find(ShaderPath());
                ModifyMaterial(newMaterial);
                UpdateParams();
            }
            return newMaterial;
        }

        protected override void OnEnable()
        {
            SetMaterialDirty();
            SetVerticesDirty();
        }

        protected override void OnDisable()
        {
            SetMaterialDirty();
            SetVerticesDirty();
        }

        //protected override void OnValidate()
        //{
        //    if (!isActiveAndEnabled) return;

        //    UpdateParams();
        //}

        protected void SetMaterialDirty()
        {
            if (graphic) graphic.SetMaterialDirty();
        }

        protected void SetVerticesDirty()
        {
            if (graphic) graphic.SetVerticesDirty();
        }

        public virtual string ShaderPath() { return ""; }

        protected virtual void UpdateParams()
        {
        }

        protected virtual void ModifyMaterial(Material newMaterial)
        {
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
            _StringBuilder.Length = 0;
            _StringBuilder.Append(ShaderPath());
            foreach (var keyword in variantList)
            {
                _StringBuilder.Append("-");
                _StringBuilder.Append(keyword);
            }

            newMaterial.name = _StringBuilder.ToString();
        }

        public void ModifyMesh(Mesh mesh)
        {
        }

        public void ModifyMesh(VertexHelper verts)
        {
            ModifyMesh(verts, graphic);
        }

        protected virtual void ModifyMesh(VertexHelper verts, Graphic g)
        {
        }
    }
}