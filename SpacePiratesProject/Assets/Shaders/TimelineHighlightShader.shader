Shader "Custom/TimelineHighlightShader"
{
    Properties
    {
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
        _Color0("Asteroid Color", Color) = (1,1,1,1)
        _Color1("Plasma Color", Color) = (1,1,1,1)
        _Color2("Police Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
        }
        LOD 100
        
        Cull Off
        ZWrite Off
        ZClip False
        ZTest Always
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color : COLOR;
                half2 texcoord  : TEXCOORD0;
            };

            sampler2D _MainTex;
            half4 _Color0;
            half4 _Color1;
            half4 _Color2;
            // Arrays need a set size. 2 should be enough: 1 for active event, 1 for ping
            static const int DATA_SIZE = 2;
            uniform float4 _HighlightData[DATA_SIZE]; //[start, end, type, alpha]

            v2f vert(appdata_t IN)
            {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.texcoord = IN.texcoord;
                OUT.color = IN.color;
                return OUT;
            }

            fixed4 frag(v2f IN) : COLOR
            {
                // Get base color
                half4 texcol = tex2D(_MainTex, IN.texcoord);
                texcol = texcol * IN.color;
                // Put colors in array so it can be indexed by event type
                half4 colors[3] = { _Color0, _Color1, _Color2 };
                for (int i = 0; i < DATA_SIZE; ++i)
                {
                    // This if statement could be removed with some math
                    if (_HighlightData[i].x <= IN.texcoord.x && _HighlightData[i].y >= IN.texcoord.x)
                    {
                        texcol.rgb = lerp(texcol, colors[(int)_HighlightData[i].z], _HighlightData[i].a);
                        break;
                    }
                }
                return texcol;
            }
            ENDCG
        }
    }
    Fallback "Sprites/Default"
}
