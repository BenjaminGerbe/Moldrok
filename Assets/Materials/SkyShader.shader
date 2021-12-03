Shader "Custom/Sky"
{
	Properties
	{
		_UpDir("Up Direction", Vector) = (0.0, 1.0, 0.0)
		[Space]
		_ColorLight0("Light Color 0", Color) = (1, 1, 1, 0)
		_ColorLight1("Light Color 1", Color) = (1, 1, 1, 0)
		_ColorLight2("Light Color 2", Color) = (1, 1, 0, 0)
		_ColorLight3("Light Color 2", Color) = (0, 0, 0, 0)
		_StopLight1("Light Stop 1", Range(0, 1)) = 0.1
		_StopLight2("Light Stop 2", Range(0, 1)) = 0.5
		[Space]
		_ColorSkyLight0("Light Sky Color 0", Color) = (0.67, 0.78, 0.99, 0)
		_ColorSkyLight1("Light Sky Color 1", Color) = (0.67, 0.78, 0.99, 0)
		_ColorSkyLight2("Light Sky Color 2", Color) = (0.49, 0.66, 1, 0)
		_ColorSkyLight3("Light Sky Color 2", Color) = (0.49, 0.66, 1, 0)
		_StopSkyLight1("Light Sky Stop 1", Range(0, 1)) = 0.495
		_StopSkyLight2("Light Sky Stop 2", Range(0, 1)) = 0.505
		[Space]
		_ColorSkyDark0("Dark Sky Color 0", Color) = (0.67, 0.78, 0.99, 0)
		_ColorSkyDark1("Dark Sky Color 1", Color) = (0.67, 0.78, 0.99, 0)
		_ColorSkyDark2("Dark Sky Color 2", Color) = (0.49, 0.66, 1, 0)
		_ColorSkyDark3("Dark Sky Color 2", Color) = (0.49, 0.66, 1, 0)
		_StopSkyDark1("Dark Sky Stop 1", Range(0, 1)) = 0.495
		_StopSkyDark2("Dark Sky Stop 2", Range(0, 1)) = 0.505
	}
	SubShader
	{
		Tags
		{
			"PreviewType" = "Plane"
		}
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			struct VertIn
			{
				float4 vertex : POSITION;
			};
	
			float3 _UpDir;
			float4 _LightColor;

			float4 _ColorLight0;
			float4 _ColorLight1;
			float4 _ColorLight2;
			float4 _ColorLight3;
			float _StopLight1;
			float _StopLight2;

			float4 _ColorSkyDark0;
			float4 _ColorSkyDark1;
			float4 _ColorSkyDark2;
			float4 _ColorSkyDark3;
			float _StopSkyDark1;
			float _StopSkyDark2;

			float4 _ColorSkyLight0;
			float4 _ColorSkyLight1;
			float4 _ColorSkyLight2;
			float4 _ColorSkyLight3;
			float _StopSkyLight1;
			float _StopSkyLight2;

			struct Gradient
			{
				float4 color0;
				float4 color1;
				float4 color2;
				float4 color3;
				float stop1;
				float stop2;
			};

			// t in 0...1 range.
			Gradient gradientBlend(Gradient g0, Gradient g1, float t)
			{
				Gradient g;

				g.color0 = lerp(g0.color0, g1.color0, t);
				g.color1 = lerp(g0.color1, g1.color1, t);
				g.color2 = lerp(g0.color2, g1.color2, t);
				g.color3 = lerp(g0.color3, g1.color3, t);
				g.stop1 = lerp(g0.stop1, g1.stop1, t);
				g.stop2 = lerp(g0.stop2, g1.stop2, t);

				return g;
			}

			// t in 0...1 range.
			float4 colorFromGradient(Gradient g, float t)
			{
				float stop0 = 0;
				float stop3 = 1;
				float4 color;

				color = lerp(g.color0, g.color1, smoothstep(stop0, g.stop1, t));
				color = lerp(color, g.color2, smoothstep(g.stop1, g.stop2, t));
				color = lerp(color, g.color3, smoothstep(g.stop2, stop3, t));

				return color;
			}

			struct VertOut
			{
				float4 position : SV_POSITION;
				float4 worldPosition : TEXCOORD0;
			};

			VertOut vert(VertIn v)
			{
				VertOut o;

				o.position = UnityObjectToClipPos(v.vertex);
				o.worldPosition = mul(unity_ObjectToWorld, v.vertex);

				return o;
			}

			float4 SkyColor(float camLightHMatch, float elevation)
			{
				Gradient g0;
				g0.color0 = _ColorSkyDark0;
				g0.color1 = _ColorSkyDark1;
				g0.color2 = _ColorSkyDark2;
				g0.color3 = _ColorSkyDark3;
				g0.stop1 = _StopSkyDark1;
				g0.stop2 = _StopSkyDark2;

				Gradient g1;
				g1.color0 = _ColorSkyLight0;
				g1.color1 = _ColorSkyLight1;
				g1.color2 = _ColorSkyLight2;
				g1.color3 = _ColorSkyLight3;
				g1.stop1 = _StopSkyLight1;
				g1.stop2 = _StopSkyLight2;

				Gradient skyGradient = gradientBlend(g0, g1, camLightHMatch);

				return colorFromGradient(skyGradient, elevation);
			}

			float4 LightColor(float viewMatchLight)
			{
				Gradient lightGradient;
				lightGradient.color0 = _ColorLight0;
				lightGradient.color1 = _ColorLight1;
				lightGradient.color2 = _ColorLight2;
				lightGradient.color3 = _ColorLight3;
				lightGradient.stop1 = _StopLight1;
				lightGradient.stop2 = _StopLight2;

				return colorFromGradient(lightGradient, viewMatchLight);
			}

			float4 frag(VertOut i) : SV_Target
			{
				float3 fragDir = normalize(i.worldPosition - _WorldSpaceCameraPos);
				float3 upDir = normalize(_UpDir);
				float elevation = dot(fragDir, upDir); // -1..1

				float3 camDir = normalize(-UNITY_MATRIX_V[2].xyz);
				float3 camUpDir = normalize(-UNITY_MATRIX_V[1].xyz);
				// Prevent vertical cam vector when looking straight up or down.
				// This would lead to a zero-length vector after being horizontally projected,
				// resulting in NAN when normalized.
				float3 camForwardishDir = camDir + camUpDir * elevation;

				float3 camDirH = normalize(cross(cross(upDir, camForwardishDir), upDir));
				float3 lightDir = normalize(_WorldSpaceLightPos0);
				float3 lightDirH = normalize(cross(cross(upDir, lightDir), upDir));

				float3 camLightHMatch = dot(camDirH, lightDirH); // -1..1
				camLightHMatch = (1 + camLightHMatch) / 2; // 0..1

				elevation = (1 + elevation) / 2; // 0..1
				float4 skyColor = SkyColor(camLightHMatch, elevation);

				float viewMatchLight = dot(fragDir, lightDir); // -1..1
				viewMatchLight = (1 + viewMatchLight) / 2; // 0..1
				float4 lightColor = LightColor(viewMatchLight);

				
				return skyColor + lightColor;
			}
			ENDCG
		}
	}
}