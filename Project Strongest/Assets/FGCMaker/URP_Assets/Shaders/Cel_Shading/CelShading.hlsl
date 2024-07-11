void CelShading_float(in float3 Normal, in float Smoothness, in float3 ClipSpacePos, in float3 WorldPos, in float4 tinting, in float rampOffset, out float3 rampOutput, out float3 direction)
{
#ifdef SHADERGRAPH_PREVIEW
	rampOutput = float3(0.5f,0.5f,0.5f);
	direction = float3(0.5f,0.5f,0.0f);
#else
	#if SHADOWS_SCREEN
	half4 shadowCoord = ComputeScreenPos(ClipSpacePos);
	#else
	half shadowCoord = TransformWorldToShadowCoord(WorldPos);
	#endif

	#if _MAIN_LIGHT_SHADOWS_CASCADE || _MAIN_LIGHT_SHADOWS
	Light light = GetMainLight(shadowCoord);
	#else
	Light light = GetMainLight();
	#endif

	half d = dot(Normal, light.direction) * 0.5f + 0.5f;

	half celShadingRamp = smoothstep(rampOffset,rampOffset + Smoothness,d);

	celShadingRamp += light.shadowAttenuation;

	rampOutput = light.color * (celShadingRamp + tinting);

	direction = light.direction;
#endif
}