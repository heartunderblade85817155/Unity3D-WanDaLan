// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Unlit/BG_Circle"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Threshold ("ThresholdValue", Float) = 2
		_MetaBallColor ("MetaBallColor", Color) = (0.3, 0.6, 0.9, 1.0)
		_Bigger ("BecomeBigger", float) = 1
		_SmallCircleNum ("SmallCircleNum", int) = 0
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

			uniform float4 _OldAndNewCellsPos[32];
			uniform float _CellRadius[32];
			uniform float _CellShapes[32];

			float _Threshold;
			fixed4 _MetaBallColor;
			float _Bigger;
			int _SmallCircleNum;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.worldVertex = mul(unity_ObjectToWorld, v.vertex);

				return o;
			}

			float GetEnergy(in float2 Origin, in float2 WorldPos, in float TheRadius, in float Shape)
			{
				float2 Dir = float2(WorldPos - Origin);
				Dir = normalize(Dir);

				float Cos, Sin;
				float XiShu, Energy;
				
				// 计算圆形
				if (Shape == 0)
				{
					Energy = (TheRadius * TheRadius) / (pow((Origin.x - WorldPos.x), 2) + pow((Origin.y - WorldPos.y), 2));
				}
				// 计算菱形
				else if (Shape == 1)
				{
					Cos = abs(dot(normalize(Dir.xy), float2(1.0f, 0.0f)));
					Sin = abs(sqrt(1 - Cos * Cos));
					XiShu = (Cos + Sin);
					Energy = (TheRadius * TheRadius) / ((pow((Origin.x - WorldPos.x), 2) + pow((Origin.y - WorldPos.y), 2)) * XiShu * XiShu);
				}
				// 计算正方形
				else if (Shape == 2)
				{
					Cos = dot(normalize(Dir.xy), normalize(float2(1.0f, -1.0f)));
					Sin = abs(sqrt(1 - Cos * Cos));
					XiShu = (abs(Cos) + Sin);
					Energy = (TheRadius * TheRadius) / ((pow((Origin.x - WorldPos.x), 2) + pow((Origin.y - WorldPos.y), 2)) * XiShu * XiShu);
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

				float Energy = 0.0f;

				for (int it = 0; it < _SmallCircleNum; ++it)
				{
					Energy += GetEnergy(_OldAndNewCellsPos[it].xy, i.worldVertex.xy, _CellRadius[it], _CellShapes[it]);
					Energy += GetEnergy(_OldAndNewCellsPos[it].zw, i.worldVertex.xy, _CellRadius[it], _CellShapes[it]);
				}

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
