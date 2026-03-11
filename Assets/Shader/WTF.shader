Shader "Custom/SceneTransition"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Cutoff ("Cutoff", Range(0, 1)) = 0.5
        _Ease ("Ease", Range(0.1, 10.0)) = 1.0
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float _Cutoff;
            float _Ease;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Sample the texture
                fixed4 texColor = tex2D(_MainTex, i.uv);
                float pattern = texColor.r; // Grayscale value of the texture (R = G = B)

                // Determine the alpha based on the cutoff and texture pattern
                float alpha = (_Cutoff > 0.0) ? step(pattern, _Cutoff) : 0.0;

                // Output black where the pattern dictates, transparent elsewhere
                fixed4 outputColor = fixed4(0.0, 0.0, 0.0, alpha);

                return outputColor;
            }
            ENDCG
        }
    }
}