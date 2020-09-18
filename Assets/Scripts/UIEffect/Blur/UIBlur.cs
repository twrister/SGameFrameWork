using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SthGame
{
    public class UIBlur : UIBaseEffect
    {
        [SerializeField]
        UIBlurMode _blurMode = UIBlurMode.None;

        [SerializeField]
        [Range(0, 2)]
        float _factor = 1;

        public UIBlurMode blurMode
        {
            get { return _blurMode; }
            set
            {
                if (_blurMode == value) return;
                _blurMode = value;
                SetMaterialDirty();
            }
        }

        public float factor
        {
            get { return _factor; }
            set
            {
                value = Mathf.Clamp(value, 0, 2);
                if (Mathf.Approximately(_factor, value)) return;
                _factor = value;
                UpdateParams();
            }
        }

        public override string ShaderPath()
        {
            return "Hidden/UIBlur";
        }

        protected override void ModifyMaterial(Material newMaterial)
        {
            SetShaderVariants(newMaterial, _blurMode);
        }

        protected override void UpdateParams()
        {
            if (newMaterial)
            {
                newMaterial.SetFloat("_Factor", _factor);
            }
        }
    }
}