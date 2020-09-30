using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SthGame
{
    public class UITransform : UIBaseEffect
    {
        [SerializeField]
        [Range(0, 2)]
        float _Scale = 1;

        [SerializeField]
        [Range(-180, 180)]
        float _Rotate = 0;

        public float scale
        {
            get { return _Scale; }
            set
            {
                value = Mathf.Clamp(value, 0, 1);
                if (Mathf.Approximately(_Scale, value)) return;
                _Scale = value;
                UpdateParams();
            }
        }

        public override string ShaderPath()
        {
            return "Hidden/UITransform";
        }

        protected override void ModifyMaterial(Material newMaterial)
        {

        }

        protected override void UpdateParams()
        {
            if (newMaterial)
            {
                newMaterial.SetFloat("_Scale", scale);
            }
        }
    }
}