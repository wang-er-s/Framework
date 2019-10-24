Shader "Unlit/Marquee"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Range("Range",Range(0.1,1))=0.3

		_Speed("Speed",Range(0,5))=1

		_Color1("Color1",Color)=(1,0,0,1)
		_Color2("Color2",Color)=(0,0,1,1)
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
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
			float _Range;

			fixed4 _Color1;
			fixed4 _Color2;

			float _Speed;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);
				
				if (col.g >= 0.2)
				{
					discard;
				}
				
				float minValue=_Time.y*_Speed;
				float maxValue=minValue+_Range;

				minValue%=1;
				maxValue%=1;

				if(minValue<maxValue)
				{
					if(col.r>minValue&&col.r<maxValue)
					{
						return _Color1;
					}
					else
					{
						return _Color2;
					}
				}
				else
				{
					if(col.r<maxValue||col.r>minValue)
					{
						return _Color1;
					}
					else
					{
						return _Color2;
					}
				}

			}
			ENDCG
		}
	}
}
