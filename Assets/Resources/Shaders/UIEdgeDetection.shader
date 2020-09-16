Shader "Hidden/UIEdgeDetection"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _EdgeColor ("Edge Color", Color) = (0, 0, 0, 1)
        _EdgeWidth ("Edge Width", Range(0, 2)) = 0
        [Toggle] _BgToggle ("Bg Toggle", Float) = 0
        _BgAlpha ("Bg Alpha", Range(0, 1)) = 1
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
            half4 _MainTex_TexelSize;
            half4 _EdgeColor;
            half _EdgeWidth;
            fixed _BgToggle;
            half _BgAlpha;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);

                o.uv = v.uv;

                return o;
            }

            // Sobel masks (see http://en.wikipedia.org/wiki/Sobel_operator)
            //        1  0 -1     -1 -2 -1
            //    X = 2  0 -2  Y = 0  0  0
            //        1  0 -1      1  2  1
            half sobel(half2 center, half2 step)
            {
                half bottomLeft = Luminance(tex2D(_MainTex, center + half2(-step.x, -step.y)));
                half midBottom = Luminance(tex2D(_MainTex, center + half2(0, step.y)));
                half bottomRight = Luminance(tex2D(_MainTex, center + half2(step.x, -step.y)));
                half midLeft = Luminance(tex2D(_MainTex, center + half2(-step.x, 0)));
                half midRight = Luminance(tex2D(_MainTex, center + half2(step.x, 0)));
                half topLeft = Luminance(tex2D(_MainTex, center + half2(-step.x, step.y)));
                half midTop = Luminance(tex2D(_MainTex, center + half2(0, step.y)));
                half topRight = Luminance(tex2D(_MainTex, center + half2(step.x, step.y)));

                half Gx = topLeft + 2.0 * midLeft + bottomLeft - topRight - 2.0 * midRight - bottomRight;
                half Gy = -topLeft - 2.0 * midTop - topRight + bottomLeft + 2.0 * midBottom + bottomRight;

                half edge = sqrt((Gx * Gx) + (Gy * Gy));
                // half edge = abs(Gx) + abs(Gy);          // 用绝对值操作代替开根号，可优化性能 

                return edge;
            }

            // Roberts Operator
            //X = -1   0      Y = 0  -1
            //     0   1          1   0
            half roberts(half2 center, half2 step)
            {
                // get samples around pixel
                half topLeft = Luminance(tex2D(_MainTex, center + half2(-step.x, step.y)));
                half bottomLeft = Luminance(tex2D(_MainTex, center + half2(-step.x, -step.y)));
                half topRight = Luminance(tex2D(_MainTex, center + half2(step.x, step.y)));
                half bottomRight = Luminance(tex2D(_MainTex, center + half2(step.x, -step.y)));

                half Gx = -1.0 * topLeft + 1.0 * bottomRight;
                half Gy = -1.0 * topRight + 1.0 * bottomLeft;
                
                half edge = sqrt((Gx * Gx) + (Gy * Gy));
                return edge;
            }

            // scharr masks ( http://en.wikipedia.org/wiki/Sobel_operator#Alternative_operators)
            //        3  0 -3        -3 -10 -3
            //    X = 10 0 -10   Y =  0  0   0
            //        3  0 -3         3  10  3
            half scharr(half2 center, half2 step)
            {
                half bottomLeft = Luminance(tex2D(_MainTex, center + half2(-step.x, -step.y)));
                half midBottom = Luminance(tex2D(_MainTex, center + half2(0, step.y)));
                half bottomRight = Luminance(tex2D(_MainTex, center + half2(step.x, -step.y)));
                half midLeft = Luminance(tex2D(_MainTex, center + half2(-step.x, 0)));
                half midRight = Luminance(tex2D(_MainTex, center + half2(step.x, 0)));
                half topLeft = Luminance(tex2D(_MainTex, center + half2(-step.x, step.y)));
                half midTop = Luminance(tex2D(_MainTex, center + half2(0, step.y)));
                half topRight = Luminance(tex2D(_MainTex, center + half2(step.x, step.y)));

                half Gx = 3.0 * topLeft + 10.0 * midLeft + 3.0 * bottomLeft - 3.0 * topRight - 10.0 * midRight - 3.0 * bottomRight;
                half Gy = -3.0 * topLeft - 10.0 * midTop - 3.0 * topRight + 3.0 * bottomLeft + 10.0 * midBottom + 3.0 * bottomRight;

                half edge = sqrt((Gx * Gx) + (Gy * Gy));
                // half edge = abs(Gx) + abs(Gy);          // 用绝对值操作代替开根号，可优化性能 

                return edge;
            }
            

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);

                #if SOBEL
                half edge = sobel(i.uv, _MainTex_TexelSize.xy * _EdgeWidth);

                #elif ROBERTS
                half edge = roberts(i.uv, _MainTex_TexelSize.xy * _EdgeWidth);

                #elif SCHARR
                half edge = scharr(i.uv, _MainTex_TexelSize.xy * _EdgeWidth);

                #endif

                #if SOBEL | ROBERTS | SCHARR

                fixed4 bg = lerp(fixed4(0,0,0,0), lerp(fixed4(col.rgb, 0), col, _BgAlpha), _BgToggle);
                col = lerp(bg, _EdgeColor, min(col.a, edge));

                #endif

                return col;
            }
            ENDCG
        }
    }
}
