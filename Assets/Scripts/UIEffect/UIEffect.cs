using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SthGame
{

    public class UIEffect : UIBehaviour
    {
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

            Initialize();
        }

        void Initialize()
        {

        }
    }
}