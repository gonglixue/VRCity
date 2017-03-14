Shader "Custom/VertFrag_Diffuse" {
	Properties {
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
	}
		SubShader{
			Tags { "RenderType" = "Opaque" }
			LOD 300

			Pass {
				Tags { "LightMode" = "Vertex" }
				Cull Back
				Lighting On
				CGPROGRAM

				#pragma vertex vert
				#pragma fragment frag
				#pragma multi_compile_fwdbase
				#include "UnityCG.cginc"
		// code
			sampler _MainTex;
			float4 _MainTex_ST;
			struct a2v {
				float4 vertex: POSITION;
				float3 normal: NORMAL;
				float4 texcoord: TEXCOORD0;
			};
			struct v2f {
				float4 pos: POSITION;  // 在这里语义就没有这么重要了。
				// 只有POSITION是必须的，这是把顶点坐标转换到clip space后的位置。
				// 我们输出的所有值，在fragment program之前被插值了。
				float2 uv: TEXCOORD0;
				float3 color:TEXCOORD1;
			};

			v2f vert(a2v v) {
				v2f o;
				float4 temp = (0, 10, 0, 0);
				v.vertex = v.vertex + temp;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);  // 把顶点位置从model space转换到clip space(projection space)。
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.color = ShadeVertexLights(v.vertex, v.normal);  // 将考虑4个距离最近的光源以及一个环境光
				return o;
			}

			float4 frag(v2f i) :COLOR{
				float4 c = tex2D(_MainTex, i.uv);
				c.rgb = c.rgb * i.color * 2;
				return c;
			}
			ENDCG
		}
	}
	FallBack "Diffuse"
}
