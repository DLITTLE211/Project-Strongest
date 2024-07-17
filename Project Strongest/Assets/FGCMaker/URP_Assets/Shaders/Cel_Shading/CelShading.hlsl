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
void AddAdditionalLights_float(float Smoothness, float3 WorldPosition, float3 WorldNormal, 
	float3 WorldView, float MainDiffuse, float MainSpecular, float3 MainColor, 
	out float Diffuse, out float Specular, out float3 Color) 
{
	Diffuse = MainDiffuse;
	Specular = MainSpecular;
	Color = MainColor * (MainDiffuse + MainSpecular);
#ifndef SHADERGRAPH_PREVIEW
	int pixelLightCount = GetAdditionalLightsCount();
	for (int i = 0; i < pixelLightCount; i++) 
	{
		Light addLight = GetAdditionalLight(i, WorldPosition);
		half NdotL = saturate(dot(WorldNormal, addLight.direction));
		half atten = addLight.distanceAttenuation * addLight.shadowAttenuation;
		half thisDiffuse = atten * NdotL;
		half thisSpecular = LightingSpecular(thisDiffuse, addLight.direction, WorldNormal, WorldView, 1, Smoothness);
		Diffuse += thisDiffuse;
		Specular += thisSpecular;
		Color += addLight.color * (thisDiffuse + thisSpecular);
	}
#endif
	half total = Diffuse + Specular;
	Color = total <= 0 ? MainColor : Color / total;
}