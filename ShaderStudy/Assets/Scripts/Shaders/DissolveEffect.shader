Shader "Custom/DissolveEffect"
{
    Properties
    {
        _MainTex ("Base Texture", 2D) = "white" {}
        _DissolveTex ("Dissolve Mask", 2D) = "black" {}
        _Color ("Color", Color) = (1, 1, 1, 1)
        _DissolveAmount ("Dissolve Amount", Range(0, 1)) = 0.0
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            // 属性
            sampler2D _MainTex;
            sampler2D _DissolveTex; // 溶解遮罩贴图
            float4 _Color;
            float _DissolveAmount;

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

            // 顶点函数
            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            // 片元函数
            fixed4 frag(v2f i) : SV_Target
            {
                // 从主贴图采样颜色
                fixed4 col = tex2D(_MainTex, i.uv);
                
                // 从溶解遮罩采样
                float dissolve = tex2D(_DissolveTex, i.uv).r;

                // 根据溶解量修改透明度
                if (dissolve < _DissolveAmount)
                {
                    // 如果溶解遮罩值小于当前溶解量，则透明
                    col.a = 0.0;
                }
                else
                {
                    // 否则正常显示
                    col.a = 1.0;
                }

                // 返回最终的颜色
                return col * _Color;
            }
            ENDCG
        }
    }
}
