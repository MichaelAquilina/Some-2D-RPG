// this is the texture we are trying to render
uniform extern texture ScreenTexture;  
sampler screen = sampler_state 
{
	// get the texture we are trying to render.
    Texture = <ScreenTexture>;
};

uniform extern texture LightMap;
sampler lightMap = sampler_state
{
	Texture = <LightMap>;
};

// colorize the luminosity of incoming pixels based on the light map passed to this shader
float4 PixelLightShading(float2 inCoord: TEXCOORD0) : COLOR
{
	float4 lightPixel = tex2D(lightMap, inCoord);
	float4 screenPixel = tex2D(screen, inCoord);

	float4 output = clamp(screenPixel * lightPixel, 0, 1);

	// return the new color of the pixel.
    return output;
}
 
technique
{
    pass P0
    {
        PixelShader = compile ps_2_0 PixelLightShading();
    }
}