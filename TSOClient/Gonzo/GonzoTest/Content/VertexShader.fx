float4x4 World;
float4x4 View;
float4x4 Projection;

//float4x4 AbsoluteBoneMatrix; //The absolute matrix of a specific bone. Set by the AvatarBase class.

float4 AmbientColor = float4(1, 1, 1, 1);
float AmbientIntensity = 0.1;

texture BodyTexture;

sampler BodyTextureSampler = sampler_state
{
	Texture = <BodyTexture>;
};

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

//The position in this struct should be a relative vertex (see Avatarbase.cs - TransformVertices())
struct VertexShaderBodyInput
{
	float4 Position : POSITION0;
	float4 Normal : NORMAL0;
	float4 TexPosition : TEXCOORD0;
};

struct VertexShaderBodyOutput
{
	float4 Position : POSITION0;
	float4 Normal : NORMAL0;
	float4 TexPosition : TEXCOORD0;
};

float4x4 CreateTranslation(float x, float y, float z)
{
	return float4x4(1, 0, 0, 0,
		0, 1, 0, 0,
		0, 0, 1, 0,
		x, y, z, 1);
}

VertexShaderBodyOutput TransformBody(VertexShaderBodyInput Input)
{
	VertexShaderBodyOutput Output;

	//Transforms the relative vertex passed to the shader by the bone's absolute matrix.
	float4x4 TranslatedMatrix = CreateTranslation(Input.Position.x, Input.Position.y, Input.Position.z) *
		/*AbsoluteBoneMatrix*/World;

	Output.Position = mul(float3(0, 0, 0), TranslatedMatrix);
	Output.TexPosition = Input.TexPosition;

	TranslatedMatrix = CreateTranslation(Input.Normal.x, Input.Normal.y, Input.Normal.z) * /*AbsoluteBoneMatrix*/ World;

	Output.Normal = mul(float3(0, 0, 0), TranslatedMatrix);

	return Output;
}

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

float4 BodyPixelShaderFunction(VertexShaderBodyOutput Input) : COLOR0
{
	float4 Color = tex2D(BodyTextureSampler, Input.TexPosition);
	return Color;
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

technique TransformBodyTechnique
{
	pass HeadPass
	{
		VertexShader = compile vs_3_0 TransformBody();
		PixelShader = compile ps_3_0 BodyPixelShaderFunction();
	}
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