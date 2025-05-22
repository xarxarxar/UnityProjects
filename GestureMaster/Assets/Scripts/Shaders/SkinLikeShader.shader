Shader "Custom/SkinLikeShader"
{
    Properties
    {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _SkinColor ("Skin Tint", Color) = (1, 0.8, 0.7, 1)
        _SSSColor ("Subsurface Color", Color) = (1, 0.5, 0.4, 1)
        _SSSStrength ("SSS Strength", Range(0, 1)) = 0.3
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        //_SpecColor 已内置，无需重复定义
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 300

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows

        sampler2D _MainTex;
        fixed4 _SkinColor;
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
            
            // 基础颜色乘以肤色调
            fixed3 baseColor = tex.rgb * _SkinColor.rgb;

            // 简单模拟 SSS（次表面散射）——通过视角方向插值添加柔光
            float NdotV = saturate(dot(o.Normal, IN.viewDir));
            fixed3 sss = _SSSColor.rgb * pow(1.0 - NdotV, 2.0) * _SSSStrength;

            o.Albedo = baseColor + sss;
            o.Smoothness = _Glossiness;

            // 使用默认内置的 SpecColor（Standard Shader）
            // 若你希望自定义，可以使用 metallic workflow
        }
        ENDCG
    }

    FallBack "Diffuse"
}
