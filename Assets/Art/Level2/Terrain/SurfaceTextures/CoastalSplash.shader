Shader "Ponyo/Level2/CoastalSplash"
{
    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Transparent" "Queue"="Transparent" }
        Pass
        {
            Tags { "LightMode"="SRPDefaultUnlit" }
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                half4 color : COLOR;
            };
            Varyings Vert(float3 positionOS : POSITION, float2 uv : TEXCOORD0, half4 color : COLOR)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(positionOS);
                output.uv = uv;
                output.color = color;
                return output;
            }
            half4 Frag(Varyings input) : SV_Target
            {
                float radius = length((input.uv - 0.5) * float2(1, 0.80));
                half alpha = (1 - smoothstep(0.30, 0.5, radius)) * input.color.a;
                return half4(input.color.rgb, alpha);
            }
            ENDHLSL
        }
    }
}
