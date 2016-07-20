Shader "Custom/FishEye2Sphere"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100
		cull off

		Pass
		{
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
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_TexelSize;
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
				float2 pfish;
				float theta,phi,r;
				float3 psph;

				float FOV = 3.141592654 * 230 / 180; // FOV of the fisheye, eg: 180 degrees
//				float width = _MainTex_TexelSize.z;
//				float height = _MainTex_TexelSize.w;
				float width = 1;
				float height = 1;

				// Polar angles
				theta = 2.0 * 3.14159265 * (( 1 - i.uv.x) / width - 0.5); // -pi to pi
				phi = 3.14159265 * (i.uv.y / height - 0.5);	// -pi/2 to pi/2

				// Vector in 3D space
				psph.x = cos(phi) * sin(theta);
				psph.y = cos(phi) * cos(theta);
				psph.z = sin(phi);
				
				// Calculate fisheye angle and radius
				theta = atan2(psph.z,psph.x);
				phi = atan2(sqrt(psph.x*psph.x+psph.z*psph.z),psph.y);
				r = width * phi / FOV; 

				// Pixel in fisheye space
				pfish.x = 0.5 * width + r * cos(theta) ;
				pfish.y = 0.5 * width + r * sin(theta);

				// sample the texture
				fixed4 col = tex2D(_MainTex, pfish);
				return col;
			}
			ENDCG
		}
	}
}
