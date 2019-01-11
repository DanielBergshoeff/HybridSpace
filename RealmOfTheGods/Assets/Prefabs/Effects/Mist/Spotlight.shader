Shader "Unlit/Spotlight"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_CharacterPosition("Char pos", vector) = (0, 0, 0, 0)
		_CircleRadius("Spotlight size", Range(0, 20)) = 3
		_RingSize("Ring size", Range(0, 5)) = 1
		_Color("Color", Color) = (0, 0, 0, 0)
		_Alpha("Alpha", Range(0.0, 1.0)) = 0.25
	}
		SubShader
		{
			Tags {"Queue" = "Transparent" "RenderType" = "Transparent" }
			LOD 100
			
			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha

			Pass
			{
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				// make fog work
				#pragma multi_compile_fog
				#pragma target 3.0

				#include "UnityCG.cginc"

				struct appdata
				{
					float4 vertex : POSITION;
					float2 uv : TEXCOORD0;
				};

				struct v2f
				{
					float2 uv : TEXCOORD0;
					UNITY_FOG_COORDS(1)
					float4 vertex : SV_POSITION;
					float3 worldPos : TEXCOORD1;
				};

				sampler2D _MainTex;
				float4 _MainTex_ST;

				float4 _CharacterPosition;
				float4 _CircleRadius;
				float _RingSize;
				float4 _Color;
				float _Alpha;

				v2f vert(appdata v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.uv = TRANSFORM_TEX(v.uv, _MainTex);
					UNITY_TRANSFER_FOG(o,o.vertex);

					o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

					return o;
				}

				fixed4 frag(v2f i) : SV_Target
				{
					// sample the texture
					fixed4 col = tex2D(_MainTex, i.uv);

					float dist = distance(i.worldPos.xz, _CharacterPosition.xz);
					// apply fog
					UNITY_APPLY_FOG(i.fogCoord, col);
					col.a = dist / _CircleRadius;
					col *= _Color;

					return col;
				}
			ENDCG
			}
		}
}
