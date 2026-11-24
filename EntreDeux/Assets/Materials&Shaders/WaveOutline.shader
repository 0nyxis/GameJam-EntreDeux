Shader "Custom/WaveOutline"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite", 2D) = "white" {}
        [HDR] _LineColor ("Line Color", Color) = (0.3, 0.7, 2, 1)
        _Reveal ("Reveal", Range(0, 1)) = 0
        _Alpha ("Alpha", Range(0, 1)) = 1
        _OutlineWidth ("Outline Width (texels)", Range(0.5, 4)) = 1
        _DashDensity ("Dash Density", Float) = 2
        _DashRatio ("Dash Ratio", Range(0.1, 0.9)) = 0.6
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
        }

        Blend SrcAlpha One   // additif : la ligne "brille"
        ZWrite Off
        Cull Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            float4 _MainTex_TexelSize;

            CBUFFER_START(UnityPerMaterial)
                float4 _LineColor;
                float _Reveal;
                float _Alpha;
                float _OutlineWidth;
                float _DashDensity;
                float _DashRatio;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float2 worldXY : TEXCOORD1;
                float4 color : COLOR;
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                float3 posWS = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.positionCS = TransformWorldToHClip(posWS);
                OUT.uv = IN.uv;
                OUT.worldXY = posWS.xy;
                OUT.color = IN.color;
                return OUT;
            }

            float Hash(float n)
            {
                return frac(sin(n * 12.9898 + 78.233) * 43758.5453);
            }

            float4 frag(Varyings IN) : SV_Target
            {
                // 1. Détection de contour (intérieur de la forme)
                float2 o = _MainTex_TexelSize.xy * _OutlineWidth;
                float center = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv).a;
                float a1 = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv + float2(o.x, 0)).a;
                float a2 = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv - float2(o.x, 0)).a;
                float a3 = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv + float2(0, o.y)).a;
                float a4 = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv - float2(0, o.y)).a;
                float edge = saturate(center - min(min(a1, a2), min(a3, a4)));

                // 2. Découpe du contour en segments (tirets)
                float coord = (IN.worldXY.x + IN.worldXY.y) * _DashDensity;
                float dashId = floor(coord);
                float dash = step(frac(coord), _DashRatio);

                // 3. Apparition segment par segment, dans un ordre aléatoire
                float visible = step(Hash(dashId), _Reveal);

                float outline = edge * dash * visible * _Alpha;

                return float4(_LineColor.rgb * IN.color.rgb, outline * _LineColor.a);
            }
            ENDHLSL
        }
    }
}