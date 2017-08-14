Shader "Unlit/ShowUV"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "black" {}
		_Sel ("uv channel selector", Range(0,3)) = 0
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100 ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma geometry geom
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float2 uv2 : TEXCOORD1;
				float2 uv3 : TEXCOORD2;
				float2 uv4 : TEXCOORD3;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float3 bary : TEXCOORD1;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _Sel;
			
			v2f vert (appdata v)
			{
				float2 uv = v.uv;
				uv = lerp(uv,v.uv2,saturate(_Sel));
				uv = lerp(uv,v.uv3,saturate(_Sel-1.0));
				uv = lerp(uv,v.uv4,saturate(_Sel-2.0));
				uv.y = 1.0-uv.y;

				v2f o = (v2f)0;
				o.vertex = float4(uv*2.0-1.0,0.0,1.0);
				o.uv = v.uv;
				return o;
			}

			[maxvertexcount(3)]
			void geom(triangle v2f v[3], inout TriangleStream<v2f> triStream) {
				for(int i=0; i<3; i++){
					v2f o = v[i];
					o.bary = half3(i == 0,i == 1,i == 2);
					triStream.Append(o);
				}
				triStream.RestartStrip();
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				half3 d = fwidth(i.bary);
				half3 a3 = smoothstep(half3(0,0,0), d*1.0, i.bary);
				half w = 1.0 - min(min(a3.x,a3.y),a3.z);

				fixed4 col = tex2D(_MainTex, i.uv);
				return lerp(col,1.0-col, w);
			}
			ENDCG
		}
	}
}
