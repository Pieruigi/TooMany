Shader "Custom/UI/RetroPixelated"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _PixelSize ("Pixel Size", Float) = 16.0
        _ColorLevels ("Color Levels", Float) = 4.0
    }
    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }
        LOD 100

        Pass
        {
            Name "Forward"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            float4 _MainTex_ST;

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            float _PixelSize;
            float _ColorLevels;

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                OUT.color = IN.color;
                return OUT;
            }

            float4 QuantizeColor(float4 col, float levels)
            {
                col.rgb = floor(col.rgb * levels) / levels;
                return col;
            }

            float4 frag(Varyings IN) : SV_Target
            {
                float2 pixelUV = IN.uv;
                float2 pixelatedUV = floor(pixelUV * _PixelSize) / _PixelSize;

                float4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, pixelatedUV) * IN.color;

                col = QuantizeColor(col, _ColorLevels);

                float alpha = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, pixelatedUV).a * IN.color.a;
                col.a = alpha;

                return col;
            }
            ENDHLSL
        }
    }
    FallBack "Sprites/Default"
}
