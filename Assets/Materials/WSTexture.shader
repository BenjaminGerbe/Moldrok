Shader "Unlit/WSTexture"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Offet ("Offset",float) = 1
        _BackGroundColor ("BackGroundColor",color) = (0,0,0)
        _ColorMotif ("ColorMotif",color) = (0,0,0)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float3 worldSapce : TEXCOORD1;
                UNITY_FOG_COORDS(2)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Offet;
            float3 _BackGroundColor;
            float3 _ColorMotif;
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldSapce = mul(UNITY_MATRIX_M,o.vertex.xyz);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture

            
                
                fixed3 col = tex2D(_MainTex, i.uv * _Offet);
                // apply fog
              


                col =  lerp(_BackGroundColor,_ColorMotif,col);

                UNITY_APPLY_FOG(i.fogCoord, col);
                return float4( col.xyz,0);
            }
            ENDCG
        }
    }
}
