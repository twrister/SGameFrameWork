Shader "Hidden/UIEdgeDetection"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _EdgeWidth ("Edge Width", Float) = 0
    }
    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Fog { Mode Off }
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #pragma multi_compile __ SOBEL ROBERTS SCHARR

            #include "UnityCG.cginc"
            #include "UIEffect.cginc"

            sampler2D _MainTex;
            fixed _EdgeWidth;
            float4 _MainTex_TexelSize;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv[9] : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);

                o.uv[0] = v.uv + _MainTex_TexelSize.xy * half2(-1, -1);
                o.uv[1] = v.uv + _MainTex_TexelSize.xy * half2(0, 1);
                o.uv[2] = v.uv + _MainTex_TexelSize.xy * half2(1, -1);
                o.uv[3] = v.uv + _MainTex_TexelSize.xy * half2(-1, 0);
                o.uv[4] = v.uv + _MainTex_TexelSize.xy * half2(0, 0);
                o.uv[5] = v.uv + _MainTex_TexelSize.xy * half2(1, 0);
                o.uv[6] = v.uv + _MainTex_TexelSize.xy * half2(-1, 1);
                o.uv[7] = v.uv + _MainTex_TexelSize.xy * half2(0, 1);
                o.uv[8] = v.uv + _MainTex_TexelSize.xy * half2(1, 1);

                return o;
            }

            

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv[4]);


                #if SOBEL

                const fixed Gx[9] = {
                    -1, -2, -1,
                     0,  0,  0,
                     1,  2,  1
                };
                const fixed Gy[9] = {
                    -1,  0,  1,
                    -2,  0,  2,
                    -1,  0,  1
                };

                half texColor;
                half edgeX = 0;
                half edgeY = 0;
                for (int idx = 0; idx < 9; ++idx)
                {
                    texColor = Luminance(tex2D(_MainTex, i.uv[idx]));
                    edgeX += texColor * Gx[idx];
                    edgeY += texColor * Gy[idx];
                }

                half edge = abs(edgeX) + abs(edgeY);

                fixed4 black = fixed4(0, 0, 0, 1);
                fixed4 empty = fixed4(0, 0, 0, 0);

                col = lerp(empty, black, min(col.a, edge));

                #endif

                return col;
            }
            ENDCG
        }
    }
}
