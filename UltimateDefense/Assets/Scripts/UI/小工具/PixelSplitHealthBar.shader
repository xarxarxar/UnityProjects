Shader "UI/PixelSplitHealthBar"
{
    Properties
    {
        _MainTex("Main Texture", 2D) = "white" {}
        _FillAmount("Fill Amount", Range(0,1)) = 1
        _Segments("Segment Count", Float) = 10
        _LineWidth("Line Width (pixels)", Float) = 2
        _Color("Bar Color", Color) = (1,0,0,1)
        _LineColor("Line Color", Color) = (0,0,0,1)
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" }
        LOD 100

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _FillAmount;
            float _Segments;
            float _LineWidth;
            float4 _Color;
            float4 _LineColor;

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                float x = i.uv.x;
                float y = i.uv.y;

                // 每段中心
                float4 col = _Color;

                // 将像素宽度换算成UV
                float screenWidth = _ScreenParams.x;
                float lineHalfUV = _LineWidth * 0.5 / screenWidth;

                float segStep = 1.0 / _Segments;

                for (int s = 0; s < (int)_Segments; s++)
                {
                    float centerX = (s + 1) / (_Segments + 1); // 均匀分布
                    if (x >= centerX - lineHalfUV && x <= centerX + lineHalfUV)
                    {
                        // 黑线高度控制
                        if ((s + 1) % 5 == 0)
                        {
                            // 每5条线顶满
                            col = _LineColor;
                        }
                        else
                        {
                            // 其他线高度为一半
                            if (y >= 0.25 && y <= 0.75)
                                col = _LineColor;
                        }
                        break;
                    }
                }

                // 根据填充量裁掉右边
                if (x > _FillAmount)
                    col.a = 0;

                return col;
            }
            ENDCG
        }
    }
}
