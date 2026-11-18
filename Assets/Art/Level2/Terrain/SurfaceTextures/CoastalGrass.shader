Shader "Ponyo/Level2/CoastalGrass"
{
    Properties
    {
        _WindStrength ("Wind strength", Range(0, 0.06)) = 0.045
        _WindSpeed ("Wind speed", Range(0, 3)) = 1.35
    }

    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Opaque" }
        Cull Off

        Pass
        {
            Tags { "LightMode"="SRPDefaultUnlit" }
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #pragma multi_compile_fog
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float _WindStrength;
                float _WindSpeed;
            CBUFFER_END

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                half3 color : COLOR;
                half fog : TEXCOORD0;
            };

            Varyings Vert(float3 positionOS : POSITION, float2 uv : TEXCOORD0, half4 color : COLOR)
            {
                float3 positionWS = TransformObjectToWorld(positionOS);
                float phase = dot(positionWS.xz, float2(1.25, 1.7)) - _Time.y * _WindSpeed;
                float sway = sin(phase) * 0.7 + sin(phase * 1.9) * 0.3;
                // The response grows toward the tip while the root remains fixed.
                positionWS.xz += float2(1.0, 0.32) * (sway * _WindStrength * uv.y * uv.y);

                Varyings output;
                output.positionCS = TransformWorldToHClip(positionWS);
                output.fog = ComputeFogFactor(output.positionCS.z);
                output.color = color.rgb;
                #ifndef UNITY_COLORSPACE_GAMMA
                    output.color = SRGBToLinear(output.color);
                #endif
                return output;
            }

            half4 Frag(Varyings input) : SV_Target
            {
                return half4(MixFog(input.color, input.fog), 1);
            }
            ENDHLSL
        }
    }
}
