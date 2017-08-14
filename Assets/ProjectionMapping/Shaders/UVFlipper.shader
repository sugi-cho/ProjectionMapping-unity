Shader "Hidden/UVFlipper"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		[Toggle] _FlipX("flip uv.x", Float) = 0
		[Toggle] _FlipY("flip uv.y", Float) = 0
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#pragma multi_compile __ _FLIPX_ON
			#pragma multi_compile __ _FLIPY_ON
			
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

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;

			fixed4 frag (v2f i) : SV_Target
			{
				#if _FLIPX_ON
				i.uv.x = 1.0-i.uv.x;
				#endif
				#if _FLIPY_ON
				i.uv.y = 1.0-i.uv.y;
				#endif

				fixed4 col = tex2D(_MainTex, i.uv);
				return col;
			}
			ENDCG
		}
	}
}
