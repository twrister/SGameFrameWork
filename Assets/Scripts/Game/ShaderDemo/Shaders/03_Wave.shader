Shader "2D Shader/03_Wave"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Amount ("Amount", Range(0, 0.4)) = 0.2               // 振幅
        _DampRadius ("Damp Radius", Range(0, 1)) = 0.5         // 衰减半径
        _Frequency ("Frequency", Range(0, 100)) = 50        // 频率
        _Speed ("Speed", Range(0, 50)) = 20                // 速度
        _CenterPos ("CenterPos", Vector) = (0, 0, 0, 0)
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
            float _Amount;
            float _DampRadius;
            float _Frequency;
            float _Speed;
            float4 _CenterPos;

            fixed4 frag (v2f i) : SV_Target
            {
                // fixed4 col = tex2D(_MainTex, i.uv);

                // fixed2 center_uv = {0.5, 0.25};
                fixed2 dt = _CenterPos.xy - i.uv;
                // 像素距离中心点距离
                float len = sqrt(dot(dt, dt));
                // 衰减
                float amount = _Amount * max(-abs(len) + _DampRadius, 0);

                i.uv.y += amount * cos((len + (_Time.y / 100) * _Speed) * _Frequency * UNITY_PI);

                fixed4 col = tex2D(_MainTex, i.uv);

                return col;
            }
            ENDCG
        }
    }
}
