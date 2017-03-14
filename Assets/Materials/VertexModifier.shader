Shader "Custom/VertexModifier" {
	Properties{
		_MainTex("Texture", 2D) = "white" {}
		_Amount("Height Adjustment", Range(0,5)) = 1.0
		_HeightMap("Height Map",2D) = "black" {}
		_RelativeScale("Relative Scale",Float) = 0
	}
		SubShader{
		Tags{ "RenderType" = "Opaque" }
		CGPROGRAM
#pragma surface surf Lambert vertex:vert
		struct Input {
		float2 uv_MainTex;
	};

	// Access the shaderlab properties
	float _Amount;
	sampler2D _MainTex;

	sampler2D _HeightMap;
	float _RelativeScale;

	// Vertex modifier function
	void vert(inout appdata_full v) {
		// Do whatever you want with the "vertex" property of v here

		//v.vertex.y += _Amount;
		float4 color = tex2Dlod(_HeightMap, v.texcoord);
		v.vertex.y += _RelativeScale*(-10000 + ((color.x * 255 * 256 * 256 + color.y * 255 * 256 + color.z * 255) * 0.1));
	}

	// Surface shader function
	void surf(Input IN, inout SurfaceOutput o) {
		o.Albedo = tex2D(_MainTex, IN.uv_MainTex).rgb;
	}
	ENDCG
	}
}