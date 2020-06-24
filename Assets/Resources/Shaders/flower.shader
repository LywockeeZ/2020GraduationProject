
Shader "scenes/flower"

{

	Properties

	{

		_Color ("Color",color) = (1,1,1,1)

		_MainTex ("Texture", 2D) = "white" {}

		_windSize ("windSize",vector) = (1,1,1)

		_windSpeed("windSpeed",vector) = (1,1,1)

		_windRandom("windRandom",vector) = (1,1,1)

	}

	SubShader

	{

		Tags { "RenderType"="Opaque" }

		LOD 100

		Cull Off

		Pass

		{

			CGPROGRAM

			#pragma vertex vert

			#pragma fragment frag

			#pragma multi_compile_fog

			#include "UnityCG.cginc"

 

			struct appdata

			{

				float4 vertex : POSITION;

				float2 uv : TEXCOORD0;

				float4 color : COLOR;

			};

 

			struct v2f

			{

				float2 uv : TEXCOORD0;

				UNITY_FOG_COORDS(1)

				float4 vertex : SV_POSITION;

			};

			sampler2D _MainTex;

			fixed4 _MainTex_ST;

			fixed4 _Pos;

			fixed3 _windSpeed;

			fixed3 _windSize;

			fixed3 _windRandom;

			fixed4 _Color;

			v2f vert (appdata v)

			{

				v2f o;

				

				float4 vert = v.vertex;

				float dis = distance (v.vertex,_Pos);

				vert.xyz += (sin (dis*40*_windRandom.xyz +_Time.y*_windSpeed.xyz)) * v.color.a*0.1*_windSize.xyz;

				o.vertex = UnityObjectToClipPos(vert);

				o.uv = TRANSFORM_TEX(v.uv, _MainTex);

				UNITY_TRANSFER_FOG(o,o.vertex);

				return o;

			}

			fixed4 frag (v2f i) : SV_Target

			{

				fixed4 col = tex2D(_MainTex, i.uv) * _Color;

				clip (col.a -0.5);

				UNITY_APPLY_FOG(i.fogCoord, col);

				return col;

			}

			ENDCG

		}

	}

}
