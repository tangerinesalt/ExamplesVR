Shader "Custom/Test01"
{
    
    SubShader
    {
        pass
        {

        CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag

        #include "UnityCG.cginc"

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
        };

        appdata_base vert (appdata_base v)
        {
            v.vertex=UnityObjectToClipPos(v.vertex);
            return v;
        }

        float4 frag (appdata_base v) : SV_Target
        {
            float3 n=(v.normal+float3(1,1,1))/2;
            return float4(n,1);
        }
        ENDCG
    }
       
}}
