Shader "Unlit/SkyBox_Shader"
{
    Properties
    {
      
        _BottomColor ("BottomColor",color) = (0,0,0)
        _TopColor ("TopColor",color) = (0,0,0)
        _value ("Value",float) = 1
        _SunPosition("sunPosition",Vector) = (0,0,0,0)
         _SunRadial ("SunRadial",float) = 1
        
    }
    SubShader
    {
        Tags { "RenderType"="Background" "Queue"="Background" }
        LOD 100
   
        Pass
        {
            Zwrite Off
            Cull Off
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
                float4 vertex : SV_POSITION;
            };


            float3 _BottomColor;
            float3 _TopColor; 
            float _value;
            float _SunRadial;
            float3 _SunPosition;

           
            
            v2f vert (appdata v)
            {
                v2f o;
           
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
            
                return o;
            }

            

            fixed4 frag (v2f i) : SV_Target
            {
                float value = clamp(i.uv.y + _value,0,1 );
                
                float3 color = lerp(_BottomColor,_TopColor,value);

    
                

                return  float4(color.xyz ,1);
                
             
            }
            ENDCG
        }
    }
}
