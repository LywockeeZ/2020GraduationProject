Shader "MyUnlit/ScrollWater"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color("Color Tint",color)=(1,1,1,1)
        //控制水流波动的幅度，也就是三角函数中的振幅（值域范围）
        _Magnitude("Distortion Magnitude",float)=0.3
        //控制周期的长度，值越大，周期越短，频率越高
        _InvWaveLength("Distortion Inserve Wave Length",float)=1
        //流动速度，用于纹理变换
        _Speed("Speed",float)=0.1
    }
    SubShader
    {
        //顶点动画需要禁用合P处理
        Tags {"Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="true" "DisableBatching"="True"}

        Pass
        {
            //透明度混合：关闭深度写入+设置混合状态+禁用剔除（双面渲染）
            Tags{"lightmode"="forwardbase"}
            ZWrite off
            Blend SrcAlpha OneMinusSrcAlpha
            Cull off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog

            #include "UnityCG.cginc"
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            float _Magnitude;
            float _InvWaveLength;
            float _Speed;

            v2f vert (appdata v)
            {
                v2f o;
                float4 offset;
                //这里的方向可以自己选择，这里选择偏移x方向，其他方向的偏移保持不变
                offset.yzw = float3(0, 0, 0);
                //利用正弦函数模拟河流整体的形状，最后乘以振幅
                offset.x = sin((v.vertex.x + v.vertex.y + v.vertex.z)*_InvWaveLength)*_Magnitude;
                o.vertex = UnityObjectToClipPos(v.vertex+offset);
                //对uv进行某一方向的滚动以模拟水流，这里选择v向
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.uv += float2(0.0, _Time.y*_Speed);

                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                col.rgb *= _Color.rgb;
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
    FallBack "Transparent/VertexLit"
}