Shader "Unlit/RingSkillIndicator"
{
    Properties
    {
        _Color            ("Fill Color",        Color)       = (1,1,1,1)
        _InnerRadius      ("Inner Radius",      Range(0,1))  = 0.2
        _OuterRadius      ("Outer Radius",      Range(0,1))  = 0.5
        _Angle            ("Angle (°)",         Range(0,360))= 90
        _Direction        ("Direction (°)",     Range(0,360))= 0
        _BorderColor      ("Border Color",      Color)       = (0,0,0,1)
        _BorderThickness  ("Border Thickness",  Range(0,0.2))= 0.02
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Cull Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            fixed4 _Color;
            float _InnerRadius;
            float _OuterRadius;
            float _Angle;
            float _Direction;
            fixed4 _BorderColor;
            float _BorderThickness;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv  : TEXCOORD0;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv  = v.uv * 2 - 1;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 p = i.uv;
                float r  = length(p);

                // 1) 半径裁剪：丢弃在内圆以内或在外圆以外的像素
                if (r < _InnerRadius || r > _OuterRadius)
                    discard;

                // 2) 角度裁剪
                float ang = degrees(atan2(p.y, p.x));
                ang -= _Direction;
                ang = (ang > 180) ? ang - 360 : (ang < -180 ? ang + 360 : ang);

                float halfA = _Angle * 0.5;
                if (abs(ang) > halfA)
                    discard;

                // 3) 边框逻辑
                // 外圆边框
                bool isOuterArcBorder = (r >= _OuterRadius - _BorderThickness);
                // 内圆边框
                bool isInnerArcBorder = (r <= _InnerRadius + _BorderThickness);

                // 径向边框（同原理）
                float deltaA  = abs(abs(ang) - halfA);
                float dRadial = r * sin(radians(deltaA));
                bool isRadialBorder = (dRadial <= _BorderThickness);

                if (isOuterArcBorder || isInnerArcBorder || isRadialBorder)
                {
                    return _BorderColor;
                }

                // 4) 内部填充
                return _Color;
            }
            ENDCG
        }
    }
}
