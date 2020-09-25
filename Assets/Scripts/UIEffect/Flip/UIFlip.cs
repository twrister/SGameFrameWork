using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SthGame
{
    public class UIFlip : UIBaseEffect
    {
        [SerializeField]
        private bool _horizontal = false;

        [SerializeField]
        private bool _vertical = false;

        public bool horizontal
        {
            get { return _horizontal; }
            set
            {
                if (_horizontal == value) return;
                _horizontal = value;
                SetVerticesDirty();
            }
        }

        public bool vertical
        {
            get { return _vertical; }
            set
            {
                if (_vertical == value) return;
                _vertical = value;
                SetVerticesDirty();
            }
        }

        protected override void ModifyMesh(VertexHelper verts, Graphic g)
        {
            if (!isActiveAndEnabled) return;

            var vt = default(UIVertex);
            for (var i = 0; i < verts.currentVertCount; i++)
            {
                verts.PopulateUIVertex(ref vt, i);
                var pos = vt.position;
                vt.position = new Vector3(
                    _horizontal ? -pos.x : pos.x,
                    _vertical ? -pos.y : pos.y
                );
                verts.SetUIVertex(vt, i);
            }
        }
    }
}