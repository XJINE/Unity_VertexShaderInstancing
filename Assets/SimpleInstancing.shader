Shader "Custom/Unlit/SimpleInstancing"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
        }

        LOD 100

        Pass
        {
            CGPROGRAM

            #pragma vertex vertexShader
            #pragma fragment fragmentShader
            
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct vertexOutput
            {
                float2 uv : TEXCOORD0;
                half4 color : TEXCOORD1;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST; // need to use TRANSFORM_TEX

            // CPU から頂点情報を送って保存するバッファ。

            StructuredBuffer<float3> _VertexBuffer;
            
            // ------------------------------------------------------------------------------------
            // VertexShader
            // ------------------------------------------------------------------------------------

            vertexOutput vertexShader (appdata input,
                                       uint vertexID : SV_VertexID,
                                       uint instanceID : SV_InstanceID)
            {
                // SV_VertexID は何番目の頂点かを示すインデックス。
                // SV_InstanceID は何番目のインスタンスかを示すインデックス。

                // バッファから頂点を取得して、わずかに移動して追加します。
                // 移動する基準となる点は、常に (0, 0, 0) にある点に注意します。

                input.vertex.xyz += _VertexBuffer[vertexID];
                input.vertex.x += (instanceID % 10) + (instanceID % 10) * 0.5;
                input.vertex.y += (instanceID / 10) + (instanceID / 10) * 0.5;

                // 一般的なバーテックスシェーダと同じように処理します。

                vertexOutput output;
                output.vertex = mul(UNITY_MATRIX_MVP, input.vertex);
                output.uv = TRANSFORM_TEX(input.uv, _MainTex);
                output.color = half4(1, 0, 0, 1);

                return output;
            }

            // ------------------------------------------------------------------------------------
            // VertexShader
            // ------------------------------------------------------------------------------------

            fixed4 fragmentShader (vertexOutput input,
                                   uint primitiveID : SV_PrimitiveID) : SV_Target
            {
                // SV_Target は戻り値の出力先を決定します。
                // MRT で複数に出力する場合などには、SV_Target1 などが指定されます。
                // SV_PrimitiveID は、Triangle や Line などのプリミティブの番号を示す値です。

                return input.color;
            }

            ENDCG
        }
    }
}