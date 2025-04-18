Shader "Unlit/SkillIndicator"
{
    Properties
    {
        _Color            ("Fill Color",        Color)       = (1,1,1,1)
        _Radius           ("Radius",            Range(0,1))  = 0.5
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
            float _Radius;
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
                // UV 从 [0,1] 映射到中心为 (0,0) 的 [-1,1]
                o.uv = v.uv * 2 - 1;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 p = i.uv;
                float r = length(p);

                // 1) 半径裁剪
                if (r > _Radius)
                    discard;

                // 2) 角度裁剪
                // atan2 返回弧度，degrees() 转为度
                float ang = degrees(atan2(p.y, p.x));
                // 调整方向偏移
                ang -= _Direction;
                // 归一化到 [-180,180]
                ang = (ang > 180) ? ang - 360 : (ang < -180 ? ang + 360 : ang);

                float halfA = _Angle * 0.5;
                if (abs(ang) > halfA)
                    discard;

                // 3) 描边逻辑
                // 判断是否在外弧描边区域
                bool isArcBorder = (r >= _Radius - _BorderThickness);

                // 判断是否在两条径向边界线附近
                // 计算像素到径向边界线的距离：d = r * sin(Δθ)
                float deltaA = abs(abs(ang) - halfA);
                float dRadial = r * sin(radians(deltaA));
                bool isRadialBorder = (dRadial <= _BorderThickness);

                if (isArcBorder || isRadialBorder)
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
