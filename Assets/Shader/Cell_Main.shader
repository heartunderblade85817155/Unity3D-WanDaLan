Shader "Unlit/Cell_Main"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_StencilValue ("StencilValue", int) = 0
		_BlendAlpha ("Alpha", float) = 1
		_NoiseMap ("NoiseMap", 2D) = "white" {}
		_NoiseCoefficient ("NoiseCoefficient", Float) = 0
	}
	SubShader
	{
		Tags{ "Queue" = "Transparent" }

		Pass
		{
			stencil
			{
				Ref [_StencilValue]
				Comp Greater
				Pass replace
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
			sampler2D _NoiseMap;
			float4 _NoiseMap_ST;

			float _NoiseCoefficient;
			float _BlendAlpha;
			
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
				if (col.a <= 0.5f)
				{
					discard;
				}

				fixed4 Noise = tex2D(_NoiseMap, i.uv);

				clip(Noise.r - _NoiseCoefficient);


				col.a = _BlendAlpha;

				return col;
			}
			ENDCG
		}
	}
}
