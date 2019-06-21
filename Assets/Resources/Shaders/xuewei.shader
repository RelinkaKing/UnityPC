Shader "MyShader/xueweiFun"
{
	Properties{
	

    _Color("_Color",Color)=(0,0,0,0)
	_MainTex ("_MainTex",2D) = ""{}
	_Speed1("speed1",Range(-10,10)) = 1

	}
		SubShader{
		Pass{
		CGPROGRAM
#pragma vertex vert
#pragma fragment frag

#include"UnityCG.cginc"


	fixed4 _Color;
	sampler2D _MainTex;
	fixed4 _MainTex_ST;
	half _Speed1;

	

	struct a2v {
		float4 vertex:POSITION;
		float2 texcoord:TEXCOORD0;
	};

	struct v2f {
		float4 pos:SV_POSITION;
		float2 uv:TEXCOORD0;
	};

	v2f vert(a2v v) {
		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv = v.texcoord;
		return o;
	}

	fixed4 frag(v2f i) :SV_TARGET{
		

	float2 uv1 = TRANSFORM_TEX(i.uv,_MainTex);
	uv1.x += _Time.x*_Speed1;
	fixed4 col = tex2D(_MainTex,uv1);
	
	
	col = col * _Color;

	return col;
	}

		ENDCG
	}
	}
}