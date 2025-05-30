Shader "Custom/TattooOverlay"
{
    Properties
    {
        _MainTex ("Base Texture", 2D) = "white" {}
        _TattooTex ("Tattoo Texture", 2D) = "white" {}

        _TattooCenter ("Tattoo Position (UV)", Vector) = (0.5, 0.5, 0, 0)
        _TattooScale ("Tattoo Scale (XY)", Vector) = (0.2, 0.2, 0, 0)
        _TattooRotation ("Tattoo Rotation (Degrees)", Float) = 0
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            sampler2D _TattooTex;

            float4 _TattooCenter;
            float4 _TattooScale;
            float _TattooRotation;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.uv = v.uv;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 baseColor = tex2D(_MainTex, i.uv);

                float2 centeredUV = (i.uv - _TattooCenter.xy) / _TattooScale.xy;
                float angle = radians(_TattooRotation);
                float s = sin(angle);
                float c = cos(angle);

                float2 rotatedUV = float2(
                    centeredUV.x * c - centeredUV.y * s,
                    centeredUV.x * s + centeredUV.y * c
                );

                float2 tattooUV = rotatedUV + 0.5;

                //防止UV越界造成拉伸条纹
                if (tattooUV.x < 0.0 || tattooUV.x > 1.0 || tattooUV.y < 0.0 || tattooUV.y > 1.0)
                {
                    return baseColor;
                }

                fixed4 tattooColor = tex2D(_TattooTex, tattooUV);
                fixed alpha = tattooColor.a;

                fixed4 finalColor = lerp(baseColor, tattooColor, alpha);

                return finalColor;
            }

            ENDCG
        }
    }
}
