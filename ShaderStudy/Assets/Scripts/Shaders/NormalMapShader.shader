Shader "Custom/NormalMapShader"
{
    Properties
    {
        _NormalMap ("Normal Map", 2D) = "bump" {}
        _Color ("Color", Color) = (1, 1, 1, 1)
    }
    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _NormalMap;
            float4 _Color;

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 normal : TEXCOORD0;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.normal = v.normal;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // 采样法线贴图
                float3 normalMap = tex2D(_NormalMap, i.normal.xy).xyz * 2.0 - 1.0;
                float3 lightDir = normalize(float3(1, 1, 1)); // 假设光源方向
                float NdotL = max(dot(i.normal, lightDir), 0.0); // 计算光照

                return _Color * NdotL; // 使用法线贴图调整表面效果
            }
            ENDCG
        }
    }
}
