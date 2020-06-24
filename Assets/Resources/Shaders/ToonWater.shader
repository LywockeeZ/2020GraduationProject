Shader "Custom/ToonWater"
{
	Properties
	{
		_Color("Color", Color) = (1, 1, 1, 1)
		_DepthColor("DepthColor", color) = (1,1,1,1)
    	        _DepthBlend("Depth",float) = 1
		_FoamColor("FoamColor",color) = (1,1,1,1)
		_FoamSpeed("FoamSpeed",float) = 1
		_FoamEdgeDepth("Foam Edge Depth", float) = 1.0
		_WaveSpeed("Wave Speed", float) = 1.0
		_WaveHeight("Wave Height", float) = 0.2
		_NoiseTex("Noise Texture", 2D) = "white" {}
		_MainTex("Main Texture", 2D) = "white" {}
	}

	SubShader
	{
        Tags
		{ 
			"Queue" = "Transparent"
		}

		Pass
		{	
			//在_Color，_DepthColor中设置Alpha值从而使水面与水底混合
      		        Blend SrcColor OneMinusSrcAlpha

			CGPROGRAM
      		        #include "UnityCG.cginc"

			#pragma vertex vert
			#pragma fragment frag
			
			// Properties
			float  _FoamEdgeDepth;
			float  _WaveSpeed;
			float  _WaveHeight;
			float _DepthBlend;
			float _FoamSpeed;
			float4 _Color;
			float4 _MainTex_ST;
			float4 _DepthColor;
			float4 _FoamColor;
			sampler2D _CameraDepthTexture;
			sampler2D _NoiseTex;
			sampler2D _MainTex;

			struct a2v
			{
				float4 vertex : POSITION;
				float4 texCoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				float2 texCoord : TEXCOORD0;
				float4 screenPos : TEXCOORD1;
			};

			v2f vert(a2v i)
			{
				v2f o;

				o.pos = UnityObjectToClipPos(i.vertex);

				//波浪顶点动画
				//为什么用tex2Dlod()，见http://www.ufgame.com/9620.html
				float noise = tex2Dlod(_NoiseTex, float4(i.texCoord.xy, 0, 0));
				o.pos.y += sin(_Time*_WaveSpeed*noise+(o.pos.x*o.pos.z))*_WaveHeight;

				// 计算屏幕空间深度
				o.screenPos = ComputeScreenPos(o.pos);
				//水面泡沫位移
				_MainTex_ST.w +=_Time*_FoamSpeed;
 
				o.texCoord = TRANSFORM_TEX(i.texCoord,_MainTex);

				return o;
			}

			float4 frag(v2f i) : COLOR
			{
				//采样并线性化深度值
				float4 depthSample = SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, i.screenPos);
				float depth = LinearEyeDepth(depthSample).r;

				//通过深度值对比创建边缘泡沫因子,其中i.screenPos.w存放的视空间下的线性深度值
				float foamLine = 1 - saturate(_FoamEdgeDepth * (depth - i.screenPos.w));
				//通过深度值对比创建水体深度颜色渐变因子
				float waterDepth = saturate(_DepthBlend * (depth - i.screenPos.w));
        		        //混合最终颜色
				float4 mainColor = tex2D(_MainTex,i.texCoord);
			  	float4 color = lerp(_Color,_DepthColor,waterDepth) + (foamLine * _FoamColor) + (mainColor *_FoamColor);
        		        return color;
			}
			ENDCG
		}
	}
}