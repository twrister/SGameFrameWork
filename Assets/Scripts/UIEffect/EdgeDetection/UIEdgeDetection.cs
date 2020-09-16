using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SthGame
{
    public class UIEdgeDetection : UIBaseEffect
    {
        [SerializeField]
        UIEdgeDetectionMode _edgeDetectionMode = UIEdgeDetectionMode.None;

        [SerializeField]
        [Range(0, 2)]
        float _edgeWidth = 1;

        [SerializeField]
        Color _edgeColor = Color.black;

        [SerializeField]
        bool _bgToggle = false;

        [SerializeField]
        [Range(0, 1)]
        float _bgAlpha = 1;

        public float edgeWidth
        {
            get { return _edgeWidth; }
            set
            {
                value = Mathf.Clamp(value, 0, 2);
                if (Mathf.Approximately(_edgeWidth, value)) return;
                _edgeWidth = value;
                UpdateParams();
            }
        }

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

        public Color edgeColor
        {
            get { return _edgeColor; }
            set
            {
                _edgeColor = value;
                UpdateParams();
            }
        }

        public float bgToggle
        {
            get { return _bgToggle ? 1 : 0; }
            set
            {
                _bgToggle = value == 1;
                UpdateParams();
            }
        }

        public float bgAlpha
        {
            get { return _bgAlpha; }
            set
            {
                value = Mathf.Clamp(value, 0, 1);
                if (Mathf.Approximately(_bgAlpha, value)) return;
                _bgAlpha = value;
                UpdateParams();
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
                newMaterial.SetFloat("_EdgeWidth", _edgeWidth);
                newMaterial.SetColor("_EdgeColor", _edgeColor);
                newMaterial.SetFloat("_BgToggle", bgToggle);
                newMaterial.SetFloat("_BgAlpha", _bgAlpha);
            }
        }
    }
}