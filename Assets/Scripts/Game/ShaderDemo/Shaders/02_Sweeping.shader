Shader "2D Shader/02_Sweeping"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _SweepingTex ("Sweeping Texture", 2D) = "black" {}
        _Brightness ("Brightness", Range(0, 1)) = 0.5
        _Offset ("Offset", Range(-1, 1)) = 0

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

            #include "UnityCG.cginc"

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

            sampler2D _MainTex;
            sampler2D _SweepingTex;
            float _Brightness;
            float _Offset;
            
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col_src = tex2D(_MainTex, i.uv);
                // frac(x) 返回x的小数部分
                fixed4 col_sweep = tex2D(_SweepingTex, i.uv - fixed2((frac(_Time.y) * 2 - 1), 0));

                fixed4 col_out = col_src + col_sweep * col_src.a * col_sweep.a * _Brightness;

                return col_out;
            }
            ENDCG
        }
    }
}
