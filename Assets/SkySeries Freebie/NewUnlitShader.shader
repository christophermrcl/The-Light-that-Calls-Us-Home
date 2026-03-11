Shader "Custom/CubemapSkybox"
{
    Properties
    {
        _MainTex ("Skybox Cubemap", Cube) = "" {}
        _Exposure ("Exposure", Range(0,8)) = 1.0
    }
    SubShader
    {
        Tags {"Queue" = "Background" "RenderType" = "Skybox"}
        Pass
        {
            ZWrite Off
            ZTest Always
            Cull Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            samplerCUBE _MainTex; // Reference to the cubemap
            float _Exposure;      // Exposure for brightness adjustment

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Sample the cubemap texture
                fixed4 col = texCUBE(_MainTex, i.pos.xyz);
                col.rgb *= _Exposure; // Apply exposure
                return col;
            }
            ENDCG
        }
    }

    Fallback "Skybox/Procedural"
}