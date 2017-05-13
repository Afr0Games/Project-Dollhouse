float4x4 World;
float4x4 View;
float4x4 Projection;

float4 AmbientColor = float4(1, 1, 1, 1);
float AmbientIntensity = 0.1;

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

	float4 WorldPosition = mul(World, Input.Position);
	float4 ViewPosition = mul(WorldPosition, View);
	Output.Position = mul(ViewPosition, Projection);
	Output.Normal = mul(ViewPosition, Projection);
	Output.TexPosition = Input.TexPosition;

	return Output;
}

float4 PixelShaderFunction(VertexShaderHeadOutput Input) : COLOR0
{
	//return AmbientColor * AmbientIntensity;
	return float4(1.0f, 0.0f, 0.0f, 1.0f);
}

technique TransformVerticesTechnique
{
	pass HeadPass
	{
		VertexShader = compile vs_3_0 TransformHead();
		PixelShader = compile ps_3_0 PixelShaderFunction();
	}
}