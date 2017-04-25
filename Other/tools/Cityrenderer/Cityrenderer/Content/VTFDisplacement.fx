float4x4 world;
float4x4 view;
float4x4 proj;

float maxHeight = 128;

texture DisplacementMap;
sampler DisplacementSampler = sampler_state
{
	Texture = <DisplacementMap>;
	MipFilter = Point;
	MinFilter = Point;
	MagFilter = Point;
	AddressU = Clamp;
	AddressV = Clamp;
};

texture SandMap;
sampler SandSampler = sampler_state
{
	Texture = <SandMap>;
	MipFilter = Linear;
	MinFilter = Linear;
	MagFilter = Linear;
	AddressU = Wrap;
	AddressV = Wrap;
};

texture GrassMap;
sampler GrassSampler = sampler_state
{
	Texture = <GrassMap>;
	MipFilter = Linear;
	MinFilter = Linear;
	MagFilter = Linear;
	AddressU = Wrap;
	AddressV = Wrap;
};

texture RockMap;
sampler RockSampler = sampler_state
{
	Texture = <RockMap>;
	MipFilter = Linear;
	MinFilter = Linear;
	MagFilter = Linear;
	AddressU = Wrap;
	AddressV = Wrap;
};

texture SnowMap;
sampler SnowSampler = sampler_state
{
	Texture = <SnowMap>;
	MipFilter = Linear;
	MinFilter = Linear;
	MagFilter = Linear;
	AddressU = Wrap;
	AddressV = Wrap;
};

struct VS_INPUT {
	float4 position	: POSITION;
	float4 uv : TEXCOORD0;
};
struct VS_OUTPUT
{
	float4 uv : TEXCOORD0;
	float4 worldPos : TEXCOORD1;
	float4 textureWeights : TEXCOORD2;
	float4 position  : POSITION;
};

float textureSize = 512.0f;
float texelSize = 1.0f / 512.0f; //size of one texel;

float4 tex2Dlod_bilinear(sampler texSam, float4 uv)
{
	float4 height00 = tex2Dlod(texSam, uv);
	float4 height10 = tex2Dlod(texSam, uv + float4(texelSize, 0, 0, 0));
	float4 height01 = tex2Dlod(texSam, uv + float4(0, texelSize, 0, 0));
	float4 height11 = tex2Dlod(texSam, uv + float4(texelSize, texelSize, 0, 0));

	float2 f = frac(uv.xy * textureSize);

	float4 tA = lerp(height00, height10, f.x);
	float4 tB = lerp(height01, height11, f.x);

	return lerp(tA, tB, f.y);
}

VS_OUTPUT Transform(VS_INPUT In)
{
	VS_OUTPUT Out = (VS_OUTPUT)0;
	float4x4 viewProj = mul(view, proj);
	float4x4 worldViewProj = mul(world, viewProj);

	float height = tex2Dlod_bilinear(DisplacementSampler, float4(In.uv.xy, 0, 0)).r;

	In.position.y = height * maxHeight;
	Out.worldPos = mul(In.position, world);
	Out.position = mul(In.position, worldViewProj);
	Out.uv = In.uv;
	float4 TexWeights = 0;

	TexWeights.x = saturate(1.0f - abs(height - 0) / 0.2f);
	TexWeights.y = saturate(1.0f - abs(height - 0.3) / 0.25f);
	TexWeights.z = saturate(1.0f - abs(height - 0.6) / 0.25f);
	TexWeights.w = saturate(1.0f - abs(height - 0.9) / 0.25f);
	float totalWeight = TexWeights.x + TexWeights.y + TexWeights.z + TexWeights.w;
	TexWeights /= totalWeight;
	Out.textureWeights = TexWeights;

	return Out;
}

float4 PShader(in float4 uv : TEXCOORD0, in float4 worldPos : TEXCOORD1, in float4 weights : TEXCOORD2) : COLOR
{
	float4 Sand = tex2D(SandSampler,uv * 8);
	float4 Grass = tex2D(GrassSampler,uv * 8);
	float4 Rock = tex2D(RockSampler,uv * 8);
	float4 Snow = tex2D(SnowSampler,uv * 8);
	return Sand * weights.x + Grass * weights.y + Rock * weights.z + Snow * weights.w;
}

technique GridDraw
{
	pass P0
	{
		vertexShader = compile vs_4_0 Transform();
		pixelShader = compile ps_4_0 PShader();
	}
}