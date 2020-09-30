Shader "Hidden/UIShiny"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Factor ("Factor", Range(0, 1)) = 0.5
        _Width ("Width", Range(0, 1)) = 0.2
        _Softness ("Softness", Range(0, 1)) = 0.8
        _Brightness ("Brightness", Range(0, 1)) = 0.8
        _Gloss ("Gloss", Range(0, 1)) = 0.8
        _Rotation ("Rotation", Range(-180, 180)) = 0
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
            #include "UIEffect.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float uv2 : TEXCOORD1;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float uv2 : TEXCOORD1;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;

                // o.uv2 = sin(v.uv.x);
                o.uv2 = v.uv.x;

                return o;
            }

            sampler2D _MainTex;
            half _Factor;
            half _Width;
            half _Softness;
            half _Brightness;
            half _Gloss;
            half _Rotation;

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);

                fixed nomalizedPos = i.uv.x;
                half location = _Factor * 2 - 0.5;
                half normalized = 1 - saturate(abs((nomalizedPos - location) / _Width));
                half shinePower = smoothstep(0, _Softness, normalized);
                half3 reflectColor = lerp(fixed3(1,1,1), col.rgb * 7, _Gloss);

                col.rgb += col.a * (shinePower / 2) * _Brightness * reflectColor;

                col = fixed4(i.uv2, i.uv2, i.uv2, 1);

                return col;
            }
            ENDCG
        }
    }
}
