Shader "Unlit/ParallaxTest"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _XSpeed ("X Scroll Speed", Range(0, 10)) = 1
        _YSpeed ("Y Scroll Speed", Range(0, 10)) = 1
    }
    SubShader
    {
        Tags { 
            
            "RenderType"="Transparent" 
            "Order" = "Transparent" 
            }
        LOD 100

        Pass
        {
            //Cull Off
            //ZWrite Off
            //Blend One One

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            float _XSpeed;
            float _YSpeed;

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

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (Meshdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);

                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                o.uv.x += _Time.y * _XSpeed;
                o.uv.y += _Time.y * _YSpeed;

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);

                float testVal = col.r + col.g + col.b - 0.001;

                clip(testVal);
                
                return col;
            }
            ENDCG
        }
    }
}
