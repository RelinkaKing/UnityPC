Shader "MyShader/SignSelect"
{
	Properties{
		_MainTex("MainTex[主纹理贴图]",2D) = ""{}
	    
	    //_BiTex("_BiTex[副纹理贴图]",2D) = ""{}
	    _NormalMap("NormalMap[法线贴图]",2D) = ""{}
		//_SpecularTex("SpecularTex[高光贴图]",2D) = ""{}
		_Specular("Specular(RGB)",Color) = (0.3,0.3,0.3,1)
		_MaskTex("MaskTex[遮罩纹理]",2D) = "white" {}
	
	    _Brightness("Brightness[亮度系数]",Range(0,10)) = 0.6
		_Gloss("Gloss[高光范围]",Range(0.01,11))=0.5
		_ChooseColor("ChooseColor[获取选择颜色]",Color) = (1,1,1,1)
		_ResponseColor("ResponseColor[获取响应颜色]",Color) = (1,1,1,1)
		_SelectedColor("SelectedColor[选择色替换色]",Color) = (1,0,1,1)
		_AnsweredColor("AnsweredColor[响应色替换色]",Color) = (0,0,1,1)
	    _Thresh("_Thresh",float)=0.001
		
	}
		SubShader{
		Tags{ "RenderType" = "Opaque" }
		Pass{
		Tags{ "LightMode" = "ForwardBase" }

		CGPROGRAM

#pragma vertex vert
#pragma fragment frag


#include "UnityCG.cginc"
#include "Lighting.cginc"
#include"AutoLight.cginc"
	
	sampler2D _MainTex;
	float4 _MainTex_ST;
	//sampler2D _BiTex;
	sampler2D _NormalMap;
	//sampler2D _SpecularTex;
	fixed4 _Specular;
	sampler2D _MaskTex;
	float4  _MaskTex_TexelSize;
	float _Gloss;
	float _Brightness;
	fixed4 _ChooseColor;
	fixed4 _ResponseColor;
	fixed4 _SelectedColor;
	fixed4 _AnsweredColor;
	float _Thresh;
	
	
	
	

	/*struct v2f {
	float4 pos:SV_POSITION;
	float2 uv[9]:TEXCOORD0;
	};*/

	struct a2v {
		float4 vertex : POSITION;
		float3 normal : NORMAL;
		float4 texcoord : TEXCOORD0;
		float4 tangent:TANGENT;
	};


	struct v2f {
		float4 pos : SV_POSITION;
		float3 worldNormal : TEXCOORD0;
		float3 worldPos : TEXCOORD3;
		float2 uv1 : TEXCOORD2;
		//float2 uv[9]:TEXCOORD3;
		float3 lightdir:TEXCOORD1;
	};
	v2f vert(a2v v) {
		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.worldNormal = UnityObjectToWorldNormal(v.normal);
		o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
		o.uv1 = TRANSFORM_TEX(v.texcoord, _MainTex);
		


		//float2 uv = v.texcoord;
		
		/*o.uv[0] = uv + _MaskTex_TexelSize.xy*fixed2(-1,-1);
		o.uv[1] = uv + _MaskTex_TexelSize.xy*fixed2(0,-1);
		o.uv[2] = uv + _MaskTex_TexelSize.xy*fixed2(1,-1);
		o.uv[3] = uv + _MaskTex_TexelSize.xy*fixed2(-1,0);
		o.uv[4] = uv + _MaskTex_TexelSize.xy*fixed2(0,0);
		o.uv[5] = uv + _MaskTex_TexelSize.xy*fixed2(1,0);
		o.uv[6] = uv + _MaskTex_TexelSize.xy*fixed2(-1,1);
		o.uv[7] = uv + _MaskTex_TexelSize.xy*fixed2(0,1);
		o.uv[8] = uv + _MaskTex_TexelSize.xy*fixed2(1,1);*/

		float3 l = ObjSpaceLightDir(v.vertex);

		TANGENT_SPACE_ROTATION;

		o.lightdir = normalize(mul(rotation, l));
		return o;
	}
	////将颜色转化为饱和度，方便卷积运算
	//fixed Luminance(fixed4 col) {
	//	return col.r*0.2125 + col.g*0.7154 + col.b*0.0721;
	//}
	////sobel卷积核进行卷积
	//fixed Sobel(v2f i) {
	//	//横向边界检查的卷积核
	//	half Gx[9] = { -1,-2,-1,
	//		0,0,0,
	//		1,2,1 };
	//	//纵向边界检查的卷积核
	//	half Gy[9] = { -1,0,1,
	//		-2,0,2,
	//		-1,0,1 };
	//	//累加横，纵向卷积结果的临时量
	//	fixed x = 0;
	//	fixed y = 0;
	//	//循环卷积，（最好展开手写），包括自身周围九个像素点的全部计算一遍
	//	for (int j = 0; j<9; j++) {
	//		//对于不同uv进行纹理采样，获得该像素的颜色
	//		fixed4 col = tex2D(_MaskTex,i.uv[j]);
	//		//把颜色转换成饱和度来计算累加
	//		fixed lu = Luminance(col);
	//		//分别进行横向和纵向的卷积，在其判断方向上，颜色差异越大，累加结果的绝对值越大，如果颜色都一样，那么累加结果就是0
	//		x += lu * Gx[j];
	//		y += lu * Gy[j];
	//	}
	//	//1减去两个方向累加的绝对值，近似体现该点周围颜色变化的梯度，
	//	//但是结果反置，结果越大->梯度越小->颜色没太大变化->色块内部,
	//	//结果越小->梯度越大->颜色变化明显->色块边缘
	//	fixed edge = 1 - abs(x) - abs(y);
	//	return edge;
	//}
	fixed4 frag(v2f i) :SV_TARGET{
		float3 normal = normalize(UnpackNormal(tex2D(_NormalMap,i.uv1)));
		//fixed3 worldNormal = normalize(i.worldNormal);
	   // fixed3 worldLightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));

		//fixed3 worldLightDir = normalize(_WorldSpaceLightPos0.xyz);

		fixed3 viewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldPos.xyz);

		fixed3 halfDir = normalize(i.lightdir + viewDir);

	fixed4 texColor = tex2D(_MainTex, i.uv1);
	//fixed4 biColor = tex2D(_BiTex, i.uv1);
	//fixed4 specularColor = tex2D(_SpecularTex, i.uv1);
	fixed4 maskColor = tex2D(_MaskTex, i.uv1);

	fixed3 albedo = texColor.rgb;//*_Brightness;

	
	float chooseValue = step(3, step(abs(_ChooseColor.r - maskColor.r), _Thresh) + step(abs(_ChooseColor.g - maskColor.g), _Thresh) + step(abs(_ChooseColor.b - maskColor.b), _Thresh));
	albedo = lerp(albedo, _SelectedColor.rgb, chooseValue);


	float responseValue=step(3, step(abs(_ResponseColor.r - maskColor.r), _Thresh) + step(abs(_ResponseColor.g - maskColor.g), _Thresh) + step(abs(_ResponseColor.b - maskColor.b), _Thresh));

	albedo = lerp(albedo, _AnsweredColor.rgb, responseValue);
	
	//fixed edge = saturate(Sobel(i));
	//fixed4 col = fixed4(albedo, 1.0);
	


	//fixed4 Color = col;
	//fixed4 Color = lerp((biColor+col)/2, col, edge);
	
	
	fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz;
	fixed3 diffuse = _LightColor0.rgb  * saturate(dot(normal, i.lightdir));
	fixed3 specular = _LightColor0.rgb*_Specular.rgb * pow(saturate(dot(normal, halfDir)), _Gloss);
	UNITY_LIGHT_ATTENUATION(attan, i, i.worldPos.xyz);
	albedo *= (ambient+diffuse+specular)*attan /*+ specular)*//**attan*/;
	fixed4 col = fixed4(albedo, 1.0)*_Brightness;

	return col;

	
	}

		ENDCG
	}
	}
}
