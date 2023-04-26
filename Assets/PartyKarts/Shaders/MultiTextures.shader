Shader "Custom/MultiTextures" {
	Properties {
		_ControlTex ("ControlTexture (RGBA)", 2D) = "black" {}
		_Tex0("Texture0 (RGB)", 2D) = "white" {}
		_Tex1("Texture1 (RGB)", 2D) = "white" {}
		_Tex2("Texture2 (RGB)", 2D) = "white" {}
		_Tex3("Texture3 (RGB)", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 300

		CGPROGRAM

		#pragma surface surf Lambert noforwardadd

		sampler2D _ControlTex;
		sampler2D _Tex0;
		sampler2D _Tex1;
		sampler2D _Tex2;
		sampler2D _Tex3;

		struct Input {
			float2 uv_ControlTex;
			float2 uv_Tex0;
		};

		fixed4 _Color;

		void surf (Input IN, inout SurfaceOutput o) {
			// Albedo comes from a texture tinted by color
			fixed4 control = tex2D (_ControlTex, IN.uv_ControlTex);

			o.Albedo += control.a * tex2D(_Tex0, IN.uv_Tex0) +
						control.r * tex2D(_Tex1, IN.uv_Tex0) +
						control.g * tex2D(_Tex2, IN.uv_Tex0) +
						control.b * tex2D(_Tex3, IN.uv_Tex0)
			;
		}
		ENDCG
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM

		#pragma surface surf Lambert noforwardadd

		sampler2D _Tex0;

		struct Input {
			float2 uv_Tex0;
		};

		fixed4 _Color;

		void surf (Input IN, inout SurfaceOutput o) {
			half4 c = tex2D (_Tex0, IN.uv_Tex0);
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
