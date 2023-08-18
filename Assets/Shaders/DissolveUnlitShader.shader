Shader "Unlit/DissolveUnlitShader"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        _MainTex("Albedo (RGB)", 2D) = "white" {}
        _DisolveTex("DisolveTex (RGB)", 2D) = "white" {}
        _Glossiness("Smoothness", Range(0,1)) = 0.5
        _Threshold("Threshold", Range(0,1)) = 0.0
        _OutLine("OutLine", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _DisolveTex;

            half _Glossiness;
            half _Threshold;
            half _OutLine;
            fixed4 _Color;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 m = tex2D(_DisolveTex, i.uv);
                half g = m.r * 0.2 + m.g * 0.7 + m.b * 0.1;
                if (g < _Threshold) {
                    discard;
                }

                // テクスチャから色を取得
                fixed4 col = tex2D(_MainTex, i.uv);

                // 消える線にアウトラインの様な物を付ける
                if (_Threshold > 0 && g - .1f < _Threshold) {
                    col.r = _OutLine;
                    col.g = _OutLine;
                    col.b = _OutLine;
                }

                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
