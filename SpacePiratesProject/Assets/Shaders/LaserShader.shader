Shader "Unlit/LaserShader"
{
    Properties
    {
        //_MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        LOD 100

        Pass
        {
            Blend One One

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work

            #include "UnityCG.cginc"

            float4 _Color;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;

                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float4 GetLinearFade(float2 uv)
            {
                float f = lerp(0, 1, uv.y);

                return float4(f.xxxx);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return _Color * GetLinearFade(i.uv);

                //return _Color * a;
            }
            ENDCG
        }
    }
}
