Shader "Unlit/SPhere1"
{
	Properties{
		_MainTex("Base (RGB)", 2D) = "white" {}
	_AlphaBlendTex("Alpha Blend (RGBA)", 2D) = "white" {}
	_OffsetU("Offset U", Range(-0.5, 0.5)) = 0
		_OffsetV("Offset V", Range(-0.5, 0.5)) = 0
		_ScaleU("Scale U", Range(0.8, 1.2)) = 1
		_ScaleV("Scale V", Range(0.8, 1.2)) = 1
		_ScaleCenterU("Scale Center U", Range(0.0, 1.0)) = 0
		_ScaleCenterV("Scale Center V", Range(0.0, 1.0)) = 0
	}
		SubShader{
		Tags{ "RenderType" = "Transparent" "Queue" = "Background" }
		Pass{
		Name "BASE"

		Blend SrcAlpha OneMinusSrcAlpha
		Lighting Off
		//ZWrite Off
		

		CGPROGRAM
#pragma vertex vert_img
#pragma fragment frag

#include "UnityCG.cginc"

	uniform sampler2D _MainTex;
	uniform sampler2D _AlphaBlendTex;
	uniform float _OffsetU;
	uniform float _OffsetV;
	uniform float _ScaleU;
	uniform float _ScaleV;
	uniform float _ScaleCenterU;
	uniform float _ScaleCenterV;

	float4 frag(v2f_img i) : COLOR{
		
		float2 uvCenter = float2(_ScaleCenterU, _ScaleCenterV);
		float2 uvOffset = float2(_OffsetU, _OffsetV);
		float2 uvScale = float2(_ScaleU, _ScaleV);
		float2 uv = (i.uv - uvCenter) * uvScale + uvCenter + uvOffset;
		/*if (uv.x > 0.5) {
			uv.x = 1 - (uv.x - 0.5);
		}*/
		
		
		float4 tex = tex2D(_MainTex, uv);
//		tex.a *= pow(1.0 - tex2D(_AlphaBlendTex, i.uv).a, 0.5);

		//if (uv.x > 0.47 && uv.x < 0.53)
		//{
		//	tex += tex2D(_MainTex, float2(i.uv.x +0.01, i.uv.y +0.01)) * 0.025;
		//	tex += tex2D(_MainTex, float2(i.uv.x + 0.01 , i.uv.y - 0.01)) * 0.25;
		//	tex += tex2D(_MainTex, float2(i.uv.x -0.01 , i.uv.y - 0.01)) * 0.29;
		//	tex += tex2D(_MainTex, float2(i.uv.x - 0.01 , i.uv.y + 0.01)) * 0.25;
		//	/*tex += tex2D(_MainTex, float2(i.uv.x 0.1, i.uv.y - 0.1)) * 0.15;
		//	tex += tex2D(_MainTex, float2(i.uv.x, i.uv.y)) * 0.16;
		//	tex += tex2D(_MainTex, float2(i.uv.x + 1.0 * depth, i.uv.y + 1.0 * depth)) * 0.15;
		//	tex += tex2D(_MainTex, float2(i.uv.x + 2.0 * depth, i.uv.y + 2.0 * depth)) * 0.12;
		//	tex += tex2D(_MainTex, float2(i.uv.x + 3.0 * depth, i.uv.y + 3.0 * depth)) * 0.09;
		//	tex += tex2D(_MainTex, float2(i.uv.x + 4.0 * depth, i.uv.y + 4.0 * depth)) * 0.05;
		//	tex += tex2D(_MainTex, float2(i.uv.x + 5.0 * depth, i.uv.y + 5.0 * depth)) * 0.025;*/
		//}

		return tex;
	}
		ENDCG
	}
	}
}
