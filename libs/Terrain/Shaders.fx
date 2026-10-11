//Define a macro to check the target platform
#ifdef OPENGL
    //For DesktopGL (OpenGL), use vs_3_0 and ps_3_0
#define VS_SHADER_PROFILE vs_3_0
#define PS_SHADER_PROFILE ps_3_0
#else
    //For DirectX, use vs_4_0_level_9_1 and ps_4_0_level_9_1
#define VS_SHADER_PROFILE vs_4_0_level_9_1
#define PS_SHADER_PROFILE ps_4_0_level_9_1
#endif

//Vertex Shader
float4x4 World;
float4x4 View;
float4x4 Projection;

struct VertexInput
{
    float4 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;
};

struct VertexOutput
{
    float4 Position : SV_POSITION;
    float2 TexCoord : TEXCOORD0;
    float3 WorldPosition : TEXCOORD1; //Add world position
    float2 DiagnosticInfo : TEXCOORD2; //Added for debugging
};

//Vertex shader function
VertexOutput VertexShaderFunction(VertexInput input)
{
    VertexOutput output;
    float4 worldPos = mul(input.Position, World);
    output.Position = mul(mul(worldPos, View), Projection);
    output.TexCoord = input.TexCoord;
    output.WorldPosition = worldPos.xyz; //Pass world position to pixel shader

    //Debugging: Store raw world position and tile indices
    output.DiagnosticInfo.x = worldPos.x;
    output.DiagnosticInfo.y = worldPos.y;

    return output;
}

//Pixel Shader (fragment shader)
Texture2D AtlasTexture : register(t0); //Atlas texture containing all terrain types
SamplerState AtlasSampler : register(s0);

Texture2D TerrainTypeTexture : register(t1);
SamplerState TerrainTypeSampler : register(s1);

Texture2D VertexColorTexture : register(t2);
SamplerState VertexColorSampler : register(s2);

Texture2D ForestTypeTexture : register(t3);
SamplerState ForestTypeSampler : register(s3);

Texture2D ForestDensityTexture : register(t4);
SamplerState ForestDensitySampler : register(s4);

Texture2D ForestTexture : register(t5);
SamplerState ForestSampler : register(s5);

float BlockWidth;
float BlockHeight;
float2 AtlasTexelSize;
float TileSize; //Size of each tile in world units
float2 TerrainSizeInTiles; //Number of tiles in x and y in the TerrainTypeTexture

//Pixel shader function
float4 MainPS(VertexOutput input) : SV_TARGET
{
    //Compute tile indices based on world position
    float2 tileIndices = floor(float2(input.WorldPosition.x, -input.WorldPosition.y) / TileSize);

    //Compute per-quad UVs (0 to 1 within each quad)
    float2 perQuadUV = frac(float2(input.WorldPosition.x, -input.WorldPosition.y) / TileSize);

    //Compute UVs for sampling terrain type texture
    float2 tileUV = (tileIndices + 0.5) / TerrainSizeInTiles;
    tileUV = saturate(tileUV);

    //Diagnostic output - use the diagnostic info to color the output
    //This will help visualize world position and tile indices
    float4 debugColor = float4(
        frac(input.DiagnosticInfo.x / (TileSize * 10)), //Red channel based on world X
        frac(input.DiagnosticInfo.y / (TileSize * 10)), //Green channel based on world Y
        0.5, //Blue channel as a constant
        1.0  //Full opacity
    );

    //Sample the terrain type texture to get the terrain type color
    float4 terrainTypeColor = TerrainTypeTexture.Sample(TerrainTypeSampler, tileUV);

    //Sample forest density and forest type (adjusted if needed)
    float forestDensity = ForestDensityTexture.Sample(ForestDensitySampler, tileUV).r;
    float4 forestTypeColor = ForestTypeTexture.Sample(ForestTypeSampler, tileUV);

    //Initialize UV coordinates for the atlas
    float2 atlasUV = float2(0, 0);

    //Compare the components of the terrainTypeColor to determine the correct atlas block
    if (terrainTypeColor.r == 0 && terrainTypeColor.g == 1 && terrainTypeColor.b == 0) //Green: Grass
        atlasUV = float2(0, 0);
    else if (terrainTypeColor.r == 0 && terrainTypeColor.g == 0 && terrainTypeColor.b == 1) //Blue: Water
        atlasUV = float2(BlockWidth, 0);
    else if (terrainTypeColor.r == 1 && terrainTypeColor.g == 0 && terrainTypeColor.b == 0) //Red: Rock
        atlasUV = float2(BlockWidth, BlockHeight);
    else if (terrainTypeColor.r == 1 && terrainTypeColor.g == 1 && terrainTypeColor.b == 1) //White: Snow
        atlasUV = float2(0, 2 * BlockHeight);
    else if (terrainTypeColor.r == 1 && terrainTypeColor.g == 1 && terrainTypeColor.b == 0) //Yellow: Sand
        atlasUV = float2(0, BlockHeight);
    else
        atlasUV = float2(0, 0); //Default to grass (green)

    //Calculate the final UV coordinates for the atlas using perQuadUV
    // Keep bilinear footprints inside an atlas cell rather than bleeding into neighbors.
    float2 atlasTexel = AtlasTexelSize;
    float2 finalUV = atlasUV + atlasTexel + perQuadUV * (float2(BlockWidth, BlockHeight) - 2.0 * atlasTexel);

    //Sample the atlas texture
    float4 sampledTexture = AtlasTexture.Sample(AtlasSampler, finalUV);

    //Sample the vertex color texture and apply vertex color
    float4 vertexColor = VertexColorTexture.Sample(VertexColorSampler, input.TexCoord);
    
    //Only apply forest if density is above a threshold
    if (forestDensity > 0.1) //Adjust threshold as needed
    {
        //Use forest type to select a tile from the forest atlas
        float2 forestTileUV;
        forestTileUV.x = (floor(forestTypeColor.r * 4) + frac(perQuadUV.x * 50)) / 4;
        forestTileUV.y = (floor(forestTypeColor.g * 4) + frac(perQuadUV.y * 50)) / 4;

        //float4 forestSample = ForestTexture.Sample(Sampler, forestTileUV);
        float4 forestSample = ForestTexture.Sample(ForestSampler, forestTileUV);
        
        //Only place a tree if noise is below forest density
        if (forestSample.a > 0.1)
            //Blend the forest sample with the base terrain
            sampledTexture = lerp(sampledTexture, forestSample, forestSample.a);
    }

    //If the texture is water, skip blending with vertex color to keep it pure
    if (terrainTypeColor.r == 0 && terrainTypeColor.g == 0 && terrainTypeColor.b == 1) //Blue: Water
        //No blending with vertex color for water
        return sampledTexture;
    else
        //For other textures, blend with the vertex color
        return sampledTexture * vertexColor;
}

//Technique to combine the shaders
technique BasicTechnique
{
    pass Pass1
    {
        VertexShader = compile VS_SHADER_PROFILE VertexShaderFunction();
        PixelShader = compile PS_SHADER_PROFILE MainPS();
    }
}
