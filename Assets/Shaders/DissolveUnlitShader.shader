Shader "Unlit/DissolveUnlitShader"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        _MainTex("Albedo (RGB)", 2D) = "white" {}
        _DisolveTex("DisolveTex (RGB)", 2D) = "white" {}
        _Glossiness("Smoothness", Range(0,1)) = 0.5
        _Threshold("Threshold", Range(0,1)) = 0.0
        _OutLineColor("OutLineColor", Color) = (1,1,1,1)
        _OutLineSize("OutLineSize", Range(0,1)) = 0.1
        _DownValue("DownValue", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        ZTest Always

        Pass
        {
            Name "BASE" //�{�̕�����`�悷��p�X�̖��O

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
            half _OutLineSize;
            half _DownValue;
            fixed4 _Color;
            fixed4 _OutLineColor;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.vertex.y -= _DownValue;
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

                // �e�N�X�`������F���擾
                fixed4 col = tex2D(_MainTex, i.uv);

                // ��������ɃA�E�g���C���̗l�ȕ���t����
                if (_Threshold > 0 && g - _OutLineSize < _Threshold) {
                    col.r = _OutLineColor.r;
                    col.g = _OutLineColor.g;
                    col.b = _OutLineColor.b;
                }

                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
