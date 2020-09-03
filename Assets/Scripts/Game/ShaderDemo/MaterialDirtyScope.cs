using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace SthGame
{
    public class MaterialDirtyScope : EditorGUI.ChangeCheckScope
    {
        readonly Object[] targets;

        public MaterialDirtyScope(Object[] targets)
        {
            this.targets = targets;
        }

        protected override void CloseScope()
        {
            if (changed)
            {
                foreach (var effect in targets.OfType<UIToneEffect>())
                {
                    effect.SetMaterialDirty();
                }
            }

            base.CloseScope();
        }
    }
}

