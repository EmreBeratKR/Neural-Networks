Shader "Custom/ClassificationShader/2D-test"
{
    Properties
    {
        
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

            #include "UnityCG.cginc"

            struct Sample
            {
                float2 pos;
                float3 color;
            };
            StructuredBuffer<Sample> _Samples;
            int _SampleCount;
            sampler2D _DecisionTex;

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
                float padding = 0.05;
                
                float2 uv = i.uv * (1.0 + 2 * padding) - padding;
                float4 bg = tex2D(_DecisionTex, uv);
                float4 col = float4(1,1,1,1);
                [loop]
                for (int idx = 0; idx < _SampleCount; idx++)
                {
                    float radius = 0.01;
                    float2 diff = uv - _Samples[idx].pos;
                    if (length(diff) < radius)
                        col.xyz = _Samples[idx].color;
                }

                if (col.x == 1 && col.y == 1 && col.z == 1)
                {
                    return bg * 0.2;
                }
                
                return col;
            }
            ENDCG
        }
    }
}
