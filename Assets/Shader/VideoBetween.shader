Shader "Custom/VideoBetween"
{


  Properties {
//    _BlurAmount("Blur Amount" , float) = 0,
    _blurSizeXY("BlurSizeXY", Range(0,10)) = 2
  }
 
  SubShader {
    // Draw ourselves after all opaque geometry
    Tags {
      "Queue" = "Transparent"
    }
    LOD 100
    cull off

  	GrabPass {}
   
    Pass
    {
      CGPROGRAM
      #pragma vertex vert
      #pragma fragment frag
      #include "UnityCG.cginc"
      sampler2D _GrabTexture;
      struct vin_vct
      {
        float4 vertex : POSITION;
      };
      struct v2f_vct
      {
        float4 vertex : POSITION;
        float4 uvgrab : TEXCOORD1;
      };
      // Vertex function
      v2f_vct vert (vin_vct v)
      {
        v2f_vct o;
        o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
        o.uvgrab = ComputeGrabScreenPos(o.vertex);
        return o;
      }
      // Fragment function
      half4 frag (v2f_vct i) : COLOR
      {
        fixed4 col = tex2Dproj( _GrabTexture, UNITY_PROJ_COORD(i.uvgrab));
        return col;
      }
   
      ENDCG
    }

//    GrabPass {}
//
//    Pass
//    {
//      CGPROGRAM
//      #pragma vertex vert
//      #pragma fragment frag
//      #include "UnityCG.cginc"
//
//	  sampler2D _GrabTexture : register(s0);
//	  float4 _GrabTexture_ST;
//    float _BlurAmount;
//
//	struct appdata
//	{
//		float4 vertex : POSITION;
//		float2 uv : TEXCOORD0;
//	};
//
//	struct v2f
//	{
//		float2 uv : TEXCOORD0;
//		UNITY_FOG_COORDS(1)
//		float4 vertex : SV_POSITION;
//	};
//
//      // Vertex function
//      v2f vert (appdata_base v)
//      {
//        v2f o;
//        o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
//        o.uv = TRANSFORM_TEX(v.texcoord, _GrabTexture);
//        return o;
//      }
//      // Fragment function
//      half4 frag (v2f i) : COLOR
//      {  
//
//        half4 sum = half4(0,0,0,1);
//
//         float blurDistance = _BlurAmount * 0.01f;
//        
//         sum += tex2D(_GrabTexture, float2(i.uv.x, i.uv.y - 5.0 * blurDistance)) * 0.025;
//         sum += tex2D(_GrabTexture, float2(i.uv.x, i.uv.y - 4.0 * blurDistance)) * 0.05;
//         sum += tex2D(_GrabTexture, float2(i.uv.x, i.uv.y - 3.0 * blurDistance)) * 0.09;
//         sum += tex2D(_GrabTexture, float2(i.uv.x, i.uv.y - 2.0 * blurDistance)) * 0.12;
//         sum += tex2D(_GrabTexture, float2(i.uv.x, i.uv.y - blurDistance)) * 0.15;
//         sum += tex2D(_GrabTexture, float2(i.uv.x, i.uv.y)) * 0.16;
//         sum += tex2D(_GrabTexture, float2(i.uv.x, i.uv.y + blurDistance)) * 0.15;
//         sum += tex2D(_GrabTexture, float2(i.uv.x, i.uv.y + 2.0 * blurDistance)) * 0.12;
//         sum += tex2D(_GrabTexture, float2(i.uv.x, i.uv.y + 3.0 * blurDistance)) * 0.09;
//         sum += tex2D(_GrabTexture, float2(i.uv.x, i.uv.y + 4.0 * blurDistance)) * 0.05;
//         sum += tex2D(_GrabTexture, float2(i.uv.x, i.uv.y + 5.0 * blurDistance)) * 0.025;
//
//         return sum;
//      
//      }
//   
//      ENDCG
//    }


  }
 
  Fallback Off

//	Properties
//	{
//                _MainTex ("Texture", 2D) = "white" {} 
//	}
//	SubShader
//	{
//		Tags { "Queue" = "Transparent" }
//		LOD 100
//		cull off
//
//		Pass
//		{
//			CGPROGRAM
//			#pragma vertex vert
//			#pragma fragment frag
//			// make fog work
//			#pragma multi_compile_fog
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
//				UNITY_FOG_COORDS(1)
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
//				o.uv = TRANSFORM_TEX(v.texcoord, _GrabTexture);
//				UNITY_TRANSFER_FOG(o,o.vertex);
//				return o;
//			}
//			
//			fixed4 frag (v2f i) : SV_Target
//			{                
//                float4 sum = float4(0.0, 0.0, 0.0, 0.0);
//                float2 tc = i.uv;
//
//                //blur radius in pixels
//                float blur = radius/resolution/4; 
//
//                //the direction of our blur
//                //(1.0, 0.0) -> x-axis blur
//                //(0.0, 1.0) -> y-axis blur
//
//                float hstep = 1;
//                float vstep = 0;
//
//                sum += tex2D(_MainTex, float2(tc.x - 4.0*blur*hstep, tc.y - 4.0*blur*vstep)) * 0.0162162162;
//                sum += tex2D(_MainTex, float2(tc.x - 3.0*blur*hstep, tc.y - 3.0*blur*vstep)) * 0.0540540541;
//                sum += tex2D(_MainTex, float2(tc.x - 2.0*blur*hstep, tc.y - 2.0*blur*vstep)) * 0.1216216216;
//                sum += tex2D(_MainTex, float2(tc.x - 1.0*blur*hstep, tc.y - 1.0*blur*vstep)) * 0.1945945946;
//
//                sum += tex2D(_MainTex, float2(tc.x, tc.y)) * 0.2270270270;
//
//                sum += tex2D(_MainTex, float2(tc.x + 1.0*blur*hstep, tc.y + 1.0*blur*vstep)) * 0.1945945946;
//                sum += tex2D(_MainTex, float2(tc.x + 2.0*blur*hstep, tc.y + 2.0*blur*vstep)) * 0.1216216216;
//                sum += tex2D(_MainTex, float2(tc.x + 3.0*blur*hstep, tc.y + 3.0*blur*vstep)) * 0.0540540541;
//                sum += tex2D(_MainTex, float2(tc.x + 4.0*blur*hstep, tc.y + 4.0*blur*vstep)) * 0.0162162162;
//
//
//                return float4(sum.rgb, 1);
//			}
//			ENDCG
//		}
//	}
//       
//  }
// 
//  Fallback Off
}
