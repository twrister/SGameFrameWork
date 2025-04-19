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
                o.uv  = v.uv * 2 - 1;    // UV from [0,1] to [-1,1]
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 p = i.uv;
                float r  = length(p);

                // 1) Radius discard: keep only within ring
                if (r < _InnerRadius || r > _OuterRadius)
                    discard;

                // 2) Angle discard: keep only within sector
                float ang = degrees(atan2(p.y, p.x)) - _Direction;
                if (ang > 180) ang -= 360;
                if (ang < -180) ang += 360;
                float halfA = _Angle * 0.5;
                if (abs(ang) > halfA)
                    discard;

                // 3) Compute border conditions
                float deltaA  = abs(abs(ang) - halfA);
                float dRadial = r * sin(radians(deltaA));
                bool isOuterArc  = (r >= _OuterRadius - _BorderThickness);
                bool isInnerArc  = (r <= _InnerRadius + _BorderThickness);
                bool isRadial    = (dRadial <= _BorderThickness);

                fixed4 result;

                if (isOuterArc || isInnerArc || isRadial)
                {
                    result = _BorderColor;
                }
                else
                {
                    // 4) Fill fade: lerp from border to fill
                    float dInner = r - (_InnerRadius + _BorderThickness);
                    float dOuter = (_OuterRadius - _BorderThickness) - r;
                    float dRad   = dRadial - _BorderThickness;
                    float dMin   = min(min(dInner, dOuter), dRad);
                    float t = saturate(dMin / _FadeDistance);
                    result = lerp(_BorderColor, _Color, t);
                }

                return result;
            }
            ENDCG
        }
    }
}
