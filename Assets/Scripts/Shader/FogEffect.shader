Shader "Custom/FogEffect"
{
    Properties
    {
        _FogColor("Fog Color", Color) = (1,1,1,1)
        _FogDensity("Fog Density", Range(0,2)) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" }
        
        Pass
        {
            Name "FogPass"
            ZWrite Off
            ZTest Always
            Cull Off
            
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
            
            TEXTURE2D_X(_BlitTexture);

            TEXTURE2D_X(_CameraDepthTexture);
            SAMPLER(sampler_CameraDepthTexture);
            
            float4 _FogColor;
            float _FogDensity;
            
            struct Attributes
            {
                uint vertexID : SV_VertexID;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 texcoord   : TEXCOORD0;
                UNITY_VERTEX_OUTPUT_STEREO
            };
            
            Varyings Vert(Attributes input)
            {
                Varyings output;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
                
                // フルスクリーン三角形の頂点位置を計算
                float4 pos = float4(0, 0, 0, 1);
                pos.x = (input.vertexID == 1) ? 3.0 : -1.0;
                pos.y = (input.vertexID == 2) ? -3.0 : 1.0;
                
                output.positionCS = pos;
                
                // UV座標を計算
                output.texcoord = float2((input.vertexID == 1) ? 2.0 : 0.0, (input.vertexID == 2) ? 2.0 : 0.0);
                
                //OpenGL と DirectX の UVの上下差を吸収するための処理なんだけど、
                //URP + Blitter + _BlitTexture の組み合わせではこのフリップが「二重に適用される」ことがあるらしいから今回消す
                // #if UNITY_UV_STARTS_AT_TOP
                // output.texcoord.y = 1.0 - output.texcoord.y;
                // #endif
                
                return output;
            }
            
            float4 Frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
                
                float2 uv = input.texcoord;
                
                // 元の色を取得
                float4 col = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv);
                
                // 深度を取得
                float depth01 = SAMPLE_TEXTURE2D_X(_CameraDepthTexture, sampler_CameraDepthTexture, uv).r;
                
                // 深度をビュー空間距離に変換
                float depthEye = LinearEyeDepth(depth01, _ZBufferParams);
                
                // フォグ係数を計算
                float fogFactor = saturate(depthEye * _FogDensity);
                
                // フォグを適用
                col.rgb = lerp(col.rgb, _FogColor.rgb, fogFactor);
                
                return col;
            }
            ENDHLSL
        }
    }
}