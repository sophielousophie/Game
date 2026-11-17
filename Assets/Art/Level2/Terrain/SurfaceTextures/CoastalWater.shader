Shader "Ponyo/Level2/CoastalWater"
{
    Properties
    {
        [MainTexture] _BaseMap ("Water texture", 2D) = "white" {}
        _WaterColor ("Sea color", Color) = (0.15, 0.62, 0.78, 1)
        _TextureStrength ("Texture strength", Range(0, 1)) = 0.68
        _TileSize ("Tile width and depth", Vector) = (5, 9, 0, 0)
        _ShoreBounds ("Shore center and half size (XZ)", Vector) = (32.11, 0.65, 31.89, 2.05)
        _FoamColor ("Shore foam", Color) = (0.79, 0.94, 0.91, 1)
    }

    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Opaque" }

        Pass
        {
            Tags { "LightMode"="UniversalForward" }
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #pragma multi_compile_fog
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);
            CBUFFER_START(UnityPerMaterial)
                float4 _BaseMap_ST;
                half4 _WaterColor;
                half _TextureStrength;
                float4 _TileSize;
                float4 _ShoreBounds;
                half4 _FoamColor;
            CBUFFER_END

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS : TEXCOORD0;
                half fog : TEXCOORD1;
            };

            Varyings Vert(float3 positionOS : POSITION)
            {
                VertexPositionInputs position = GetVertexPositionInputs(positionOS);
                Varyings output;
                output.positionCS = position.positionCS;
                // Adjacent hexagons sample the same image at the same world position.
                output.positionWS = position.positionWS;
                output.fog = ComputeFogFactor(position.positionCS.z);
                return output;
            }

            half4 Frag(Varyings input) : SV_Target
            {
                float2 xz = input.positionWS.xz;
                float2 uv = xz / max(_TileSize.xy, 0.01);
                half3 textureColor = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, uv).rgb;
                half3 sea = lerp(_WaterColor.rgb, textureColor, _TextureStrength);

                // Signed distance to the common footprint of the stair foundation and soil.
                float2 edge = abs(xz - _ShoreBounds.xy) - _ShoreBounds.zw;
                float shore = length(max(edge, 0)) + min(max(edge.x, edge.y), 0);
                float ripple = 0.035 * sin(xz.x * 4.1 + xz.y * 5.7)
                    + 0.027 * sin(xz.x * 9.3 - xz.y * 7.2);
                float contact = smoothstep(-0.03, 0.035, shore);
                float shallow = contact * (1 - smoothstep(0.08, 0.85, shore));
                float foam = contact * (1 - smoothstep(0.055, 0.23, abs(shore - 0.20 + ripple)));
                float flecks = smoothstep(0.27, 0.68, textureColor.g);
                float outerWash = 1 - smoothstep(0.025, 0.11, abs(shore - 0.57 - ripple * 2));
                sea = lerp(sea, half3(0.07, 0.40, 0.44), shallow * 0.40);
                sea = lerp(sea, _FoamColor.rgb, foam * (0.27 + 0.42 * flecks) + outerWash * flecks * 0.17);
                return half4(MixFog(sea, input.fog), 1);
            }
            ENDHLSL
        }
    }
}
