// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "MyShader/Blur"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {       

        Pass
        {
            CGPROGRAM
            #pragma vertex vert 
            #pragma fragment frag 
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;

            struct a2v{
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f{
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert(a2v v){
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;

                return o;
            }

            fixed4 frag(v2f i) : SV_Target{
                fixed3 sum = tex2D(_MainTex, i.uv).rgb;
                sum += tex2D(_MainTex, i.uv+float2(-_MainTex_TexelSize.x, _MainTex_TexelSize.y)).rgb;
                sum += tex2D(_MainTex, i.uv+float2(0, _MainTex_TexelSize.y)).rgb;
                sum += tex2D(_MainTex, i.uv+float2(_MainTex_TexelSize.x, _MainTex_TexelSize.y)).rgb;
                sum += tex2D(_MainTex, i.uv+float2(-_MainTex_TexelSize.x, 0)).rgb;
                sum += tex2D(_MainTex, i.uv+float2(_MainTex_TexelSize.x, 0)).rgb;
                sum += tex2D(_MainTex, i.uv+float2(-_MainTex_TexelSize.x,- _MainTex_TexelSize.y)).rgb;
                sum += tex2D(_MainTex, i.uv+float2(0,- _MainTex_TexelSize.y)).rgb;
                sum += tex2D(_MainTex, i.uv+float2(_MainTex_TexelSize.x,- _MainTex_TexelSize.y)).rgb;
                sum /= 9;

                return fixed4(sum, 1);
            }
            ENDCG
        }
    }
}
