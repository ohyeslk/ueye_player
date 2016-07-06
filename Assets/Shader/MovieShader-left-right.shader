Shader "Custom/MovieShader-left-right"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
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
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				float2 uv = i.uv;

			//	uv.x = 1 - uv.x;
				fixed4 col = tex2D(_MainTex, uv);
	//			float4 black = float4(0, 0, 0, 1);

//				float fadeOutEdge = 0.02;
//				if (uv.y > 1 || uv.y < 0)
//				{
//					float process = 0;
//					if (uv.y > 0.5) {
//						process = clamp((uv.y - 1) / fadeOutEdge, 0, 1);
//					}
//					else {
//						process = clamp((0 - uv.y) / fadeOutEdge, 0, 1);
//					}
//					col = lerp(col, black, process);  
//				}

//				if (uv.x > 1 - fadeOutEdge || uv.x < fadeOutEdge)
//				{
//					float process = 0;
//					if (uv.x > 0.5 ) {
//						process = clamp((uv.x - ( 1 - fadeOutEdge) ) / fadeOutEdge, 0, 1);
//					}
//					else {
//						process = clamp(( fadeOutEdge - uv.x ) / fadeOutEdge, 0, 1);
//					}
//					col = lerp(col, black, process);
//				}

				return col;
			}

			ENDCG
		}
	}
}
