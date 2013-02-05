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

uniform extern float LightValue;

// here we do the real work. 
float4 PixelShaderFunction(float2 inCoord: TEXCOORD0) : COLOR
{
	// we retrieve the color in the original texture at 
	// the current coordinate remember that this function 
	// is run on every pixel in our texture.
    float4 c = tex2D(screen, inCoord);

	c.r = clamp( c.r * tex2D(lightMap, inCoord).r, 0, 1);
	c.g = clamp( c.g * tex2D(lightMap, inCoord).g, 0, 1);
	c.b = clamp( c.b * tex2D(lightMap, inCoord).b, 0, 1);

	// return the new color of the pixel.
    return c;
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