Shader "Custom/ParallaxTest"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        [PerRendererData] _ScaledTime ("Scaled Time", float) = 0
        _XSpeed ("X Scroll Speed", Range(0, 10)) = 1
        _YSpeed ("Y Scroll Speed", Range(0, 10)) = 1
        _ClipThreshold ("Clip Threshold", Range(0, 1)) = 0.01
        _UseTransparency ("Use Transparency", Int) = 0
        _BaseAlpha ("Base Alpha", Range(0, 1)) = 0
        _AlphaMult ("Alpha Multiplyer", float) = 1
    }
    
    SubShader
    {
        Tags {"RenderType"="Transparent" "Order" = "Transparent"}
        LOD 100

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Back

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _ScaledTime;
            float _XSpeed;
            float _YSpeed;
            float _ClipThreshold;
            fixed _UseTransparency;
            float _BaseAlpha;
            float _AlphaMult;

            struct Meshdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (Meshdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);

                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.uv.x += _ScaledTime * _XSpeed;
                o.uv.y += _ScaledTime * _YSpeed;

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);

                float maxValue = max(max(col.r, col.g), col.b);
                clip(maxValue - _ClipThreshold);

                if (_UseTransparency == 1)
                {
                    col.a = maxValue * _AlphaMult + _BaseAlpha;
                }

                return col;
            }
            ENDCG
        }
    }

    Fallback "Universal Render Pipeline/Unlit"
}
