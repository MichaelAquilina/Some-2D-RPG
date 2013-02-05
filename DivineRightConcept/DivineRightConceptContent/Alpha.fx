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

// here we do the real work. 
float4 PixelShaderFunction(float2 inCoord: TEXCOORD0) : COLOR
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
		// The xbox can only run pixel shader 2.0 and for this 
		// purpose that is plenty for us too.
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}