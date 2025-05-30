Shader "Custom/TextureWithColor"
{
    // ========================
    // 材质属性区
    // ========================
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}    // 贴图属性：可以在 Inspector 拖入一张图片
        _Color ("Tint Color", Color) = (1,1,1,1)      // 颜色属性：默认白色，不影响贴图
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }                // 表示这个 Shader 是不透明的
        LOD 100                                        // Shader 的复杂度等级，越低越简单，越快

        Pass
        {
            // ========================
            // 下面开始写 CG 程序块
            // ========================
            CGPROGRAM

            // 使用哪个函数当作顶点着色器和片元着色器
            #pragma vertex vert
            #pragma fragment frag

            // 引入 Unity 的常用函数（比如 UnityObjectToClipPos）
            #include "UnityCG.cginc"

            // ========================================
            // 声明变量（和 Properties 中的对应）
            // ========================================

            sampler2D _MainTex;        // 2D 贴图采样器（用来获取贴图上的颜色）
            fixed4 _Color;             // 四个分量的颜色向量（R,G,B,A），我们用它来乘到贴图上

            // ========================
            // 顶点着色器输入结构体（每个顶点带来的信息）
            // ========================
            struct appdata
            {
                float4 vertex : POSITION;   // 顶点坐标（模型空间）
                float2 uv : TEXCOORD0;      // 顶点对应的贴图坐标（UV）
            };

            // ========================
            // 顶点着色器输出结构体（传给片元阶段的数据）
            // ========================
            struct v2f
            {
                float4 pos : SV_POSITION;   // 最终要显示在屏幕上的位置（裁剪空间）
                float2 uv : TEXCOORD0;      // 要传给片元着色器的 UV 坐标
            };

            // ========================
            // 顶点着色器：作用是把模型空间坐标变成屏幕空间坐标，并传出 UV
            // ========================
            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex); // 用 Unity 提供的函数把模型坐标转成裁剪空间坐标
                o.uv = v.uv;                             // UV 坐标直接传下去
                return o;
            }

            // ========================
            // 片元着色器：对每个像素进行处理，返回这个像素的颜色
            // ========================
            fixed4 frag (v2f i) : SV_Target
            {
                // 从贴图上获取这个像素对应 UV 坐标的颜色值（RGBA）
                fixed4 texColor = tex2D(_MainTex, i.uv);

                // 把采样到的贴图颜色乘上你传进来的颜色（实现染色效果）
                fixed4 finalColor = texColor * _Color;

                // 返回这个最终颜色，屏幕上就会显示它
                return finalColor;
            }

            ENDCG
        }
    }
}
