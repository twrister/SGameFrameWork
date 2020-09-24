Shader "Hidden/UIBlur"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_Factor("Factor", Range(0, 1)) = 0
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

            #pragma multi_compile __ GAUSSIAN_3 GAUSSIAN_5 GAUSSIAN_7 GAUSSIAN_9 GAUSSIAN_13 GAUSSIAN_21

            #include "UnityCG.cginc"
            #include "UIEffect.cginc"

            sampler2D _MainTex;
            half4 _MainTex_TexelSize;
            half _Factor;

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

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);

                #if GAUSSIAN_3 | GAUSSIAN_5 | GAUSSIAN_7 | GAUSSIAN_9 | GAUSSIAN_13 | GAUSSIAN_21

                #if GAUSSIAN_3
                const int KERNEL_SIZE = 3;
                const float KERNEL_[3] = { 0.4566, 1.0, 0.4566};
                #elif GAUSSIAN_5
                const int KERNEL_SIZE = 5;
                const float KERNEL_[5] = { 0.2486, 0.7046, 1.0, 0.7046, 0.2486};
                #elif GAUSSIAN_7
                const int KERNEL_SIZE = 7;
                const float KERNEL_[7] = { 0.1719, 0.4566, 0.8204, 1.0, 0.8204, 0.4566, 0.1719};
                #elif GAUSSIAN_9
                const int KERNEL_SIZE = 9;
                const float KERNEL_[9] = { 0.0438, 0.1719, 0.4566, 0.8204, 1.0, 0.8204, 0.4566, 0.1719, 0.0438};
                #elif GAUSSIAN_13
                const int KERNEL_SIZE = 13;
                const float KERNEL_[13] = { 0.0438, 0.1138, 0.2486, 0.4566, 0.7046, 0.9141, 1.0, 0.9141, 0.7046, 0.4566, 0.2486, 0.1138, 0.0438};
                #elif GAUSSIAN_21
                const int KERNEL_SIZE = 21;
                const float KERNEL_[21] = {0.0432, 0.0785, 0.1340, 0.2146, 0.3228, 0.4560, 0.6049, 0.7537, 0.8818, 0.9689, 1.0, 0.9689, 0.8818, 0.7537, 0.6049, 0.4560, 0.3228, 0.2146, 0.1340, 0.0785, 0.0432};
                #endif

                sampler2D tex = _MainTex;
                half2 texcood = i.uv;
                half2 blur = _MainTex_TexelSize.xy * 2;

                float4 o = 0;
                float sum = 0;
                float2 shift = 0;
                for(int x = 0; x < KERNEL_SIZE; x++)
                {
                    shift.x = blur.x * (float(x) - KERNEL_SIZE/2);
                    for(int y = 0; y < KERNEL_SIZE; y++)
                    {
                        shift.y = blur.y * (float(y) - KERNEL_SIZE/2);
                        float2 uv = texcood + shift;
                        float weight = KERNEL_[x] * KERNEL_[y];
                        sum += weight;
                        o += tex2D(tex, uv) * weight;
                    }
                }
                col = o / sum;

                #endif


                return col;
            }
            ENDCG
        }
    }
}
