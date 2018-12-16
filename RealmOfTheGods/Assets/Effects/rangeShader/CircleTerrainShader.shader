Shader "Custom/TerrainCircle" {
	Properties{
		_MainTex("Texture",2D)= "white"{}
		_MainColor("Main Color", Color) = (0, 1, 0)
		_Radius ("Radius", Range(0, 100)) = 100
		_Thickness("Thickness", Range(0, 100)) = 5

		_CircleColor("Circle Color", Color) = (1,0,0)
		_Center("Center", Vector) = (0,0,0,0)

		_CircleColor2("Circle Color 2", Color) = (1,0,0)
		_Center2("Center 2", Vector) = (0,0,0,0)
	}

		SubShader{
			CGPROGRAM
			#pragma surface surfaceFunc Lambert

			sampler2D _MainTex;
			fixed3 _MainColor;
			float _Thickness;
			float _Radius;

			fixed3 _CircleColor;
			float3 _Center;

			fixed3 _CircleColor2;
			float3 _Center2;

			

			struct Input {
				float2 uv_MainTex;
				float3 worldPos;
				  
			};

			void surfaceFunc(Input IN, inout SurfaceOutput o){

				half4 c = tex2D(_MainTex, IN.uv_MainTex);
				float dist = distance(_Center, IN.worldPos);
				float dist2 = distance(_Center2, IN.worldPos);

				if(dist > _Radius && dist < (_Radius + _Thickness))
				{
					o.Albedo = c.rgb * _CircleColor;
				}
				else if(dist2 > _Radius && dist2 < (_Radius + _Thickness))
				{
					o.Albedo = c.rgb * _CircleColor2;
				}
				else
				{
					o.Albedo = c.rgb * _MainColor;
				}
				//o.Alpha = c.a;

			}
			ENDCG

		}
}
