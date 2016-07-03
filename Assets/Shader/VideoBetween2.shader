Shader "Custom/VideoBetween2"
{

	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
//        _Color ("Main Color", COLOR) = (1,1,1,1)
//		_CoverTex ("Cover" , 2D) = "white" {}
//        _FadePos("Fade Position" , Range(0,1)) = 0.5
//        _FadeRange("Fade Range" , Range(0,1)) = 0.1
//        _BlurAmount("Blur Amount" , float) = 0
//        _BlurRange("Blur Range " , Int ) = 0
	}
	SubShader
	{
	    Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
	    Blend SrcAlpha OneMinusSrcAlpha
	    Cull Off
	    LOD 200

		// cover
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
//				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
//			sampler2D _CoverTex;
			float4 _MainTex_ST;
//			fixed4 _Color;
//			float _FadePos;
//			float _FadeRange;
//			float4 _CoverRec[10];
//			int _TemIndex;
//			int _CountNum;
//			float _BlurAmount;
//			int _BlurRange;


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
				fixed4 col = tex2D(_MainTex, i.uv);
				col.r = col.g = col.b = 0;
				col.a = i.uv.x;
				return col ;
			}

			ENDCG
		}
	}
Fallback "Diffuse"

//	Properties
//	{
//		_MainTex ("Texture", 2D) = "white" {}
//	}
//	SubShader
//	{
//		Tags {  "RenderType" = "Transparent" "Queue" = "Transparent" }
//		Blend SrcAlpha OneMinusSrcAlpha
//		LOD 100
//		cull off
//
//		Pass
//		{
//			CGPROGRAM
//			#pragma vertex vert
//			#pragma fragment frag
//			// make fog work
//			
//			#include "UnityCG.cginc"
//
//			struct appdata
//			{
//				float4 vertex : POSITION;
//				float2 uv : TEXCOORD0;
//			};
//
//			struct v2f
//			{
//				float2 uv : TEXCOORD0;
//				float4 vertex : SV_POSITION;
//			};
//
//			sampler2D _MainTex;
//			float4 _MainTex_ST;
//			
//			v2f vert (appdata v)
//			{
//				v2f o;
//				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
//				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
//				return o;
//			}
//			
//			fixed4 frag (v2f i) : SV_Target
//			{
//				// sample the texture
////				fixed4 col = tex2D(_MainTex, i.uv);
//				fixed4 col = fixed(0,0,0,0.5);
//				col.a = 0.5;
//				return col;
//			}
//			ENDCG
//		}
//	}
}
