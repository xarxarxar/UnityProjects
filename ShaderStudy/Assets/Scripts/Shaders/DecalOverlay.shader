Shader "Custom/SkinLikeShaderWithDecal"
{
    Properties
    {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _SkinColor ("Skin Tint", Color) = (1, 0.8, 0.7, 1)

        _SSSColor ("Subsurface Color", Color) = (1, 0.5, 0.4, 1)
        _SSSStrength ("SSS Strength", Range(0, 1)) = 0.3

        _Glossiness ("Smoothness", Range(0,1)) = 0.5

        _DecalTex ("Decal Texture", 2D) = "white" {}
        _DecalColor ("Decal Tint", Color) = (1, 1, 1, 1)
        _DecalCenter ("Decal Center (UV)", Vector) = (0.5, 0.5, 0, 0)
        _DecalScale ("Decal Scale (UV)", Vector) = (0.2, 0.2, 0, 0)
        _DecalRotation ("Decal Rotation (Degrees)", Float) = 0
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 300

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows

        sampler2D _MainTex;
        fixed4 _SkinColor;

        sampler2D _DecalTex;
        fixed4 _DecalColor;
        float4 _DecalCenter;
        float4 _DecalScale;
        float _DecalRotation;

        fixed4 _SSSColor;
        float _SSSStrength;
        half _Glossiness;

        struct Input
        {
            float2 uv_MainTex;
            float3 viewDir;
        };

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
            fixed3 baseColor = tex.rgb * _SkinColor.rgb;

            // === 贴花 UV 运算 ===
            float2 centeredUV = (IN.uv_MainTex - _DecalCenter.xy) / _DecalScale.xy;

            float angle = radians(_DecalRotation);
            float s = sin(angle);
            float c = cos(angle);
            float2 rotatedUV = float2(
                centeredUV.x * c - centeredUV.y * s,
                centeredUV.x * s + centeredUV.y * c
            );

            float2 decalUV = rotatedUV + 0.5;

            // 判断是否在贴花范围内
            if (decalUV.x >= 0.0 && decalUV.x <= 1.0 && decalUV.y >= 0.0 && decalUV.y <= 1.0)
            {
                fixed4 decalCol = tex2D(_DecalTex, decalUV) * _DecalColor;
                baseColor = lerp(baseColor, decalCol.rgb, decalCol.a);
            }

            // SSS 模拟
            float NdotV = saturate(dot(o.Normal, IN.viewDir));
            fixed3 sss = _SSSColor.rgb * pow(1.0 - NdotV, 2.0) * _SSSStrength;

            o.Albedo = baseColor + sss;
            o.Smoothness = _Glossiness;
        }
        ENDCG
    }

    FallBack "Diffuse"
}
