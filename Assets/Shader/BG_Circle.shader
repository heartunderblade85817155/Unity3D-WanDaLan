// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Unlit/BG_Circle"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_OldCellPos ("OldCellPos", Vector) = (-100, -100, 0, 0)
		_NewCellPos ("NewCellPos", Vector) = (-100, -100, 0, 0)
		_Threshold ("ThresholdValue", Float) = 2
		_Radius ("Radius", Float) = 3
		_MetaBallColor ("MetaBallColor", Color) = (0.3, 0.6, 0.9, 1.0)
		_XingZhuang ("XingZhuang", int) = 1
		_Bigger ("BecomeBigger", float) = 1
	}
	SubShader
	{
		Tags{ "Queue" = "Transparent" }

		Pass
		{
			stencil
			{
				Ref 100
				Comp Greater
				Pass Keep
				Fail Keep
			}

			Blend OneMinusDstAlpha DstAlpha

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
				float4 vertex : SV_POSITION;
				float4 worldVertex : TEXCOORD1;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _OldCellPos;
			float4 _NewCellPos;
			float _Threshold;
			float _Radius;
			fixed4 _MetaBallColor;
			int _XingZhuang;
			float _Bigger;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.worldVertex = mul(unity_ObjectToWorld, v.vertex);

				return o;
			}

			float GetEnergy(in float3 Origin, in float3 WorldPos, in int XingZhuang)
			{
				float2 Dir = WorldPos.xy - Origin.xy;
				Dir = normalize(Dir);

				float Cos, Sin;
				float XiShu, Energy;
				
				// 计算菱形
				if (XingZhuang == 1)
				{
					Cos = abs(dot(Dir, float2(1.0f, 0.0f)));
					Sin = abs(sqrt(1 - Cos * Cos));
					XiShu = (Cos + Sin);
					Energy = (_Radius * _Radius) / ((pow((Origin.x - WorldPos.x), 2) + pow((Origin.y - WorldPos.y), 2)) * XiShu * XiShu);
				}

				return Energy;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);

				if (col.a < 0.6f)
				{
					discard;
				}

				float Energy = GetEnergy(_OldCellPos.xyz, i.worldVertex.xyz, _XingZhuang);
				Energy += GetEnergy(_NewCellPos.xyz, i.worldVertex.xyz, _XingZhuang);

				if (Energy >= (_Threshold - _Bigger))
				{
					col = _MetaBallColor;
				}

				return col;
			}
			ENDCG
		}
	}
}
