Shader "Unlit/CellShading TwoColors"
{
    Properties
    {
        _Step("Step", Range(0,1)) = 0
       
         _V1("V1", Range(0,1)) = 0.25
         _V2("V2", Range(0,1)) = 0.3
        _ShadowThreshold("ShadowThreshold", Range(1,10)) = 1
        _Gloss("Gloss", float) = 0
    
        _Couleur("Color", Color) = (1,1,1,1)
        _Couleur2("Color2", Color) = (1,1,1,1)
        _MainTex("Texture", 2D) = "white" {}


    }
        SubShader
    {
           Tags { "RenderType" = "Opaque"  }


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
            float3 _Couleur2;

            float _V1;
            float _V2;
            float _ShadowThreshold;
      
            struct Input
            {
                float4 pos : SV_POSITION;
                float3 vlight : COLOR;
                float3 lightDir : TEXCOORD1;
                float3 vNormal : TEXCOORD2;
                LIGHTING_COORDS(3, 4)
                UNITY_FOG_COORDS(5)

            };



            Input vert(appdata_full v)
            {

                Input o;
                o.pos = UnityObjectToClipPos(v.vertex);

                o.lightDir = normalize(ObjSpaceLightDir(v.vertex));
                o.vNormal = normalize(v.normal).xyz;

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



            half4 frag(Input IN) : COLOR
            {
                IN.lightDir = normalize(IN.lightDir);
                IN.vNormal = normalize(IN.vNormal);

                float atten = LIGHT_ATTENUATION(IN);
                atten = step(_Step, atten);
                float NdotL = saturate(dot(IN.vNormal, IN.lightDir));
                float3 diffuse = (NdotL * atten) * _LightColor0.xyz;
                
                if (diffuse.x < _V1) {
                    diffuse = 0;
                }
                else if (diffuse.x >= _V1 && diffuse.x < _V2) {
                    diffuse = 0.25;
                }
                else {
                    diffuse = 1;
                }

                //diffuse = step(_Step, diffuse);

                float3 output;
                output = diffuse;

                output = saturate(lerp(_Couleur  * _Couleur2,_Couleur2 ,diffuse.xxx));

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

                return half4(output,1);

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

      
        /*Pass{
             Name "ShadowCaster"
             Tags { "LightMode" = "ShadowCaster" }

             ZWrite On ZTest LEqual

             CGPROGRAM
             #pragma target 2.0

             #pragma multi_compile_shadowcaster

             #pragma vertex vertShadowCaster
             #pragma fragment fragShadowCaster

             #include "UnityStandardShadow.cginc"

             ENDCG
        }*/

      

    }

        FallBack "Diffuse"

}
