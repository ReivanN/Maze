Shader "Hidden/Custom/ChromaticAberration"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _AberrationAmount("Aberration Amount", Range(0, 1)) = 0.02
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline" }
        LOD 100

        Pass
        {
            Name "ChromaticAberration"

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #pragma target 3.0

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            float4 _MainTex_TexelSize;
            float _AberrationAmount;

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv         : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv          : TEXCOORD0;
            };

            Varyings Vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                return OUT;
            }

            float4 Frag(Varyings IN) : SV_Target
            {
                float2 uv = IN.uv;

                // —мещени€ каналов
                float2 offset = (_AberrationAmount) * (uv - 0.5); // центрированное смещение

                float3 col;
                col.r = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + offset).r;
                col.g = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv).g;
                col.b = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv - offset).b;

                return float4(col, 1.0);
            }

            ENDHLSL
        }
    }
}
