Shader "Custom/TransparentTexture"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}       // 带透明通道的贴图
        _Color ("Tint Color", Color) = (1,1,1,1)         // 混合颜色，包含透明度
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }  // 指定为透明渲染队列
        LOD 100

        Pass
        {
            // 设置为透明混合模式：源颜色按 alpha 混合，目标颜色取背景
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D _MainTex;      // 主贴图
            fixed4 _Color;           // 可调颜色和透明度

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex); // 模型空间 -> 裁剪空间
                o.uv = v.uv;                             // 保留贴图坐标
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // 采样贴图颜色（包含 alpha）
                fixed4 texColor = tex2D(_MainTex, i.uv);

                // 和材质颜色混合（也包含 alpha 通道）
                fixed4 finalColor = texColor * _Color;

                // 可选：如果透明度太小（几乎看不到），直接丢弃像素（提高效率）
                if (finalColor.a < 0.01)
                    discard;

                return finalColor;
            }

            ENDCG
        }
    }
}
