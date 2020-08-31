using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SthGame
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Graphic))]
    [DisallowMultipleComponent]
    public class UIEffect : UIBehaviour, IMaterialModifier
    {
        //Graphic _graphic;
        //public Graphic graphic
        //{
        //    get { return _graphic ? _graphic : _graphic = GetComponent<Graphic>(); }
        //}

        [SerializeField]
        EffectMode m_EffectMode = EffectMode.None;

        [SerializeField]
        [Range(0, 1)]
        float m_EffectFactor = 1;

        protected override void OnValidate()
        {
            if (!isActiveAndEnabled) return;
            SetEffectParamsDirty();
        }

        //public override ParameterTexture paramTex
        //{
        //    get { return paramTex; }
        //}

        protected void SetEffectParamsDirty()
        {
            //paramTex.SetData(this, 0, m_EffectFactor); // param.x : effect factor
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            //if (graphic) graphic.SetVerticesDirty();

            Initialize();
        }

        void Initialize()
        {

        }

        public Material GetModifiedMaterial(Material baseMaterial)
        {
            Logger.Log("GetModifiedMaterial");

            if (!isActiveAndEnabled) return baseMaterial;

            var modifiedMaterial = baseMaterial;
            return modifiedMaterial;
        }
    }
}