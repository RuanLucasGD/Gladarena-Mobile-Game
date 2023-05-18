#ifndef DUSTYROOM_STYLIZED_LIGHTING_INCLUDED
#define DUSTYROOM_STYLIZED_LIGHTING_INCLUDED

#include "UnityPBSLighting.cginc"

UNITY_INSTANCING_BUFFER_START(Props)

UNITY_DEFINE_INSTANCED_PROP(fixed4, _Color)

#ifdef _CELPRIMARYMODE_SINGLE
UNITY_DEFINE_INSTANCED_PROP(fixed4, _ColorDim)
#endif  // _CELPRIMARYMODE_SINGLE

#ifdef DR_SPECULAR_ON
UNITY_DEFINE_INSTANCED_PROP(half4, _FlatSpecularColor)
UNITY_DEFINE_INSTANCED_PROP(float, _FlatSpecularSize)
UNITY_DEFINE_INSTANCED_PROP(float, _FlatSpecularEdgeSmoothness)
#endif  // DR_SPECULAR_ON

#ifdef DR_RIM_ON
UNITY_DEFINE_INSTANCED_PROP(half4, _FlatRimColor)
UNITY_DEFINE_INSTANCED_PROP(float, _FlatRimSize)
UNITY_DEFINE_INSTANCED_PROP(float, _FlatRimEdgeSmoothness)
UNITY_DEFINE_INSTANCED_PROP(float, _FlatRimLightAlign)
#endif  // DR_RIM_ON

UNITY_INSTANCING_BUFFER_END(Props)

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

half _SelfShadingSize;
half _ShadowEdgeSize;
half _LightContribution;
half _Flatness;

half _SelfShadingSizeExtra;
half _ShadowEdgeSizeExtra;
half _FlatnessExtra;

half _UnityShadowPower;
half _UnityShadowSharpness;
half3 _UnityShadowColor;

#ifdef _CELPRIMARYMODE_STEPS
fixed4 _ColorDimSteps;
sampler2D _CelStepTexture;
#endif  // _CELPRIMARYMODE_STEPS

#ifdef _CELPRIMARYMODE_CURVE
fixed4 _ColorDimCurve;
sampler2D _CelCurveTexture;
#endif  // _CELPRIMARYMODE_CURVE

#ifdef DR_CEL_EXTRA_ON
fixed4 _ColorDimExtra;
#endif  // DR_CEL_EXTRA_ON

#ifdef DR_GRADIENT_ON
half4 _ColorGradient;
half _GradientCenterX;
half _GradientCenterY;
half _GradientSize;
half _GradientAngle;
#endif  // DR_GRADIENT_ON

half _TextureImpact;

sampler2D _MainTex;

struct InputObject
{
    float2 uv_MainTex;
    float3 viewDir;
    float3 lightDir;
    float3 worldPos;
    float3 worldNormal;
    
    #ifdef DR_VERTEX_COLORS_ON
    float4 color: Color;  // Vertex color
    #endif  // DR_VERTEX_COLORS_ON

    INTERNAL_DATA
};

struct SurfaceOutputDustyroom
{
    fixed3 Albedo;
    fixed3 Normal;
    half3 Emission;
    float Smoothness;
    float Metallic;
    half Occlusion;
    fixed Alpha;
    float Attenuation;
    float SpecularLightmapOcclusion;
};

inline half NdotLTransition(half3 normal, half3 lightDir, half selfShadingSize, half shadowEdgeSize, half flatness) {
    half NdotL = dot(normal, lightDir);
    half angleDiff = saturate((NdotL * 0.5 + 0.5) - selfShadingSize);
    half angleDiffTransition = smoothstep(0, shadowEdgeSize, angleDiff); 
    return lerp(angleDiff, angleDiffTransition, flatness);
}

inline half NdotLTransitionPrimary(half3 normal, half3 lightDir) { 
    return NdotLTransition(normal, lightDir, _SelfShadingSize, _ShadowEdgeSize, _Flatness);
}

inline half NdotLTransitionExtra(half3 normal, half3 lightDir) { 
    return NdotLTransition(normal, lightDir, _SelfShadingSizeExtra, _ShadowEdgeSizeExtra, _FlatnessExtra);
}

inline half NdotLTransitionTexture(half3 normal, half3 lightDir, sampler2D stepTex) {
    half NdotL = dot(normal, lightDir);
    half angleDiff = saturate((NdotL * 0.5 + 0.5) - _SelfShadingSize * 0.0);
    half angleDiffTransition = tex2D(stepTex, half2(angleDiff, 0.5)).r; 
    return angleDiffTransition;//lerp(angleDiff, angleDiffTransition, _Flatness);
}

inline half4 LightingDustyroomStylized(inout SurfaceOutputDustyroom s, half3 lightDir, UnityGI gi) {
    half attenSharpened = saturate(s.Attenuation * _UnityShadowSharpness);

#if defined(_UNITYSHADOWMODE_COLOR) && defined(USING_DIRECTIONAL_LIGHT)
    s.Albedo = lerp(_UnityShadowColor, s.Albedo, attenSharpened);
#endif

#ifdef USING_DIRECTIONAL_LIGHT
    half3 light = lerp(half3(1, 1, 1), _LightColor0.rgb, _LightContribution);
#else
    half3 light = gi.light.color.rgb;
#endif

    half unityShadowPower = 1;  // Corresponds to no shadow mode keyword.
#if defined(_UNITYSHADOWMODE_MULTIPLY)
    unityShadowPower = lerp(1, attenSharpened, _UnityShadowPower);
#endif

    half4 c;
    c.rgb = s.Albedo * light * unityShadowPower;

#if defined(UNITY_LIGHT_FUNCTION_APPLY_INDIRECT)
    c.rgb += s.Albedo * gi.indirect.diffuse;
#endif

    c.a = s.Alpha;
    return c;
}

inline void LightingDustyroomStylized_GI(inout SurfaceOutputDustyroom s, UnityGIInput data, inout UnityGI gi) {
    gi = UnityGlobalIllumination(data, s.Occlusion, s.Normal);
    s.Attenuation = data.atten;
}

void vertObject(inout appdata_full v, out InputObject o) {
    UNITY_INITIALIZE_OUTPUT(InputObject, o);
    o.lightDir = WorldSpaceLightDir(v.vertex);
}

inline half4 SurfaceCore(half3 worldNormal, half3 worldPos, half3 lightDir, half3 viewDir) {
    fixed4 c = UNITY_ACCESS_INSTANCED_PROP(Props, _Color);
    
    #if defined(_CELPRIMARYMODE_SINGLE)
        half NdotLTPrimary = NdotLTransitionPrimary(worldNormal, lightDir);
        c = lerp(UNITY_ACCESS_INSTANCED_PROP(Props, _ColorDim), c, NdotLTPrimary);
    #endif  // _CELPRIMARYMODE_SINGLE
    
    #if defined(_CELPRIMARYMODE_STEPS)
        half NdotLTSteps = NdotLTransitionTexture(worldNormal, lightDir, _CelStepTexture);
        c = lerp(_ColorDimSteps, c, NdotLTSteps);
    #endif  // _CELPRIMARYMODE_STEPS
    
    #if defined(_CELPRIMARYMODE_CURVE)
        half NdotLTCurve = NdotLTransitionTexture(worldNormal, lightDir, _CelCurveTexture);
        c = lerp(_ColorDimCurve, c, NdotLTCurve);
    #endif  // _CELPRIMARYMODE_CURVE
    
    #if defined(DR_CEL_EXTRA_ON)
        half NdotLTExtra = NdotLTransitionExtra(worldNormal, lightDir);
        c = lerp(_ColorDimExtra, c, NdotLTExtra);
    #endif  // DR_CEL_EXTRA_ON
    
    #ifdef DR_GRADIENT_ON
        float angleRadians = _GradientAngle / 180.0 * 3.14159265359;
        float posGradRotated = (worldPos.x - _GradientCenterX) * sin(angleRadians) + 
                               (worldPos.y - _GradientCenterY) * cos(angleRadians);
        float gradientTop = _GradientCenterY + _GradientSize * 0.5;
        half gradientFactor = saturate((gradientTop - posGradRotated) / _GradientSize);
        c = lerp(c, _ColorGradient, gradientFactor);
    #endif  // DR_GRADIENT_ON

    #ifdef DR_RIM_ON
        float4 rim = 1.0 - dot(viewDir, worldNormal);
        half NdotL = dot(worldNormal, lightDir);
        float rimLightAlign = UNITY_ACCESS_INSTANCED_PROP(Props, _FlatRimLightAlign);
        float rimSize = UNITY_ACCESS_INSTANCED_PROP(Props, _FlatRimSize);
        float rimSpread = 1.0 - rimSize - NdotL * rimLightAlign;
        float rimEdgeSmooth = UNITY_ACCESS_INSTANCED_PROP(Props, _FlatRimEdgeSmoothness);
        float rimTransition = smoothstep(rimSpread - rimEdgeSmooth * 0.5, rimSpread + rimEdgeSmooth * 0.5, rim);
        c = lerp(c, UNITY_ACCESS_INSTANCED_PROP(Props, _FlatRimColor), rimTransition);
    #endif  // DR_RIM_ON

    #ifdef DR_SPECULAR_ON
        float3 halfVector = normalize(_WorldSpaceLightPos0 + viewDir);
        float NdotH = dot(worldNormal, halfVector) * 0.5 + 0.5;
        float specularSize = UNITY_ACCESS_INSTANCED_PROP(Props, _FlatSpecularSize);
        float specEdgeSmooth = UNITY_ACCESS_INSTANCED_PROP(Props, _FlatSpecularEdgeSmoothness);
        float specular = saturate(pow(NdotH, 100.0 * (1.0 - specularSize) * (1.0 - specularSize)));
        float specularTransition = smoothstep(0.5 - specEdgeSmooth * 0.1, 0.5 + specEdgeSmooth * 0.1, specular);
        c = lerp(c, UNITY_ACCESS_INSTANCED_PROP(Props, _FlatSpecularColor), specularTransition);
    #endif  // DR_SPECULAR_ON
    
    return c;
}
    
void surfObject(InputObject IN, inout SurfaceOutputDustyroom o) {
    half4 c = SurfaceCore(IN.worldNormal, IN.worldPos, IN.lightDir, IN.viewDir);
    float2 uv_PolyArtAlbedo = IN.uv_MainTex * _PolyArtAlbedo_ST.xy + _PolyArtAlbedo_ST.zw;
    float4 tex2DNode16 = tex2D(_PolyArtAlbedo, uv_PolyArtAlbedo);
    float2 uv_PolyArtMask01 = IN.uv_MainTex * _PolyArtMask01_ST.xy + _PolyArtMask01_ST.zw;
    float4 tex2DNode13 = tex2D(_PolyArtMask01, uv_PolyArtMask01);
    float4 temp_cast_0 = (tex2DNode13.r).xxxx;
    float4 temp_cast_1 = (tex2DNode13.g).xxxx;
    float4 temp_cast_2 = (tex2DNode13.b).xxxx;
    float2 uv_PolyartMask02 = IN.uv_MainTex * _PolyartMask02_ST.xy + _PolyartMask02_ST.zw;
    float4 tex2DNode41 = tex2D(_PolyartMask02, uv_PolyartMask02);
    float4 temp_cast_3 = (tex2DNode41.r).xxxx;
    float4 blendOpSrc22 = tex2DNode16;
    float4 blendOpDest22 = ((min(temp_cast_0, _Color01) * _Color01Power) + (min(temp_cast_1, _Color02) * _Color2Power) + (min(temp_cast_2, _Color03) * _Color03Power) + (min(temp_cast_3, _Color04) * _Color04Power));
    float4 lerpResult4 = lerp(tex2DNode16, ((saturate((blendOpSrc22 * blendOpDest22))) * _OverallBrightness), (tex2DNode13.r + tex2DNode13.g + tex2DNode13.b + tex2DNode41.r));
    

    {
        #if defined(_TEXTUREBLENDINGMODE_ADD)
            c += lerp(half4(0.0, 0.0, 0.0, 0.0), half4(lerpResult4), _TextureImpact);
        #else  // _TEXTUREBLENDINGMODE_MULTIPLY
            // This is the default blending mode for compatibility with the v.1 of the asset.
            c *= lerp(half4(1.0, 1.0, 1.0, 1.0), half4(lerpResult4), _TextureImpact);
        #endif
    }

    {
        #ifdef DR_VERTEX_COLORS_ON
            c *= IN.color;
        #endif  // DR_VERTEX_COLORS_ON
    }

    o.Albedo = c.rgb;
    o.Alpha = c.a;
}

#endif // DUSTYROOM_STYLIZED_LIGHTING_INCLUDED
