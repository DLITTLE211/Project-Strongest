#ifndef CUSTOM_LIGHITNG_INCLUDED
#define CUSTOM_LIGHTING_INCLUDED

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
#endif