// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Fade" {
	Properties
	{
		_Color("Tint", Color) = (1,1,1,1)
		_MainTex("Texture", 2D) = "white" {}
	    _T("t",float) = 0
		_F2("fade",Range(0,1)) = 1
	}

		CGINCLUDE
#include "UnityCG.cginc"
#include "Assets/Packages/Kitte/Shaders/Libs/PhotoshopMath.cginc"

		sampler2D _MainTex;
	    half _T, _F2;
	    fixed4 _Color;


	struct v2f {
		float4 pos : SV_POSITION;
		fixed4 color : COLOR;
		float2 texcoord : TEXCOORD0;
		float4 gPos : TEXCOORD1;
		float4 sPos : TEXCOORD2;
	};

	v2f vert(appdata_full v)
	{
		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.color = v.color;
		o.texcoord = v.texcoord;
		o.gPos = ComputeGrabScreenPos(o.pos);
		o.sPos = ComputeScreenPos(o.pos);
		return o;
	}

	half4 frag(v2f i) : COLOR
	{

		half2 uv = i.texcoord / half2(_TileX, _TileY);

		half4 c = tex2D(_MainTex, uv);

		half2 gUV = i.gPos.xy / i.gPos.w;

		float3 p = i.sPos.xyz / i.sPos.w;

		float width = 0.3;
		float af = 1;
		if (p.y > 1 - width) af *= (1 - p.y) / width;
		c.a = pow(af, 2) * _F2;

		return c;
	}
		ENDCG

		SubShader
	{
		Lighting Off
			ZTest LEqual
			ZWrite Off
			Tags{
			"RenderType" = "Opaque" // The material is opaque
			"IgnoreProjector" = "True" // Don't let projectors affect this object
			"Queue" = "Transparent"
		}
			Pass
		{
			Alphatest Greater 0
			Blend SrcAlpha OneMinusSrcAlpha
			//Offset - 1, -1
			SetTexture[_MainTex]
		{
			ConstantColor[_Color]
			Combine texture * constant
		}
		}
	}
}
