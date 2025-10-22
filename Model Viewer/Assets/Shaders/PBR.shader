Shader "Custom/PBR"
{
    Properties
    {
        _Albedo ("Albedo", Color) = (1,1,1,1)
        _Metallicness ("Metallicness", float) = 0
        _Roughness ("Roughness", float) = 0.3
        _Ao ("ao", float) = 1
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline"
        }
        LOD 200

        Pass
        {
            Tags
            {
                "LightMode"="UniversalForward"
            }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"


            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS: NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 normalWS : TEXCOORD0;
                float3 viewDirWS : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
            };

            float4 _Albedo;
            float _Metallicness;
            float _Roughness;
            float _Ao;

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                float3 positionWS = TransformObjectToWorld(IN.positionOS);
                OUT.positionHCS = TransformWorldToHClip(positionWS.xyz);
                OUT.normalWS = normalize(TransformObjectToWorldNormal(IN.normalOS));
                OUT.viewDirWS = normalize(_WorldSpaceCameraPos - positionWS.xyz);
                OUT.worldPos = positionWS.xyz;
                return OUT;
            }

            float3 FresnelSchlick(float cosTheta, float3 f0)
            {
                return f0 + (1 - f0) * pow(saturate(1 - cosTheta), 5);
            }

            float DistributionGgx(float3 n, float3 h, float roughness)
            {
                float a = pow(roughness, 2);
                float a2 = pow(a, 2);
                float nDotH = max(dot(n, h), 0);
                float nDotH2 = pow(nDotH, 2);

                float denominator = (nDotH2 * (a2 - 1.0) + 1.0);
                denominator = PI * pow(denominator, 2);
                return a2 / denominator;
            }

            float GeometrySchlickGgx(float nDotV, float roughness)
            {
                float r = roughness + 1;
                float k = pow(r, 2) / 8.0f;

                float denominator = nDotV * (1 - k) + k;

                return nDotV / denominator;
            }

            float GeometrySmith(float3 n, float3 v, float3 l, float roughness)
            {
                float nDotV = max(dot(n, v), 0);
                float nDotL = max(dot(n, l), 0);
                float ggx2 = GeometrySchlickGgx(nDotV, roughness);
                float ggx1 = GeometrySchlickGgx(nDotL, roughness);
                return ggx1 * ggx2;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float3 n = normalize(IN.normalWS);
                float3 v = normalize(IN.viewDirWS);

                float3 lo = float3(0.03, 0.03, 0.03) * _Albedo * _Ao; // ambient

                int additionalLightCount = GetAdditionalLightsCount();

                for (int i = 0; i < additionalLightCount; ++i)
                {
                    Light light = GetAdditionalLight(i, float4(IN.worldPos, 1.0));
                    float3 l = normalize(light.direction);
                    float3 h = normalize(v + l);
                    float3 radiance = light.color * light.distanceAttenuation;

                    float3 f0 = float3(0.04, 0.04, 0.04);
                    f0 = lerp(f0, _Albedo, _Metallicness);
                    float3 f = FresnelSchlick(max(dot(h, v), 0), f0);
                    float ndf = DistributionGgx(n, h, _Roughness);
                    float g = GeometrySmith(n, v, l, _Roughness);

                    float3 numerator = ndf * f * g;
                    float denominator = 4.0 * max(dot(n, v), 0) * max(dot(n, l), 0) + Eps_float();
                    float3 specular = numerator / denominator;

                    float kS = f;
                    float kD = float3(1, 1, 1) - kS;
                    kD *= 1 - _Metallicness;

                    float nDotL = max(dot(n, l), 0);
                    lo += (kD * (_Albedo / PI) + specular) * radiance * nDotL;
                }



                lo = lo / (lo + float3(1,1,1));
                float x = 1.0/2.2;
                lo = pow(lo, float3(x,x,x));
                
                return float4(lo, 1.0);
            }
            ENDHLSL
        }
    }
}