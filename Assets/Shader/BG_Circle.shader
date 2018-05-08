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
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.worldVertex = mul(unity_ObjectToWorld, v.vertex);

				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);

				if (col.a < 0.6f)
				{
					discard;
				}

				float Energy = (_Radius * _Radius) / (pow((_OldCellPos.x - i.worldVertex.x), 2) + pow((_OldCellPos.y - i.worldVertex.y), 2));
				Energy += (_Radius * _Radius) / (pow((_NewCellPos.x - i.worldVertex.x), 2) + pow((_NewCellPos.y - i.worldVertex.y), 2));

				if (Energy >= _Threshold)
				{
					col = _MetaBallColor;
				}

				return col;
			}
			ENDCG
		}
	}
}
