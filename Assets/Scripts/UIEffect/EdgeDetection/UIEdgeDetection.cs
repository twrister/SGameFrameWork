using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SthGame
{
    public class UIEdgeDetection : UIBaseEffect
    {
        [SerializeField]
        [Range(0, 1)]
        float _EdgeWidth = 1;

        [SerializeField]
        UIEdgeDetectionMode _edgeDetectionMode = UIEdgeDetectionMode.None;

        public UIEdgeDetectionMode edgeDetectionMode
        {
            get { return _edgeDetectionMode; }
            set
            {
                if (_edgeDetectionMode == value) return;
                _edgeDetectionMode = value;
                SetMaterialDirty();
            }
        }

        public override string ShaderPath()
        {
            return "Hidden/UIEdgeDetection";
        }

        protected override void ModifyMaterial(Material newMaterial)
        {
            SetShaderVariants(newMaterial, _edgeDetectionMode);
        }

        protected override void UpdateParams()
        {
            if (newMaterial)
            {
                newMaterial.SetFloat("_EdgeWidth", _EdgeWidth);
            }
        }
    }
}