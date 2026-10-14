#if OPENGL
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_5_0
#define PS_SHADERMODEL ps_5_0
#endif

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
    float3 Position : POSITION0;
    float2 TexPosition : TEXCOORD0;
    float3 Normal : NORMAL0;
    float BoneBinding : BLENDWEIGHT0;
};

struct VitaboyOutput
{
    float4 Position : SV_POSITION;
    float2 TexPosition : TEXCOORD0;
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

    //BLENDWEIGHT0 carries a mesh-local *bone index*, not a blend weight.
    //Round before converting to an integer so GPU interpolation/float precision
    //never selects a fractional matrix index.
    int BoneIndex = (int)clamp(floor(Input.BoneBinding + 0.5), 0.0, 49.0);
    float4 Skinned = mul(float4(Input.Position, 1.0), ChildBones[BoneIndex]);

    float4 WorldPosition = mul(Skinned, World);
    float4 ViewPosition = mul(WorldPosition, View);
    Output.Position = mul(ViewPosition, Projection);
    Output.TexPosition = Input.TexPosition;

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
        VertexShader = compile VS_SHADERMODEL TransformVertices();
        PixelShader = compile PS_SHADERMODEL VitaboyPixelShaderFunction();
    }
}