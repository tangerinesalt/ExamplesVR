Shader "Custom/Bellows" {
    Properties {
        _FrontColor ("Front Color", Color) = (1,1,1,1)
        _BackColor ("Back Color", Color) = (0.1,0.1,0.1,1)
        _TransitionWidth ("Transition Width", Range(0.01, 1)) = 0.2
        // 用来改变正反面的判定阈值，决定正反法线方向颜色的混合比例
        _FacingBias ("Facing Bias", Range(-1, 1)) = 0
        _FrontMetallic ("Front Metallic", Range(0, 1)) = 0
        _BackMetallic ("Back Metallic", Range(0, 1)) = 0
        _FrontSmoothness ("Front Smoothness", Range(0, 1)) = 0.7
        _BackSmoothness ("Back Smoothness", Range(0, 1)) = 0.5
        _RimColor ("Rim Color", Color) = (0.1,0.1,0.1,1)
        _RimPower ("Rim Power", Range(0.5, 8)) = 3
        _RimStrength ("Rim Strength", Range(0, 1)) = 0.2
    }
    SubShader {
        Tags { "RenderType"="Opaque" }
        Cull Off
        
        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows
        #pragma target 3.0
        
        struct Input {
            float3 worldNormal;
            float3 viewDir;
        };
        
        fixed4 _FrontColor;
        fixed4 _BackColor;
        float _TransitionWidth;
        float _FacingBias;
        float _FrontMetallic;
        float _BackMetallic;
        float _FrontSmoothness;
        float _BackSmoothness;
        fixed4 _RimColor;
        float _RimPower;
        float _RimStrength;
        
        void surf (Input IN, inout SurfaceOutputStandard o) {
            float3 normal = normalize(IN.worldNormal);
            float3 viewDir = normalize(IN.viewDir);
            float facing = dot(normal, viewDir) + _FacingBias;
            float blend = saturate(smoothstep(-_TransitionWidth, _TransitionWidth, facing));
            
            fixed3 baseColor = lerp(_BackColor.rgb, _FrontColor.rgb, blend);
            float metallic = lerp(_BackMetallic, _FrontMetallic, blend);
            float smoothness = lerp(_BackSmoothness, _FrontSmoothness, blend);
            
            o.Albedo = baseColor;
            o.Metallic = metallic;
            o.Smoothness = smoothness;
            o.Occlusion = 1;
            float rim = pow(saturate(1 - dot(normal, viewDir)), _RimPower) * _RimStrength;
            o.Emission = _RimColor.rgb * rim;
            o.Alpha = 1;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
