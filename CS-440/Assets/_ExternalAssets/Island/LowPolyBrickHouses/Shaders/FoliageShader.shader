// Upgrade NOTE: upgraded instancing buffer 'Props' to new syntax.

Shader "Custom/FoliageShader"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo", 2D) = "white" {}
		_BumpMap("Normal Map", 2D) = "bump" {}
		_Cutoff("Alpha Cutoff", Range(0.0, 1.0)) = 0.5
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
		_Intensity("Intensity", Range(0, 1.0)) = 1.0
		_WindSpeed("WindSpeed", Range(0, 10.0)) = 1.0
		_Randomness("Randomness", Range(0, 5.0)) = 1.0
	}
	SubShader
	{
		Tags{ "Queue" = "AlphaTest" "RenderType" = "TransparentCutout" "IgnoreProjector" = "True" }
		LOD 200
		Blend One Zero
		ZWrite On

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows vertex:vert alphatest:_Cutoff addshadow

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		struct Input
		{
			float2 uv_MainTex;
		};

		sampler2D _MainTex;
		sampler2D _BumpMap;

		fixed4 _Color;
		half _Glossiness;
		half _Metallic;

		float _Intensity;
		float _WindSpeed;
		float _Randomness;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
		// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		float random(float3 p)
		{
			return frac(43758.5453 * sin(dot(p, float3(12.9898, 78.233, 45.5432)) % 3.14159));
		}

		void vert(inout appdata_full v)
		{
			float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
			float3 offset = _Intensity * (sin(worldPos.xyz + _Time.y * _WindSpeed) + _Randomness * random(worldPos));
			v.vertex.xyz += offset;
		}

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_MainTex));

			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Transparent/Cutout/Diffuse"
	//FallBack "Diffuse"
}
