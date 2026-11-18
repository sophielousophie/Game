Shader "Ponyo/Level2/CoastalWater"
{
    Properties
    {
        _WaveMotion ("Water waves model animation", 2DArray) = "" {}
        _ReflectionMap ("Sky reflection", 2D) = "gray" {}
        _ReflectionRotation ("Reflection rotation", Float) = 115
        _WaveHeight ("Wave height", Range(0, 0.6)) = 0.32
        _WaveTileSize ("Wave tile width", Float) = 18
        _ShoreBounds ("Shore center and half size (XZ)", Vector) = (42.85, 0.65, 39.55, 2.05)
        _FoamColor ("Shore foam", Color) = (0.79, 0.94, 0.91, 1)
    }
    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Opaque" }
        Pass
        {
            Tags { "LightMode"="UniversalForward" }
            HLSLPROGRAM
            #pragma target 3.5
            #pragma vertex Vert
            #pragma fragment Frag
            #pragma multi_compile_fog
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            TEXTURE2D_ARRAY(_WaveMotion);
            SAMPLER(sampler_WaveMotion);
            TEXTURE2D(_ReflectionMap);
            SAMPLER(sampler_ReflectionMap);
            CBUFFER_START(UnityPerMaterial)
                float _WaveHeight;
                float _WaveTileSize;
                float4 _ShoreBounds;
                half4 _FoamColor;
                float _ReflectionRotation;
            CBUFFER_END
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS : TEXCOORD0;
                half fog : TEXCOORD1;
            };
            float ShoreDistance(float2 xz)
            {
                float2 edge = abs(xz - _ShoreBounds.xy) - _ShoreBounds.zw;
                return length(max(edge, 0)) + min(max(edge.x, edge.y), 0);
            }
            float WaveSample(float2 xz)
            {
                float2 uv = xz / max(_WaveTileSize, 0.1);
                float frame = frac(_Time.y / 16.6666667) * 127;
                float first = floor(frame);
                float a = SAMPLE_TEXTURE2D_ARRAY_LOD(_WaveMotion, sampler_WaveMotion, uv, first, 0).r;
                float b = SAMPLE_TEXTURE2D_ARRAY_LOD(_WaveMotion, sampler_WaveMotion, uv, first + 1, 0).r;
                float attenuation = lerp(0.035, 1, smoothstep(0.08, 2.4, ShoreDistance(xz)));
                return lerp(a, b, frac(frame)) * _WaveHeight * attenuation;
            }
            Varyings Vert(float3 positionOS : POSITION)
            {
                Varyings output;
                float3 positionWS = TransformObjectToWorld(positionOS);
                positionWS.y += WaveSample(positionWS.xz);
                output.positionCS = TransformWorldToHClip(positionWS);
                output.positionWS = positionWS;
                output.fog = ComputeFogFactor(output.positionCS.z);
                return output;
            }
            half4 Frag(Varyings input) : SV_Target
            {
                float2 xz = input.positionWS.xz;
                float stepSize = max(_WaveTileSize, 0.1) / 81;
                float height = WaveSample(xz);
                float dx = WaveSample(xz + float2(stepSize, 0)) - WaveSample(xz - float2(stepSize, 0));
                float dz = WaveSample(xz + float2(0, stepSize)) - WaveSample(xz - float2(0, stepSize));
                half3 normal = normalize(float3(-dx * 3.2, stepSize * 2, -dz * 3.2));
                half3 view = GetWorldSpaceNormalizeViewDir(input.positionWS);
                half3 reflected = reflect(-view, normal);
                half fresnel = pow(1 - saturate(dot(normal, view)), 4);
                float2 reflectionUV = float2(atan2(reflected.z, reflected.x) / TWO_PI + 0.5 + _ReflectionRotation / 360,
                    acos(clamp(reflected.y, -1, 1)) / PI);
                half3 sky = SAMPLE_TEXTURE2D(_ReflectionMap, sampler_ReflectionMap, reflectionUV).rgb;
                half3 deep = lerp(half3(0.006, 0.035, 0.045), half3(0.02, 0.13, 0.15), saturate(height / max(_WaveHeight, 0.001) * 0.5 + 0.5));
                half3 sea = lerp(deep, sky * 0.72, 0.30 + fresnel * 0.48);
                Light sun = GetMainLight();
                half3 halfDirection = SafeNormalize(view + sun.direction);
                half sparkle = pow(saturate(dot(normal, halfDirection)), 180) * 0.85;
                sea += sun.color * sparkle;

                float shore = ShoreDistance(xz);
                float ripple = 0.025 * sin(xz.x * 4.1 + xz.y * 5.7 - _Time.y * 1.1)
                    + 0.018 * sin(xz.x * 8.3 - xz.y * 6.2 + _Time.y * 0.8);
                float contact = smoothstep(-0.03, 0.03, shore);
                float wash = 1 - smoothstep(0.025, 0.18, abs(shore - 0.15 + ripple));
                float broken = smoothstep(-0.4, 0.7, sin(xz.x * 7 + xz.y * 6 + _Time.y * 1.5));
                float foam = contact * wash * (0.36 + broken * 0.38);
                sea = lerp(sea, _FoamColor.rgb, foam);
                return half4(MixFog(sea, input.fog), 1);
            }
            ENDHLSL
        }
    }
}
