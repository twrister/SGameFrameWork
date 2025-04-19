Shader "Unlit/RingSkillIndicatorSimpleFade"
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
        _FadeDistance     ("Fade Distance",     Range(0,0.2))= 0.05
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
            float _FadeDistance;

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
                o.uv  = v.uv * 2 - 1;    // UV 从 [0,1] 转到 [-1,1]
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 p = i.uv;
                float r  = length(p);

                // 1) 半径裁剪：只保留环带内
                if (r < _InnerRadius || r > _OuterRadius)
                    discard;

                // 2) 角度裁剪：只保留扇形区域
                float ang = degrees(atan2(p.y, p.x)) - _Direction;
                if (ang > 180) ang -= 360;
                if (ang < -180) ang += 360;
                float halfA = _Angle * 0.5;
                if (abs(ang) > halfA)
                    discard;

                // 3) 边框判断：内外圆弧或径向边界
                float deltaA  = abs(abs(ang) - halfA);
                float dRadial = r * sin(radians(deltaA));
                bool isOuterArc  = (r >= _OuterRadius - _BorderThickness);
                bool isInnerArc  = (r <= _InnerRadius + _BorderThickness);
                bool isRadial    = (dRadial <= _BorderThickness);
                if (isOuterArc || isInnerArc || isRadial)
                {
                    return _BorderColor;
                }

                // 4) 填充渐变：直接从 BorderColor 过渡到 Fill Color
                // 计算到边框的最小距离
                float dInner = r - (_InnerRadius + _BorderThickness);
                float dOuter = (_OuterRadius - _BorderThickness) - r;
                float dRad   = dRadial - _BorderThickness;
                float dMin   = min(min(dInner, dOuter), dRad);

                // 归一化到 [0,1]
                float t = saturate(dMin / _FadeDistance);

                // 单次线性插值
                fixed4 col = lerp(_BorderColor, _Color, t);
                return col;
            }
            ENDCG
        }
    }
}
