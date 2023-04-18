// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "PolyArtMaskTint"
{
	Properties
	{
		_Smoothness("Smoothness", Range( 0 , 1)) = 0
		_Metallic("Metallic", Range( 0 , 1)) = 0
		_Color01("Color01", Color) = (0.7205882,0.08477508,0.08477508,0)
		_Color02("Color02", Color) = (0.02649222,0.3602941,0.09785674,0)
		_Color03("Color03", Color) = (0.07628676,0.2567445,0.6102941,0)
		_Color04("Color04", Color) = (0.6207737,0.1119702,0.8014706,0)
		_PolyArtAlbedo("PolyArtAlbedo", 2D) = "white" {}
		_PolyArtMask01("PolyArtMask01", 2D) = "white" {}
		_PolyartMask02("PolyartMask02", 2D) = "white" {}
		_OverallBrightness("OverallBrightness", Range( 0 , 4)) = 1
		_Color01Power("Color01Power", Range( 0 , 4)) = 1
		_Color2Power("Color2Power", Range( 0 , 4)) = 1
		_Color03Power("Color03Power", Range( 0 , 4)) = 1
		_Color04Power("Color04Power", Range( 0 , 4)) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _PolyArtAlbedo;
		uniform float4 _PolyArtAlbedo_ST;
		uniform sampler2D _PolyArtMask01;
		uniform float4 _PolyArtMask01_ST;
		uniform float4 _Color01;
		uniform float _Color01Power;
		uniform float4 _Color02;
		uniform float _Color2Power;
		uniform float4 _Color03;
		uniform float _Color03Power;
		uniform sampler2D _PolyartMask02;
		uniform float4 _PolyartMask02_ST;
		uniform float4 _Color04;
		uniform float _Color04Power;
		uniform float _OverallBrightness;
		uniform float _Metallic;
		uniform float _Smoothness;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_PolyArtAlbedo = i.uv_texcoord * _PolyArtAlbedo_ST.xy + _PolyArtAlbedo_ST.zw;
			float4 tex2DNode16 = tex2D( _PolyArtAlbedo, uv_PolyArtAlbedo );
			float2 uv_PolyArtMask01 = i.uv_texcoord * _PolyArtMask01_ST.xy + _PolyArtMask01_ST.zw;
			float4 tex2DNode13 = tex2D( _PolyArtMask01, uv_PolyArtMask01 );
			float4 temp_cast_0 = (tex2DNode13.r).xxxx;
			float4 temp_cast_1 = (tex2DNode13.g).xxxx;
			float4 temp_cast_2 = (tex2DNode13.b).xxxx;
			float2 uv_PolyartMask02 = i.uv_texcoord * _PolyartMask02_ST.xy + _PolyartMask02_ST.zw;
			float4 tex2DNode41 = tex2D( _PolyartMask02, uv_PolyartMask02 );
			float4 temp_cast_3 = (tex2DNode41.r).xxxx;
			float4 blendOpSrc22 = tex2DNode16;
			float4 blendOpDest22 = ( ( min( temp_cast_0 , _Color01 ) * _Color01Power ) + ( min( temp_cast_1 , _Color02 ) * _Color2Power ) + ( min( temp_cast_2 , _Color03 ) * _Color03Power ) + ( min( temp_cast_3 , _Color04 ) * _Color04Power ) );
			float4 lerpResult4 = lerp( tex2DNode16 , ( ( saturate( ( blendOpSrc22 * blendOpDest22 ) )) * _OverallBrightness ) , ( tex2DNode13.r + tex2DNode13.g + tex2DNode13.b + tex2DNode41.r ));
			o.Albedo = lerpResult4.rgb;
			o.Metallic = _Metallic;
			o.Smoothness = _Smoothness;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	//CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=16100
7;29;1906;1004;2632.222;432.5741;1.375956;True;True
Node;AmplifyShaderEditor.ColorNode;10;-1908.554,183.2922;Float;False;Property;_Color02;Color02;3;0;Create;True;0;0;False;0;0.02649222,0.3602941,0.09785674,0;0.2509804,0,0.7803922,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;41;-2227.691,600.7684;Float;True;Property;_PolyartMask02;PolyartMask02;8;0;Create;True;0;0;False;0;dea1ff9608588c14ab9d2dd4887ac6db;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;11;-1962.772,431.8295;Float;False;Property;_Color03;Color03;4;0;Create;True;0;0;False;0;0.07628676,0.2567445,0.6102941,0;1,0.6431373,0,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;33;-1929.07,732.8728;Float;False;Property;_Color04;Color04;5;0;Create;True;0;0;False;0;0.6207737,0.1119702,0.8014706,0;1,0.6431373,0,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;9;-1941.449,-45.75774;Float;False;Property;_Color01;Color01;2;0;Create;True;0;0;False;0;0.7205882,0.08477508,0.08477508,0;0.07552986,0.4010895,0.9338235,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;13;-2073.799,-316.9005;Float;True;Property;_PolyArtMask01;PolyArtMask01;7;0;Create;True;0;0;False;0;dea1ff9608588c14ab9d2dd4887ac6db;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;39;-1389.024,774.8583;Float;False;Property;_Color04Power;Color04Power;13;0;Create;True;0;0;False;0;1;2;0;4;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMinOpNode;17;-1577.399,193.755;Float;True;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;31;-1373.947,342.9876;Float;False;Property;_Color2Power;Color2Power;11;0;Create;True;0;0;False;0;1;2;0;4;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMinOpNode;15;-1607.751,-89.10741;Float;True;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMinOpNode;34;-1626.32,655.0611;Float;True;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;38;-1413.948,516.7104;Float;False;Property;_Color03Power;Color03Power;12;0;Create;True;0;0;False;0;1;2;0;4;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;35;-1381.273,142.3805;Float;False;Property;_Color01Power;Color01Power;10;0;Create;True;0;0;False;0;1;2;0;4;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMinOpNode;18;-1598.141,416.7289;Float;True;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;40;-1099.408,728.6387;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;36;-1098.774,23.16727;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;32;-1105.004,261.0664;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;37;-1118.99,434.884;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;19;-865.3752,93.06007;Float;True;4;4;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;16;-1042.803,-639.5633;Float;True;Property;_PolyArtAlbedo;PolyArtAlbedo;6;0;Create;True;0;0;False;0;2f06be5cc89f84f48bfe39a1f88242c4;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;24;-636.9345,166.0474;Float;False;Property;_OverallBrightness;OverallBrightness;9;0;Create;True;0;0;False;0;1;2;0;4;0;1;FLOAT;0
Node;AmplifyShaderEditor.BlendOpsNode;22;-622.9166,-39.07136;Float;False;Multiply;True;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;20;-1187.253,-356.6879;Float;True;4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;23;-341.7787,-32.76241;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;4;-204.1115,-328.2731;Float;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;2;-325,351;Float;False;Property;_Smoothness;Smoothness;0;0;Create;True;0;0;False;0;0;0.2;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1;-332.5154,220.1814;Float;False;Property;_Metallic;Metallic;1;0;Create;True;0;0;False;0;0;0.2;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;92.56065,-105;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;PolyArtMaskTint;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;17;0;13;2
WireConnection;17;1;10;0
WireConnection;15;0;13;1
WireConnection;15;1;9;0
WireConnection;34;0;41;1
WireConnection;34;1;33;0
WireConnection;18;0;13;3
WireConnection;18;1;11;0
WireConnection;40;0;34;0
WireConnection;40;1;39;0
WireConnection;36;0;15;0
WireConnection;36;1;35;0
WireConnection;32;0;17;0
WireConnection;32;1;31;0
WireConnection;37;0;18;0
WireConnection;37;1;38;0
WireConnection;19;0;36;0
WireConnection;19;1;32;0
WireConnection;19;2;37;0
WireConnection;19;3;40;0
WireConnection;22;0;16;0
WireConnection;22;1;19;0
WireConnection;20;0;13;1
WireConnection;20;1;13;2
WireConnection;20;2;13;3
WireConnection;20;3;41;1
WireConnection;23;0;22;0
WireConnection;23;1;24;0
WireConnection;4;0;16;0
WireConnection;4;1;23;0
WireConnection;4;2;20;0
WireConnection;0;0;4;0
WireConnection;0;3;1;0
WireConnection;0;4;2;0
ASEEND*/
//CHKSM=4A47280B3B2066E058C8F3E2D96FBDC49A306237