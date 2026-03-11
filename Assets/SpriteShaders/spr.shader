Shader "Custom/SpriteWithDepth"
{
    Properties
    {
        _MainTex ("Base Texture", 2D) = "white" { }
        _Color ("Tint Color", Color) = (1, 1, 1, 1)
    }

    SubShader
    {
        Tags { "Queue" = "Overlay" "RenderType" = "Opaque" }

        Pass
        {
            Name "SpritePass"
            Tags { "LightMode" = "UniversalForward" }

            // Enable depth writing and testing
            ZWrite On
            ZTest LEqual
            ColorMask RGB
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            Varyings vert(Attributes v)
            {
                Varyings o;
                o.vertex = TransformObjectToHClip(v.vertex.xyz);
                o.uv = v.uv;
                return o;
            }

            half4 frag(Varyings i) : SV_Target
            {
                half4 col = tex2D(_MainTex, i.uv) * _Color;
                return col;
            }

            ENDCG
        }
    }

    // Fallback shader for unsupported pipelines
    Fallback "Sprites/Default"
}