Shader "Custom/URP/Ooze"
{
    Properties
    {
        [PerRendererData] _MainTex ("Blob Sprite", 2D) = "white" {}
        _Progress ("Progress", Range(0, 1)) = 0
        _OozeColor ("Ooze Color", Color) = (0.05, 0.12, 0.05, 1)
        _EdgeColor ("Edge Color", Color) = (0.35, 0.6, 0.25, 1)
        _NoiseScale ("Noise Scale", Float) = 5
        _EdgeWidth ("Edge Width", Float) = 0.08
        _GravityBias ("Gravity Bias", Float) = 0.5
        _BoundsMin ("Sprite Bounds Min (xy)", Vector) = (0, 0, 0, 0)
        _Size ("Sprite Bounds Size (xy)", Vector) = (1, 1, 0, 0)
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
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

            float _Progress;
            float4 _OozeColor;
            float4 _EdgeColor;
            float _NoiseScale;
            float _EdgeWidth;
            float _GravityBias;
            float4 _BoundsMin;
            float4 _Size;

            struct Attributes
            {
                float4 positionOS : POSITION;
                float4 color      : COLOR;
                float2 uv         : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float4 color      : COLOR;
                float2 uv         : TEXCOORD0;
                float2 posOS      : TEXCOORD1;
            };

            Varyings vert(Attributes v)
            {
                Varyings o;
                o.positionCS = TransformObjectToHClip(v.positionOS.xyz);
                o.color = v.color;
                o.uv = v.uv;
                o.posOS = v.positionOS.xy;
                return o;
            }

            float2 GradDir(float2 p)
            {
                p = fmod(p, 289.0);
                float x = fmod((34.0 * p.x + 1.0) * p.x, 289.0) + p.y;
                x = fmod((34.0 * x + 1.0) * x, 289.0);
                x = frac(x / 41.0) * 2.0 - 1.0;
                return normalize(float2(x - floor(x + 0.5), abs(x) - 0.5));
            }

            float GradNoise(float2 p)
            {
                float2 ip = floor(p);
                float2 fp = frac(p);
                float d00 = dot(GradDir(ip), fp);
                float d01 = dot(GradDir(ip + float2(0, 1)), fp - float2(0, 1));
                float d10 = dot(GradDir(ip + float2(1, 0)), fp - float2(1, 0));
                float d11 = dot(GradDir(ip + float2(1, 1)), fp - float2(1, 1));
                fp = fp * fp * fp * (fp * (fp * 6.0 - 15.0) + 10.0);
                return lerp(lerp(d00, d10, fp.x), lerp(d01, d11, fp.x), fp.y) + 0.5;
            }

            half4 frag(Varyings i) : SV_Target
            {
                float2 uv01 = (i.posOS - _BoundsMin.xy) / max(_Size.xy, 0.0001);
                float blob = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv).a;

                float t = _Time.y;
                float2 nuv = uv01 * _NoiseScale + float2(0.0, t * 0.03);
                float noise = GradNoise(nuv);

                float2 duv = float2(uv01.x * 12.0, uv01.y * 1.5 + t * 0.03);
                float drip = GradNoise(duv) * saturate(0.5 - uv01.y);

                float gravity = (0.5 - uv01.y) * _GravityBias;
                float value = blob + (noise - 0.5) * 0.6 + drip * 0.3 + gravity;

                float threshold = lerp(1.8, -0.6, _Progress);
                float mass = smoothstep(threshold, threshold + 0.03, value);
                float band = (1.0 - smoothstep(threshold, threshold + _EdgeWidth, value)) * mass;
                float clipMask = saturate(blob * 4.0);

                float alpha = mass * clipMask * _OozeColor.a * i.color.a;
                float3 col = lerp(_OozeColor.rgb, _EdgeColor.rgb, band) * i.color.rgb;

                return half4(col, alpha);
            }
            ENDHLSL
        }
    }
}
