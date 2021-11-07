
#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "AutoLight.cginc"

struct appdata
{
    float4 vertex : POSITION;
    float2 uv : TEXCOORD0;
    float3 normal : NORMAL;
};

struct v2f
{
    float2 uv : TEXCOORD0;
    float3 normal : TEXCOORD1;
    float4 vertex : SV_POSITION;
    float3 wPos : TEXCOORD2;
    float3 vlight : COLOR;
    LIGHTING_COORDS(3, 4)
};

sampler2D _MainTex;
float4 _MainTex_ST;
float3 _Couleur;
float _Step;
float _Gloss;

v2f vert(appdata v)
{
    v2f o;
    o.vertex = UnityObjectToClipPos(v.vertex);
    o.normal = UnityObjectToWorldNormal(v.normal);
    o.wPos = mul(unity_ObjectToWorld, v.vertex);
    o.uv = v.uv;


    TRANSFER_VERTEX_TO_FRAGMENT(o);

    return o;
}

float4 frag(v2f i) : SV_Target
{
    // sample the texture
 //  float3 N = normalize( i.normal);
   float3 N = normalize(i.normal);
   float3 L = normalize(UnityWorldSpaceLightDir(i.wPos));
   float NdotL = saturate(dot(N, L));

   float distanceToLight = _WorldSpaceLightPos0.xyz - i.wPos.xyz;
   float attenuation = LIGHT_ATTENUATION(i);
   float3 diffuse = (NdotL * attenuation) * _LightColor0.xyz;
   float3 V = normalize(_WorldSpaceCameraPos - i.wPos);
   float3 R = reflect(-L,N);



   /*
      float DP = fwidth(diffuse);
      float3 mask = diffuse / DP;
      diffuse = mask;*/
      // diffuse *= _LightColor0;
       diffuse = step(_Step,diffuse);



      float3 output = _Couleur;
       output += diffuse ;

       //  float3 specular = saturate( dot(V, R));
         //specular = pow(specular, _Gloss);

         //specular = step(_Step, specular );


         //output += step(_Step, output);
         //output += specular;

/*
       float attenuation = LIGHT_ATTENUATION(i);
       float3 output;
       float NdotL = saturate(dot(N, L));
       output = UNITY_LIGHTMODEL_AMBIENT.rgb * 2;
       output += i.vlight;
       output += _LightColor0.rgb * NdotL * (attenuation * 2);*/

         return float4(output,0);
}