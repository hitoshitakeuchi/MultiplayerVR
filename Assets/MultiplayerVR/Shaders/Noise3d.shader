// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Unlit/Noise3d"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	CGINCLUDE
		#include "UnityCG.cginc"

		struct appdata
		{
			float4 vertex : POSITION;
			float2 uv : TEXCOORD0;
		};

		struct v2f
		{
			float4 vertex : SV_POSITION;
			float2 uv : TEXCOORD0;
			float3 wPos : TEXCOORD1;
		};

		sampler2D _MainTex,_NoiseTex;
		float4 _MainTex_ST;
		
		v2f vert (appdata v)
		{
			float3 wPos = mul(unity_ObjectToWorld, v.vertex).xyz;
			v2f o;
			o.vertex = mul(UNITY_MATRIX_VP, float4(wPos,1));
			o.uv = TRANSFORM_TEX(v.uv, _MainTex);
			o.wPos = v.vertex.xyz;
			return o;
		}
		
		half4 frag (v2f i) : SV_Target
		{
			half3 pos = i.wPos/10;
			pos.y -= _Time.x;
			half4 c = 0;
			half v = 1;
			for(int i = 0; i<10; i++){
				c += v*(tex2D(_NoiseTex, pos.xy+pos.z) + tex2D(_NoiseTex, pos.zx+pos.y));
				pos *= 2;
				v *= -0.5;
			}

			return c.b*1.5;
		}
	ENDCG
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			ENDCG
		}
	}
}
