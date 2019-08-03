float4x4 World;
float4x4 View;
float4x4 Projection;

float4 AmbientColor = float4(1, 1, 1, 1);
float AmbientIntensity = 0.1;

texture VitaboyTexture;

float4x4 ChildBones[50];

sampler VitaboyTextureSampler = sampler_state
{
	Texture = <VitaboyTexture>;
};

struct VitaboyInput
{
	float4 Position : POSITION0;
	float4 TexPosition : TEXCOORD0;
	float4 Normal : NORMAL0;
	float BoneBinding : BLENDWEIGHT0;
};

struct VitaboyOutput
{
	float4 Position : SV_POSITION;
	float4 TexPosition : TEXCOORD0;
	float4 Normal : NORMAL0;
};

float4x4 CreateTranslation(float x, float y, float z)
{
	return float4x4(1, 0, 0, 0,
		0, 1, 0, 0,
		0, 0, 1, 0,
		x, y, z, 1);
}

VitaboyOutput TransformVertices(VitaboyInput Input)
{
	VitaboyOutput Output;

	float4 Position = mul(Input.Position, ChildBones[Input.BoneBinding]);

	float4 WorldPosition = mul(Position, World);
	float4 ViewPosition = mul(WorldPosition, View);
	Output.Position = mul(ViewPosition, Projection);
	Output.TexPosition = Input.TexPosition;
	Output.Normal = mul(ViewPosition, Projection);

	return Output;
}

float4 VitaboyPixelShaderFunction(VitaboyOutput Input) : COLOR0
{
	float4 Color = tex2D(VitaboyTextureSampler, Input.TexPosition);
	return Color;
}

technique TransformVerticesTechnique
{
	pass HeadPass
	{
		VertexShader = compile vs_4_0_level_9_1 TransformVertices();
		PixelShader = compile ps_4_0_level_9_1 VitaboyPixelShaderFunction();
	}
}