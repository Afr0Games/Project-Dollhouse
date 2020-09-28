

//------------------------------------------------------
//--                                                --
//--				www.riemers.net					--
//--				Basic shaders					--
//--			Use/modify as you like              --
//--                                                --
//------------------------------------------------------

struct VertexToPixel
{
	float4 Position   	: POSITION;
	float4 Color		: COLOR0;
	//float2 TextureCoords : TEXCOORD0;
	float2 TerrainTypeCoords : TEXCOORD0;
	float2 GrassCoords : TEXCOORD1;
	float2 RockCoords : TEXCOORD2;
	float2 SandCoords : TEXCOORD3;
	float2 BlendCoords : TEXCOORD4;
	float2 SnowCoords : TEXCOORD5;
	float2 WaterCoords : TEXCOORD6;
	float3 LightingFactor : NORMAL0;
};

struct PixelToFrame
{
	float4 Color : COLOR0;
};

//------- XNA-to-HLSL variables --------
float4x4 xView;
float4x4 xProjection;
float4x4 xWorld;
float3 xLightDirection;
float xAmbient;
bool xEnableLighting;
bool xShowNormals;

//------- Texture Samplers --------

/*texture2D xTexture;
SamplerState TextureSampler 
{ 
	magfilter = LINEAR; 
	minfilter = LINEAR; 
	mipfilter = LINEAR; 
	AddressU = mirror; 
	AddressV = mirror;
};*/

texture2D Blend;
SamplerState BlendSampler
{
	Texture = <Blend>;
	magfilter = LINEAR;
	minfilter = LINEAR;
	mipfilter = LINEAR;
	AddressU = CLAMP;
	AddressV = CLAMP;
};

texture2D TerrainType;
SamplerState TerrainTypeSampler
{
	Texture = <TerrainType>;
	magfilter = LINEAR;
	minfilter = LINEAR;
	mipfilter = LINEAR;
	AddressU = mirror;
	AddressV = mirror;
};

texture2D Grass;
SamplerState GrassSampler
{
	Texture = <Grass>;
	magfilter = LINEAR;
	minfilter = LINEAR;
	mipfilter = LINEAR;
	AddressU = mirror;
	AddressV = mirror;
};

texture2D Rock;
SamplerState RockSampler
{
	Texture = <Rock>;
	magfilter = LINEAR;
	minfilter = LINEAR;
	mipfilter = LINEAR;
	AddressU = mirror;
	AddressV = mirror;
};

texture2D Sand;
SamplerState SandSampler
{
	Texture = <Sand>;
	magfilter = LINEAR;
	minfilter = LINEAR;
	mipfilter = LINEAR;
	AddressU = mirror;
	AddressV = mirror;
};

texture2D Snow;
SamplerState SnowSampler
{
	Texture = <Snow>;
	magfilter = LINEAR;
	minfilter = LINEAR;
	mipfilter = LINEAR;
	AddressU = mirror;
	AddressV = mirror;
};

texture2D Water;
SamplerState WaterSampler
{
	Texture = <Water>;
	magfilter = LINEAR;
	minfilter = LINEAR;
	mipfilter = LINEAR;
	AddressU = mirror;
	AddressV = mirror;
};

//------- Technique: Pretransformed --------

VertexToPixel PretransformedVS(float4 inPos : POSITION, float4 inColor : COLOR)
{
	VertexToPixel Output = (VertexToPixel)0;

	Output.Position = inPos;
	Output.Color = inColor;

	return Output;
}

PixelToFrame PretransformedPS(VertexToPixel PSIn)
{
	PixelToFrame Output = (PixelToFrame)0;

	Output.Color = PSIn.Color;

	return Output;
}

technique Pretransformed_2_0
{
	pass Pass0
	{
		VertexShader = compile vs_4_0_level_9_1 PretransformedVS();
		PixelShader = compile ps_4_0_level_9_1 PretransformedPS();
	}
}

technique Pretransformed
{
	pass Pass0
	{
		VertexShader = compile vs_4_0_level_9_1 PretransformedVS();
		PixelShader = compile ps_4_0_level_9_1 PretransformedPS();
	}
}

//------- Technique: Colored --------

VertexToPixel ColoredVS(float4 inPos : POSITION, float4 inColor : COLOR, float3 inNormal : NORMAL)
{
	VertexToPixel Output = (VertexToPixel)0;
	float4x4 preViewProjection = mul(xView, xProjection);
	float4x4 preWorldViewProjection = mul(xWorld, preViewProjection);

	Output.Position = mul(inPos, preWorldViewProjection);
	Output.Color = inColor;

	float3 Normal = normalize(mul(normalize(inNormal), xWorld));
	Output.LightingFactor = 1;
	if (xEnableLighting)
		Output.LightingFactor = saturate(dot(Normal, -xLightDirection));

	return Output;
}

PixelToFrame ColoredPS(VertexToPixel PSIn)
{
	PixelToFrame Output = (PixelToFrame)0;

	Output.Color = PSIn.Color;
	Output.Color.rgb *= saturate(PSIn.LightingFactor + xAmbient);

	return Output;
}

technique Colored_2_0
{
	pass Pass0
	{
		VertexShader = compile vs_4_0_level_9_1 ColoredVS();
		PixelShader = compile ps_4_0_level_9_1 ColoredPS();
	}
}

technique Colored
{
	pass Pass0
	{
		VertexShader = compile vs_4_0_level_9_1 ColoredVS();
		PixelShader = compile ps_4_0_level_9_1 ColoredPS();
	}
}

//------- Technique: Textured --------

VertexToPixel TexturedVS(float4 inPos : POSITION, float4 inColor : COLOR0, float2 inTexCoords : TEXCOORD0, float2 inGrassCoords : TEXCOORD1, float2 inRockCoords : TEXCOORD2, float2 inSandCoords : TEXCOORD3, float2 inBlendCoords : TEXCOORD4, float2 inSnowCoords : TEXCOORD5, float2 inWaterCoords : TEXCOORD6, float3 inNormal : NORMAL)
{
	VertexToPixel Output = (VertexToPixel)0;
	float4x4 preViewProjection = mul(xView, xProjection);
	float4x4 preWorldViewProjection = mul(xWorld, preViewProjection);

	Output.Position = mul(inPos, preWorldViewProjection);
	Output.Color = inColor;
	//Output.TextureCoords = inTexCoords;
	Output.TerrainTypeCoords = inTexCoords;
	Output.GrassCoords = inGrassCoords;
	Output.RockCoords = inRockCoords;
	Output.SandCoords = inSandCoords;
	Output.BlendCoords = inBlendCoords;
	Output.SnowCoords = inSnowCoords;
	Output.WaterCoords = inWaterCoords;

	float3 Normal = normalize(mul(normalize(inNormal), xWorld));
	Output.LightingFactor = 1;
	if (xEnableLighting)
		Output.LightingFactor = saturate(dot(Normal, -xLightDirection));

	return Output;
}

PixelToFrame TexturedPS(VertexToPixel PSIn)
{
	PixelToFrame Output = (PixelToFrame)0;
	/*float4 Color = TerrainType.Sample(TerrainTypeSampler, PSIn.TextureCoords);
	float4 GrassClr = float4(0, 255, 0, 255);
	float4 RockClr = float4(255, 0, 0, 255);
	float4 SandClr = float4(255, 255, 0, 255);
	float4 SnowClr = float4(255, 255, 255, 255);
	float4 WaterClr = float4(12, 0, 255, 255);

	float4 Diff = Color - GrassClr;
	if (!any(Diff))
		Output.Color = tex2D(GrassSampler, PSIn.TextureCoords);

	Diff = Color - RockClr;
	if (!any(Diff))
		Output.Color = tex2D(RockSampler, PSIn.TextureCoords);

	Diff = Color - SandClr;
	if(!any(Diff))
		Output.Color = tex2D(SandSampler, PSIn.TextureCoords);

	Diff = Color - SnowClr;
	if (!any(Diff))
		Output.Color = tex2D(SnowSampler, PSIn.TextureCoords);

	Diff = Color - WaterClr;
	if(!any(Diff))
		Output.Color = tex2D(WaterSampler, PSIn.TextureCoords);*/

	//Output.Color = tex2D(TextureSampler, PSIn.TextureCoords);

	//float4 Color = tex2D(TerrainTypeSampler, PSIn.TerrainTypeCoords);
	float4 Color = TerrainType.Sample(TerrainTypeSampler, PSIn.TerrainTypeCoords);
	float4 GrassClr = float4(0, 255, 0, 255);
	float4 RockClr = float4(255, 0, 0, 255);
	float4 SandClr = float4(255, 255, 0, 255);
	float4 SnowClr = float4(255, 255, 255, 255);
	float4 WaterClr = float4(12, 0, 255, 255);

	float4 GrassResult = tex2D(GrassSampler, PSIn.GrassCoords);
	float4 RockResult = tex2D(RockSampler, PSIn.RockCoords);
	float4 SandResult = tex2D(SandSampler, PSIn.SandCoords);
	float4 SnowResult = tex2D(SnowSampler, PSIn.SnowCoords);
	float4 WaterResult = tex2D(WaterSampler, PSIn.WaterCoords);

	float4 BaseColor = TerrainType.Sample(TerrainTypeSampler, PSIn.TerrainTypeCoords);

	if (all(Color == GrassClr))
		BaseColor = GrassResult;

	if (all(Color == RockClr))
		BaseColor = RockResult;

	if (!all(Color == SandClr))
		BaseColor = SandResult;

	if (all(Color == SnowClr))
		BaseColor = SnowResult;

	if (all(Color == WaterClr))
		BaseColor = WaterResult;

	//Does color need to be assigned before control leaves the function???
	//Output.Color = tex2D(TerrainTypeSampler, PSIn.TerrainTypeCoords);

	float4 BlendColor = Blend.Sample(BlendSampler, PSIn.BlendCoords);
	//float4 BaseColor = xTexture.Sample(TextureSampler, PSIn.TextureCoords);

	Output.Color = lerp(BaseColor, BlendColor, BlendColor.r);
	Output.Color *= lerp(BaseColor, BlendColor, BlendColor.g);
	Output.Color *= lerp(BaseColor, BlendColor, BlendColor.b);
	Output.Color.rgb *= saturate(PSIn.LightingFactor + xAmbient);

	return Output;
}

technique Textured_2_0
{
	pass Pass0
	{
		VertexShader = compile vs_4_0_level_9_1 TexturedVS();
		PixelShader = compile ps_4_0_level_9_1 TexturedPS();
	}
}

technique Textured
{
	pass Pass0
	{
		VertexShader = compile vs_4_0_level_9_1 TexturedVS();
		PixelShader = compile ps_4_0_level_9_1 TexturedPS();
	}
}
