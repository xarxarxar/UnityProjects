Shader "Custom/BasicLighting"
{
    Properties
    {
        _Color ("Color", Color) = (1, 1, 1, 1) // 白色
         _Dire ("Direction", Vector) = (1, 1, 1, 0) // 光照方向 (float4 类型)
    }
    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            // 定义材质的颜色
            float4 _Color;
            float4 _Dire; // 声明光照方向变量

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 normal : NORMAL;
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
                // 简单的 Lambertian 光照计算
                float3 lightDir = normalize(float3(_Dire.x, _Dire.y, _Dire.z)); // 假设光源方向 (1, 1, 1)
                float NdotL = max(dot(i.normal, lightDir), 0.0); // 计算光照强度

                return _Color * NdotL; // 根据光照强度调节颜色
            }
            ENDCG
        }
    }
}
