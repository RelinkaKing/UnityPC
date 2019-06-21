// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "MyShader/Xuewei555"
{
	Properties{
		_Color("_Color",Color) = (0,0,0,0)
		_MainTex("MainTex",2D) = ""{}
	_AlphaScale("AlphaScale",Range(0,1)) = 1
		_OutLineScale("OutLineScale",Range(0,1))=1
	}

		SubShader{
		Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" "IgnoreProjector" = "True" }
		Pass{
		ZWrite on
		ColorMask 0
	}

		Pass{
		Tags{ "LightMode" = "ForwardBase" }
		ZWrite off
		Blend SrcAlpha OneMinusSrcAlpha
		CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma target 4.0

#include "UnityCG.cginc"
#include "Lighting.cginc"

		fixed4 _Color;
		sampler2D _MainTex;
	fixed _Cutoff;
	fixed _AlphaScale;
	fixed _OutLineScale;

	struct a2v {
		float4 _vertex:POSITION;
		float3 _normal:NORMAL;
		float2 _uv:TEXCOORD0;
	};

	struct v2f {
		float4 pos:SV_POSITION;
		float3 worldNormal:TEXCOORD0;
		float2 uv:TEXCOORD1;
	};

	v2f vert(a2v v) {
		v2f o;
		half3 _offset = normalize(v._normal)*_OutLineScale*abs(sin(_Time.y));
		v._vertex.xyz += _offset;
		
		o.pos = UnityObjectToClipPos(v._vertex);
		o.worldNormal = normalize(mul(unity_ObjectToWorld,v._normal));
		o.uv = v._uv;
		return o;
	}
	fixed4 frag(v2f i) :SV_Target
	{
		fixed4 col = tex2D(_MainTex,i.uv);
	fixed NdotL = saturate(dot(i.worldNormal,normalize(WorldSpaceLightDir(i.pos))));
	fixed3 _diffuse = _LightColor0.rgb*NdotL;
	float3 _ambient = UNITY_LIGHTMODEL_AMBIENT.rgb;
	col = fixed4(col.rgb*_Color.rgb*(_diffuse + _ambient),col.a*_AlphaScale);
	return col;
	}

		ENDCG
	}
	}

		FallBack "Transparent/VertexLit"
}