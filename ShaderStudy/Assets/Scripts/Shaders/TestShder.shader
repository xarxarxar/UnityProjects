Shader "Custom/RedColor"
{
    SubShader
    {
        Pass
        {
            CGPROGRAM

            // 编译指令：指定使用的顶点和片元函数
            #pragma vertex vert
            #pragma fragment frag

            // 顶点结构体，输入数据
            struct appdata
            {
                float4 vertex : POSITION; // 顶点位置
            };

            // 顶点输出结构体，传给片元函数
            struct v2f
            {
                float4 pos : SV_POSITION; // 屏幕位置
            };

            // 顶点函数：负责将顶点坐标从模型空间转换到屏幕空间
            v2f vert(appdata v)
            {
                v2f o;
                // 把模型空间坐标变换成裁剪空间（用于绘制）
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            // 片元函数：返回一个红色的像素
            fixed4 frag(v2f i) : SV_Target
            {
                // 返回纯红色 (R=1, G=0, B=0, A=1)
                return fixed4(1, 0, 0, 1);
            }

            ENDCG
        }
    }
}
