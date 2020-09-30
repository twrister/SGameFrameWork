Shader "Hidden/UITransform"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}

        _OffsetScale ("Offset", Vector) = (0, 0, 1, 1)
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
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            half4 _OffsetScale;
            half _Rotation;

            float4 CalculateRotation(float4 pos)
            {
                float s,c;
                sincos(radians(_Rotation), s, c);
                float2x2 rotMatrix = float2x2(c, -s, s, c);
                pos.xy = mul(pos.xy, rotMatrix);
                
                return pos;

            }

            v2f vert (appdata v)
            {
                v2f o;
                // o.vertex = UnityObjectToClipPos(v.vertex);

                float4 k = float4(v.vertex.x, v.vertex.y, 1.0, 1.0);
                k = CalculateRotation(k);
                o.vertex = UnityObjectToClipPos(k);
                // float4 ver = float4(k.x, v.vertex.y, k.y, v.vertex.w);

                // float4 ver = UnityObjectToClipPos(v.vertex);
                // o.vertex = CalculateRotation(ver);

                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);



                return col;
            }
            ENDCG
        }
    }
}
