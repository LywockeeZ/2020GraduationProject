Shader "Unity Shaders Book/Chapter 11/ImageSequenceAnimation"
{
	Properties
	{
		_Color("Color Tint",Color) = (1,1,1,1)
		//包含了所有关键帧的图像纹理
		_MainTex ("Image Sequence", 2D) = "white" {}
	    //水平方向和数值方向上包含的关键帧图像的个数
		_HorizontalAmount("Horizontral Amount",Float) = 4
		_VerticalAmount("Vertical Amount",Float) = 4
		//控制序列帧动画的播放速度
		_Speed("Speed",Range(1,100)) = 30
	}
		SubShader
		{
			//由于序列帧图像通常是透明背景，所以需要设置pass的相关状态，以渲染透明效果
			//半透明“标配”
			Tags { "Queue" = "Transparent" "IgnoreProject" = "True" "RenderType" = "Transparent" }

			Pass
			{
				Tags{"LightMode" = "ForwardBase"}
				//关闭深度写入
				ZWrite off
			//开启并设置混合模式
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			fixed4 _Color;
			sampler2D _MainTex;
		    float4 _MainTex_ST;
			float _HorizontalAmount;
			float _VerticalAmount;
			float _Speed;

			struct a2v
			{
				float4 vertex : POSITION;
				float4 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;				
			};

			v2f vert (a2v v)
			{
				v2f o;
				//基本的顶点变换
				o.pos = UnityObjectToClipPos(v.vertex);
				//存储顶点纹理坐标
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);				
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				//本质就是计算出每个时刻需要播放的关键帧在纹理中的位置
				//这个位置就是该关键帧所在行列数的索引数

				//_Time.y与_Speed相称得到模拟时间，取整得到当前时间
				float time = floor(_Time.y * _Speed);
			    //time除以_HorizontalAmount的结果的商为当前对应的行索引，余数为列索引
			    float row = floor(time / _HorizontalAmount);
			    float column = time - row * _HorizontalAmount;

				//使用行列索引值来构建真正的采样坐标
				//把原纹理坐标i.uv按行数和列数进行等分，得到每个子图象的纹理坐标
				//half2 uv = float2(i.uv.x / _HorizontalAmount, i.uv.y / _VerticalAmount);
				//使用当前的行列数对上面的结果进行偏移，得到当前子图像的纹理坐标
				//uv.x += column / _HorizontalAmount;
				//数值方向用减法，Unity纹理坐标的竖直方向的顺序与序列帧纹理中的顺序是相反的
				//uv.y -= row / _VerticalAmount;

				//把上述过程整合到一起
				half2 uv = i.uv + half2(column, -row);
				uv.x /= _HorizontalAmount;
				uv.y /= _VerticalAmount;

				fixed4 c = tex2D(_MainTex, uv);
				c.rgb *= _Color;

				return c;
			}
			ENDCG
		}
	}
			FallBack "Transparent/VertexLit"
}