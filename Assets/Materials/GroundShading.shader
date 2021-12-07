Shader "Unlit/GroundShading"
{
    Properties
    {
        _Step("Step", Range(0,1)) = 0
        _Gloss("Gloss", float) = 0
        _Couleur("Color", Color) = (1,1,1,1)
        _Shadow("Shadow", Color) = (1,1,1,1)
        _MainTex("Texture", 2D) = "white" {}
         _V1("V1", Range(0,1)) = 0.25
         _V2("V2", Range(0,1)) = 0.3
        _ShadowThreshold("ShadowThreshold", Range(1,10)) = 1


        _MainColor("Main Color", Color) = (1,1,1,1)
        _MainTexture("Main Texture", 2D) = "white"{}
        _StepFoot("_Step Foot", 2D) = "white"{}
        _OutlineColor("Outline Color", Color) = (1,1,1,1)
        _OutlineSize("OutlineSize", Range(0,1.5)) = 1.1
        _StepColor("Step Color ", Color) = (1,1,1,1)
        _StepColorShadow("Step Color Shadow", Color) = (1,1,1,1)
_ChangeTime ("ChangeTime", float) = 0
    }
        SubShader
    {
           Tags { "RenderType" = "Opaque"}


        Pass
        {
            Tags {"LightMode" = "ForwardBase"}
            CGPROGRAM

            #pragma multi_compile_fwdbase
            #pragma vertex vert
            #pragma fragment frag
            #pragma fragmentoption ARB_precision_hint_fastest
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
             #pragma multi_compile_fog


            float _Step;
            float3 _Couleur;
            float3 _Shadow;
            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _StepFoot;
            float4 _StepFoot_ST;
            float _V1;
            float _V2;
            float _ShadowThreshold;
            float3 _Lightning;
            float3 _StepColor;
            float3 _StepColorShadow;
            
            struct Input
            {
                float4 pos : SV_POSITION;
                float3 vlight : COLOR;
                float3 lightDir : TEXCOORD1;
                float3 vNormal : TEXCOORD2;
                float2 uv : TEXCOORD5;
                LIGHTING_COORDS(3, 4)
                UNITY_FOG_COORDS(6)
            };



            Input vert(appdata_full v)
            {

                Input o;
                o.pos = UnityObjectToClipPos(v.vertex);

                o.lightDir = normalize(ObjSpaceLightDir(v.vertex));
                o.vNormal = normalize(v.normal).xyz;
                o.uv = v.texcoord;

                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                float3 worldNormal = mul((float3x3)unity_ObjectToWorld, SCALED_NORMAL);
                o.vlight = float3(0, 0, 0);

                #ifdef LIGHTMAP_OFF
                    float3 shlight = ShadeSH9(float4(worldNormal, 1.0));
                    o.vlight = shlight;
                #ifdef VERTEXLIGHT_ON
                    o.vlight += Shade4PointLights(
                        unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0,
                        unity_LightColor[0].rgb, unity_LightColor[1].rgb, unity_LightColor[2].rgb, unity_LightColor[3].rgb,
                        unity_4LightAtten0, worldPos, worldNormal
                    );

                #endif // VERTEXLIGHT_ON

                #endif // LIGHTMAP_OFF

                TRANSFER_VERTEX_TO_FRAGMENT(o);

                UNITY_TRANSFER_FOG(o, o.pos);

                return o;
            }



            float4 _LightColor0; // Contains the light color for this pass.


            float _ChangeTime;
            half4 frag(Input IN) : COLOR
            {
                IN.lightDir = normalize(IN.lightDir);
                IN.vNormal = normalize(IN.vNormal);

                float atten = LIGHT_ATTENUATION(IN);
               /// atten = step(_Step, atten);
                float NdotL = saturate(dot(IN.vNormal, IN.lightDir));
                float3 diffuse = (NdotL * atten) * _LightColor0.xyz;

                //diffuse = step(_Step, diffuse);

                if (diffuse.x >= _V1) {
                    diffuse = 1;
                }
                else if (diffuse.x < _V1 && diffuse.x > _V2) {
                    diffuse = 0.5;
                }
                else {
                    diffuse = 0;
                }


                float3 tex = tex2D(_StepFoot,IN.uv);
                
                float3 col = tex2D(_MainTex, IN.uv );
                col = lerp(col,_StepColor,tex.x);
                float3  ShadowValue = saturate((tex - (diffuse )));
                float3 output = diffuse;
              
                
                output = saturate( lerp(  _Shadow * col   , col, diffuse.xxx));
                output =saturate( lerp(output,_StepColorShadow,ShadowValue.x));
               /* float atten = LIGHT_ATTENUATION(IN);
                float3 color;
                float NdotL = saturate(dot(IN.vNormal, IN.lightDir));
                color = UNITY_LIGHTMODEL_AMBIENT.rgb * 2;
                color += IN.vlight;
                color += (_LightColor0) * NdotL * (atten * 2);
  q              color = step(_Step, color);
                float3 output = _Couleur;
                output += atten;*/
                
                UNITY_APPLY_FOG(IN.fogCoord, output);
                return half4(output.xyz,1);
                
            }



            ENDCG
        }



        Pass
        {
            Tags { "LightMode" = "ForwardAdd" }
         
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdadd 
            #pragma fragmentoption ARB_precision_hint_fastest

           
            
            #include "FGLightManage.cginc"
           


            ENDCG
        }
      
        //Pass
        //{
        //    Cull Front
        //    CGPROGRAM
        //    #pragma vertex vert
        //    #pragma fragment frag
        //    #include "UnityCG.cginc"
        //    fixed4 _OutlineColor;
        //    float _OutlineSize;
        //    struct appdata
        //    {
        //        float3 Normal :NORMAL;
        //        float4 vertex:POSITION;
        //    };
        //    struct v2f
        //    {
        //        
        //        float4 clipPos:SV_POSITION;
        //    };
        //    v2f vert(appdata v)
        //    {
        //        v2f o;
        //        o.clipPos = UnityObjectToClipPos(v.vertex );
        //       
        //        return o;
        //    }
        //    fixed4 frag(v2f i) : SV_Target
        //    {
        //        return _OutlineColor;
        //    }
        //    ENDCG
        //}


      

    }

        FallBack "Diffuse"

}
