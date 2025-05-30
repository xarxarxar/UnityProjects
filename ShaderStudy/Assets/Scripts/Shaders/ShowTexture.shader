Shader "Custom/ShowTexture"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            // 引入 Unity 内置的 Shader 变量和函数
            #include "UnityCG.cginc"

            // 属性变量，用于在材质中传入贴图
            sampler2D _MainTex;

            struct appdata
            {
                float4 vertex : POSITION;   // 顶点坐标
                float2 uv : TEXCOORD0;      // UV 坐标（贴图坐标）
            };

            struct v2f
            {
                float4 pos : SV_POSITION;   // 屏幕位置（必须）
                float2 uv : TEXCOORD0;      // 传给片元函数的 UV 坐标
            };

            // 顶点函数：负责坐标变换 + 传 UV 给片元函数
            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex); // 顶点转为屏幕坐标
                o.uv = v.uv; // 将模型的 UV 坐标传给片元阶段
                return o;
            }

            // 片元函数：从贴图中取颜色
            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv); // 从贴图中采样颜色
                return col;
            }

            ENDCG
        }
    }
}
