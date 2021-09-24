Shader "Custom/Occluder"
{
    SubShader
    {
        Tags{"RenderType" = "Geometry" "Queue" = "Geometry"}

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite On

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            fixed4 vert(float4 pos : POSITION) : SV_POSITION
            {
                return UnityObjectToClipPos(pos);
            }

            fixed4 frag() : COLOR
            {
                return 0;
            }

            ENDCG
        }
    }
}