Shader "Custom/BlendTextures"
{
    Properties
    {
        _MainTex("Base Texture", 2D) = "white" {}
        _BlendTex("Blend Texture", 2D) = "white" {}
    }
        SubShader
    {
        Tags { "RenderType" = "Opaque" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            sampler2D _BlendTex;

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 baseColor = tex2D(_MainTex, i.uv);
                fixed4 blendColor = tex2D(_BlendTex, i.uv);

                // Only blend where the blend texture is not completely transparent
                float alphaThreshold = 0.01; // Adjust this threshold as needed
                fixed4 result = baseColor;

                if (blendColor.a > alphaThreshold)
                {
                    result = blendColor;
                }

                return result;
            }
            ENDCG
        }
    }
        FallBack "Diffuse"
}
