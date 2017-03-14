Shader "Custom/Height" {
	Properties
	{
		_MainTint("Global Color Tint", Color) = (1,1,1,1)
		_HeightMap("Height Map",2D) = "black" {}
		_Factor("Elevation Factor", Range(0,10)) = 0.5
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque" }
		LOD 200

		CGPROGRAM
		#pragma surface surf Lambert vertex:vert
		float4 _MainTint;
		sampler2D _HeightMap;
		float _Factor;
		struct Input {
			float2 uv_MainTex;
			float4 vertColor;
		};

		void vert(inout appdata_full v, out Input o)
		{
			o.vertColor = v.color;
			o.uv_MainTex = v.texcoord.xy;
			float3 heightColor = tex2Dlod(_HeightMap, v.texcoord).rgb;

			v.vertex.y *= _Factor * heightColor.x;
		}
		void surf(Input IN, inout SurfaceOutput o)
		{
			o.Albedo = IN.vertColor.rgb * _MainTint.rgb;
		}



		ENDCG
	}
	FallBack "Diffuse"
}
