Shader "Custom/ShaderTest24101102"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
 
            #include "UnityCG.cginc"
            
            //新的引用
            #include "Lighting.cginc"
 
            float4 _Color;
            sampler2D _MainTex;
            float _Glossiness;
            //返回结构体        //引用结构体
            appdata_full vert (appdata_full v)
            {    
                //模型顶点坐标转屏幕坐标
                v.vertex = UnityObjectToClipPos(v.vertex);
                
                //获取法线坐标并转换成世界坐标下的法线坐标
                float3 worldNormal = UnityObjectToWorldNormal(v.normal);
 
                 //世界坐标下的光线坐标  //单位化坐标   //获取世界坐标下的光线坐标
                float3 worldLight = normalize(_WorldSpaceLightPos0.xyz);
                
                 //上面的公式
                float3 diffuse =_LightColor0.rgb * v.color.rgb * max(0,dot(worldNormal,worldLight));
 
                //算出的值给颜色
                v.color = float4(diffuse,1);
                
                return v;
            }
 
 
            float4 frag (appdata_full v) : SV_Target
            {    
                //输出颜色    
                return float4(v.color*_Color.xyz,1) ;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
