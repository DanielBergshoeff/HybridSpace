Shader "Custom/TerrainCircle" {
	Properties{
		_MainTex("Texture",2D)= "white"{}
		_MainColor("Main Color", Color) = (0, 1, 0)
		_Radius("Radius", Range(0, 200)) = 100
		_Softness("Sphere Softness",Range(0,100)) = 0
		_SoftnessPlayer("Player Sphere Softness",Range(0,100)) = 0
		_RadiusPlayer("Radius", Range(0, 200)) = 100
		_CircleColor("Circle Color", Color) = (1,0,0)
		_Center("Center", Vector) = (0,0,0,0)

		_CircleColor2("Circle Color 2", Color) = (1,0,0)
		_Center2("Center 2", Vector) = (0,0,0,0)

		_CircleColor3("Circle Color 3", Color) = (1,0,0)
		_Center3("Center 3", Vector) = (0,0,0,0)

		_CircleColor4("Circle Color 4", Color) = (1,0,0)
		_Center4("Center 4", Vector) = (0,0,0,0)

		_TotalRadius("Total Radius", Range(0, 200)) = 30
		_TotalSoftness("Total Softness",Range(0, 100)) = 10
		_TotalCenter("Total Center", Vector) = (0, 0, 0, 0)
	}

		SubShader{

			Tags { "RenderType"="Transparent" "Queue"="Transparent" "AllowProjectors"="False" }
 
			blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma surface surfaceFunc Lambert alpha:blend

			fixed4 LightingNoLighting(SurfaceOutput s, fixed3 lightDir, fixed atten)
		{
			return fixed4(s.Albedo, s.Alpha);
		}

			sampler2D _MainTex;
			fixed3 _MainColor;
			half _Radius;
			half _Softness;
			half _SoftnessPlayer;
			half _RadiusPlayer;


			fixed3 _CircleColor;
			float3 _Center;

			fixed3 _CircleColor2;
			float3 _Center2;

			fixed3 _CircleColor3;
			float3 _Center3;

			fixed3 _CircleColor4;
			float3 _Center4;

			half _TotalRadius;
			half _TotalSoftness;
			float3 _TotalCenter;

			struct Input {
				float2 uv_MainTex;
				float3 worldPos;

				  
			};

			void surfaceFunc(Input IN, inout SurfaceOutput o){

				
				half4 c = tex2D(_MainTex, IN.uv_MainTex);
				float dist = distance(_Center, IN.worldPos);
				float dist2 = distance(_Center2, IN.worldPos);
				float dist3 = distance(_Center3, IN.worldPos);
				float dist4 = distance(_Center4, IN.worldPos);

				float dist5 = distance(_TotalCenter, IN.worldPos);



				half sum = saturate((dist - _RadiusPlayer) / - _SoftnessPlayer);
				half sum2 = saturate((dist2 - _Radius) / -_Softness);
				half sum3 = saturate((dist3 - _Radius) / -_Softness);
				half sum4 = saturate((dist4 - _Radius) / -_Softness);

				half sum5 = saturate((_TotalRadius - dist5) / -_TotalSoftness);



				o.Alpha = lerp(1.0, 0.0, sum) * lerp(1.0, 0.0, sum2) * lerp(1.0, 0.0, sum3) * lerp(1.0, 0.0, sum4) * lerp(1.0, 0.0, sum5);

				o.Albedo = c.rgb * _MainColor;
			}
				
			
			ENDCG
		}
}
