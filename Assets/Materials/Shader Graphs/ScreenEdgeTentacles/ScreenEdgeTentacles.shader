Shader "Custom/URP/ScreenEdgeTentacles"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite (unused)", 2D) = "white" {}
        _Intensity   ("Intensity", Range(0, 1)) = 0
        _Seed        ("Seed", Float) = 0
        _Speed       ("Speed", Float) = 1
        _Count       ("Tendril Count", Float) = 16
        _Width       ("Root Half-Width (screen heights)", Float) = 0.03
        _Taper       ("Taper (lower = fatter body)", Float) = 0.6
        _FingerDepth ("Max Reach Toward Center (0-1)", Float) = 0.6
        _MinLength   ("Min Length When Retracted (0-1)", Float) = 0.3
        _BaseDepth   ("Edge Band Depth (0-1)", Float) = 0.006
        _Softness    ("Edge Softness", Float) = 0.012
        _Sway        ("Sideways Sway (0 = perfectly straight)", Float) = 0.015
        _Distortion  ("Distortion", Float) = 0.05
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "RenderType" = "Transparent"
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "CanUseSpriteAtlas" = "True"
        }

        Cull Off
        ZWrite Off
        ZTest Always
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            Name "ScreenEdgeTentacles"

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareOpaqueTexture.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float _Intensity;
                float _Seed;
                float _Speed;
                float _Count;
                float _Width;
                float _Taper;
                float _FingerDepth;
                float _MinLength;
                float _BaseDepth;
                float _Softness;
                float _Sway;
                float _Distortion;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float4 color      : COLOR;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float4 color      : COLOR;
                float4 screenPos  : TEXCOORD0;
            };

            // Per-tendril random value in 0-1. idx = tendril index, salt = which property.
            float Rand(float idx, float salt)
            {
                return frac(sin(dot(float2(idx, salt + _Seed * 0.137), float2(127.1, 311.7))) * 43758.5453);
            }

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.color = IN.color;

                float4 o = OUT.positionCS * 0.5;
                o.xy = float2(o.x, o.y * _ProjectionParams.x) + o.w;
                o.zw = OUT.positionCS.zw;
                OUT.screenPos = o;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                if (_Intensity <= 0.0001) return half4(0, 0, 0, 0);

                float2 suv = IN.screenPos.xy / IN.screenPos.w;
                float2 q = (suv - 0.5) * 2.0;                 // -1..1 on both axes
                float aspect = _ScreenParams.x / _ScreenParams.y;
                float hx = aspect * 0.5;                      // half-extents in "screen height" units
                float hy = 0.5;
                float t = _Time.y * _Speed;

                // rho = 1 exactly on the screen border (all four sides), 0 at the centre
                float rho = max(abs(q.x), abs(q.y));

                // Where the ray from the centre through this pixel hits the border
                float2 bq = q / max(rho, 1e-5);

                // Position along the border perimeter (clockwise from top-left), in screen-height units
                float s;
                if (abs(bq.y) >= abs(bq.x))
                    s = (bq.y > 0.0) ? (bq.x + 1.0) * hx : (2.0 * hx + 2.0 * hy + (1.0 - bq.x) * hx);
                else
                    s = (bq.x > 0.0) ? (2.0 * hx + (1.0 - bq.y) * hy) : (4.0 * hx + 2.0 * hy + (bq.y + 1.0) * hy);

                float perimeter = 4.0 * (hx + hy);
                float N = max(3.0, floor(_Count + 0.5));
                float cell = perimeter / N;
                float kc = floor(s / cell);

                float tendril = 0.0;
                float halo = 0.0;

                [unroll]
                for (int o = -1; o <= 1; o++)
                {
                    float k = kc + o;                         // unwrapped index, used for position
                    float idx = k - N * floor(k / N);         // wrapped index, used for random values

                    float jitter = (Rand(idx, 1.0) - 0.5) * 0.6;
                    float lenR   = lerp(0.4, 1.0, Rand(idx, 2.0));
                    float widR   = lerp(0.7, 1.3, Rand(idx, 3.0));
                    float ph1    = Rand(idx, 4.0) * 6.2832;
                    float ph2    = Rand(idx, 5.0) * 6.2832;
                    float ph3    = Rand(idx, 6.0) * 6.2832;
                    float w1     = lerp(0.15, 0.45, Rand(idx, 7.0));
                    float w2     = lerp(0.35, 1.00, Rand(idx, 8.0));
                    float swSpd  = lerp(0.8, 2.0, Rand(idx, 9.0));

                    float si = (k + 0.5 + jitter) * cell;     // this tendril's anchor on the border

                    // Each tendril extends and retracts on its own two-sine rhythm
                    float anim = 0.5 + 0.5 * (0.65 * sin(t * w1 * 6.2832 + ph1) + 0.35 * sin(t * w2 * 6.2832 + ph2));
                    float reach = _Intensity * _FingerDepth * lenR * lerp(_MinLength, 1.0, anim);
                    float rhoTip = 1.0 - reach;

                    // u: 1 at the border (root), 0 at the tip
                    float u = saturate((rho - rhoTip) / max(reach, 1e-4));
                    float depthFrac = 1.0 - u;

                    // Optional small sideways wobble, zero at the root, strongest at the tip
                    float sway = _Sway * depthFrac * sin(depthFrac * 5.0 + t * swSpd * 3.1416 + ph3);

                    // Distance from the tendril's straight radial axis, shrinking toward the centre
                    float lat = abs((s - si) * rho - sway);

                    float halfW = _Width * widR * pow(max(u, 1e-5), _Taper);
                    float feather = max(_Softness * 0.1, 1e-4);
                    float inside = step(rhoTip, rho);         // nothing beyond the tip

                    float m = saturate((halfW - lat) / feather) * inside;
                    float h = saturate((halfW * 2.2 - lat) / (feather * 6.0 + halfW)) * inside;

                    tendril = max(tendril, m);
                    halo = max(halo, h);
                }

                // Thin black band hugging the screen border
                float baseT = 1.0 - _BaseDepth * _Intensity;
                float baseMask = smoothstep(baseT, baseT + 0.04, rho);
                float baseHalo = smoothstep(baseT - 0.08, baseT, rho);

                float mask = max(baseMask, tendril);
                float warp = max(halo, baseHalo);

                // Distortion: sample from slightly further out, so the picture is pulled toward the centre
                float2 dirp = normalize(float2(q.x * aspect, q.y) + 1e-5);
                float2 offsetUV = dirp * float2(1.0 / aspect, 1.0) * (warp * _Distortion * _Intensity);
                float2 warpedUV = saturate(suv + offsetUV);

                half3 scene = SampleSceneColor(warpedUV);
                half3 col = lerp(scene, half3(0, 0, 0), mask);

                // Only cover pixels we changed, so transparents drawn earlier survive elsewhere
                float alpha = saturate(max(mask, warp)) * IN.color.a;
                return half4(col, alpha);
            }
            ENDHLSL
        }
    }
}
