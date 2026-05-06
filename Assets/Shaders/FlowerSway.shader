Shader "Custom/FlowerSway"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _SwayAmount ("Sway Amount", Range(0, 30)) = 10
        _SwaySpeed ("Sway Speed", Range(0, 5)) = 2
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            float _SwayAmount;
            float _SwaySpeed;

            UNITY_INSTANCING_BUFFER_START(Props)
                UNITY_DEFINE_INSTANCED_PROP(float, _RandomOffset)
            UNITY_INSTANCING_BUFFER_END(Props)

            v2f vert (appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);

                // Get per-instance random offset (value between 0 and 100)
                float offset = UNITY_ACCESS_INSTANCED_PROP(Props, _RandomOffset);

                // Sway angle: sin(time * speed + offset)
                float angle = sin(_Time.y * _SwaySpeed + offset) * radians(_SwayAmount);

                // Rotate around Y axis (sway left/right)
                float s, c;
                sincos(angle, s, c);
                float4x4 rot = float4x4(
                    c, 0, s, 0,
                    0, 1, 0, 0,
                   -s, 0, c, 0,
                    0, 0, 0, 1
                );

                float4 worldPos = mul(rot, v.vertex);
                o.vertex = UnityObjectToClipPos(worldPos);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(i);
                fixed4 col = tex2D(_MainTex, i.uv) * _Color;
                return col;
            }
            ENDCG
        }
    }
}