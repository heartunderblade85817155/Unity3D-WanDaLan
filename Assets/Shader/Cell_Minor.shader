﻿Shader "Unlit/Cell_Minor"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_StencilValue ("StencilValue", int) = 0
		_AlphaBlend ("AlphaBlend", float) = 0.5
	}
	SubShader
	{
		Tags{ "Queue" = "Transparent" }

		Pass
		{
			stencil
			{
				Ref [_StencilValue]
				Comp Less
				Pass Zero
				Fail Keep
			}


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
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _AlphaBlend;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				if (col.a == 0)
				{
					discard;
				}
				return fixed4(col.rgb, _AlphaBlend);
			}
			ENDCG
		}
	}
}
