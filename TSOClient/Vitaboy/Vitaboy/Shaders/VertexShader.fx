float4x4 World;
float4x4 View;
float4x4 Projection;

float4 AmbientColor = float4(1, 1, 1, 1);
float AmbientIntensity = 0.1;

texture HeadTexture;

sampler HeadTextureSampler = sampler_state
{
	Texture = <HeadTexture>;
};

texture AccessoryTexture;

sampler AccessoryTextureSampler = sampler_state
{
	Texture = <AccessoryTexture>;
};

texture LeftHandTexture;

sampler LeftHandTextureSampler = sampler_state
{
	Texture = <LeftHandTexture>;
};

texture RightHandTexture;

sampler RightHandTextureSampler = sampler_state
{
	Texture = <RightHandTexture>;
};

struct VertexShaderHeadInput
{
	float4 Position : POSITION0;
	float4 Normal : NORMAL0;
	float4 TexPosition : TEXCOORD0;
};

struct VertexShaderHeadOutput
{
	float4 Position : POSITION0;
	float4 Normal : NORMAL0;
	float4 TexPosition : TEXCOORD0;
};

VertexShaderHeadOutput TransformHead(VertexShaderHeadInput Input)
{
	VertexShaderHeadOutput Output;

	float4 WorldPosition = mul(Input.Position, World);
	float4 ViewPosition = mul(WorldPosition, View);
	Output.Position = mul(ViewPosition, Projection);
	Output.Normal = mul(ViewPosition, Projection);
	Output.TexPosition = Input.TexPosition;

	return Output;
}

VertexShaderHeadOutput TransformAccessory(VertexShaderHeadInput Input)
{
	VertexShaderHeadOutput Output;

	float4 WorldPosition = mul(Input.Position, World);
	float4 ViewPosition = mul(WorldPosition, View);
	Output.Position = mul(ViewPosition, Projection);
	Output.Normal = mul(ViewPosition, Projection);
	Output.TexPosition = Input.TexPosition;

	return Output;
}

VertexShaderHeadOutput TransformLeftHand(VertexShaderHeadInput Input)
{
	VertexShaderHeadOutput Output;

	float4 WorldPosition = mul(Input.Position, World);
	float4 ViewPosition = mul(WorldPosition, View);
	Output.Position = mul(ViewPosition, Projection);
	Output.Normal = mul(ViewPosition, Projection);
	Output.TexPosition = Input.TexPosition;

	return Output;
}

VertexShaderHeadOutput TransformRightHand(VertexShaderHeadInput Input)
{
	VertexShaderHeadOutput Output;

	float4 WorldPosition = mul(Input.Position, World);
	float4 ViewPosition = mul(WorldPosition, View);
	Output.Position = mul(ViewPosition, Projection);
	Output.Normal = mul(ViewPosition, Projection);
	Output.TexPosition = Input.TexPosition;

	return Output;
}

float4 HeadPixelShaderFunction(VertexShaderHeadOutput Input) : COLOR0
{
	float4 Color = tex2D(HeadTextureSampler, Input.TexPosition);
	return Color;
}

float4 AccessoryPixelShaderFunction(VertexShaderHeadOutput Input) : COLOR0
{
	float4 Color = tex2D(AccessoryTextureSampler, Input.TexPosition);
	return Color;
}

float4 LeftHandPixelShaderFunction(VertexShaderHeadOutput Input) : COLOR0
{
	float4 Color = tex2D(LeftHandTextureSampler, Input.TexPosition);
	return Color;
}

float4 RightHandPixelShaderFunction(VertexShaderHeadOutput Input) : COLOR0
{
	float4 Color = tex2D(RightHandTextureSampler, Input.TexPosition);
	return Color;
}

technique TransformHeadTechnique
{
	pass HeadPass
	{
		VertexShader = compile vs_3_0 TransformHead();
		PixelShader = compile ps_3_0 HeadPixelShaderFunction();
	}
}

technique TransformAccessoryTechnique
{
	pass HeadPass
	{
		VertexShader = compile vs_3_0 TransformAccessory();
		PixelShader = compile ps_3_0 AccessoryPixelShaderFunction();
	}
}

technique TransformLeftHandTechnique
{
	pass HeadPass
	{
		VertexShader = compile vs_3_0 TransformLeftHand();
		PixelShader = compile ps_3_0 LeftHandPixelShaderFunction();
	}
}

technique TransformRightHandTechnique
{
	pass HeadPass
	{
		VertexShader = compile vs_3_0 TransformRightHand();
		PixelShader = compile ps_3_0 RightHandPixelShaderFunction();
	}
}