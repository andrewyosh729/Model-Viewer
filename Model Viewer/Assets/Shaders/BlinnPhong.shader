Shader "Custom/BlinnPhong"
{
    Properties
    {
        _AmbientColor ("Ambient Color", Color) = (0.1,0.1,0.1,1)
        _SpecularColor ("Specular Color", Color) = (0.5,0.5,0.5,1)
        _DiffuseColor ("Diffuse Color", Color) = (0.6,0.6,0.6,1)
        _Shininess ("Shininess", Range(1,250)) = 32
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

            float4 _AmbientColor;
            float4 _SpecularColor;
            float4 _DiffuseColor;
            float _Shininess;

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

            half4 frag(Varyings IN) : SV_Target
            {
                float3 color = _AmbientColor;
                int additionalLightCount = GetAdditionalLightsCount();

                for (int i = 0; i < additionalLightCount; ++i)
                {
                    Light light = GetAdditionalLight(i, float4(IN.worldPos, 1.0));
                    float3 normal = normalize(IN.normalWS);
                    float3 lightDirection = normalize(light.direction);
                    float lambertian = max(dot(lightDirection, normal), 0.0);
                    float specular = 0.0;

                    if (lambertian > 0.0)
                    {
                        // blinn phong
                        float3 viewDirection = normalize(IN.viewDirWS);
                        float3 halfDirection = normalize(lightDirection + viewDirection);
                        float specularAngle = max(dot(halfDirection, normal), 0.0);
                        specular = pow(specularAngle, _Shininess);
                    }
                    color += light.color * light.distanceAttenuation * (_DiffuseColor * lambertian + _SpecularColor * specular);
                }


                return float4(color, 1.0);
            }
            ENDHLSL
        }
    }
}