/*This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at
http://mozilla.org/MPL/2.0/.

The Original Code is the Terrain library.

The Initial Developer of the Original Code is
Mats 'Afr0' Vederhus. All Rights Reserved.

Contributor(s):
*/

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
    float4 WorldPos = mul(input.Position, World);
    output.Position = mul(mul(WorldPos, View), Projection);
    output.TexCoord = input.TexCoord;
    output.WorldPosition = WorldPos.xyz; //Pass world position to pixel shader

    //Debugging: Store raw world position and tile indices
    output.DiagnosticInfo.x = WorldPos.x;
    output.DiagnosticInfo.y = WorldPos.y;

    return output;
}

//Legacy pixel-shader syntax for MonoGame DesktopGL (ps_3_0).
//A sampler2D owns its texture binding, avoiding separate resource allocation.
#ifdef OPENGL
texture AtlasTexture;
sampler2D AtlasSampler : register(s0) = sampler_state
{
    Texture = <AtlasTexture>;
    MinFilter = Linear;
    MagFilter = Linear;
    MipFilter = Point;
    AddressU = Clamp;
    AddressV = Clamp;
};

texture TerrainTypeTexture;
sampler2D TerrainTypeSampler : register(s1) = sampler_state
{
    Texture = <TerrainTypeTexture>;
    MinFilter = Point;
    MagFilter = Point;
    MipFilter = Point;
    AddressU = Clamp;
    AddressV = Clamp;
};

texture VertexColorTexture;
sampler2D VertexColorSampler : register(s2) = sampler_state
{
    Texture = <VertexColorTexture>;
    MinFilter = Linear;
    MagFilter = Linear;
    MipFilter = Point;
    AddressU = Clamp;
    AddressV = Clamp;
};

texture ForestTypeTexture;
sampler2D ForestTypeSampler : register(s3) = sampler_state
{
    Texture = <ForestTypeTexture>;
    MinFilter = Point;
    MagFilter = Point;
    MipFilter = Point;
    AddressU = Clamp;
    AddressV = Clamp;
};

texture ForestDensityTexture;
sampler2D ForestDensitySampler : register(s4) = sampler_state
{
    Texture = <ForestDensityTexture>;
    MinFilter = Point;
    MagFilter = Point;
    MipFilter = Point;
    AddressU = Clamp;
    AddressV = Clamp;
};

texture ForestTexture;
sampler2D ForestSampler : register(s5) = sampler_state
{
    Texture = <ForestTexture>;
    MinFilter = Linear;
    MagFilter = Linear;
    MipFilter = Point;
    AddressU = Clamp;
    AddressV = Clamp;
};

texture TransitionTexture;
sampler2D TransitionSampler : register(s6) = sampler_state
{
    Texture = <TransitionTexture>;
    MinFilter = Point;
    MagFilter = Point;
    MipFilter = Point;
    AddressU = Clamp;
    AddressV = Clamp;
};

texture TransitionMaskAtlasTexture;
sampler2D TransitionMaskAtlasSampler : register(s7) = sampler_state
{
    Texture = <TransitionMaskAtlasTexture>;
    MinFilter = Linear;
    MagFilter = Linear;
    MipFilter = Point;
    AddressU = Clamp;
    AddressV = Clamp;
};

texture RoadMapTexture;
sampler2D RoadMapSampler : register(s8) = sampler_state
{
    Texture = <RoadMapTexture>;
    MinFilter = Point;
    MagFilter = Point;
    MipFilter = Point;
    AddressU = Clamp;
    AddressV = Clamp;
};

texture RoadAtlasTexture;
sampler2D RoadAtlasSampler : register(s9) = sampler_state
{
    Texture = <RoadAtlasTexture>;
    MinFilter = Linear;
    MagFilter = Linear;
    MipFilter = Point;
    AddressU = Clamp;
    AddressV = Clamp;
};

texture RoadSizeTexture;
sampler2D RoadSizeSampler : register(s10) = sampler_state
{
    Texture = <RoadSizeTexture>;
    MinFilter = Point;
    MagFilter = Point;
    MipFilter = Point;
    AddressU = Clamp;
    AddressV = Clamp;
};

#define SAMPLE_TEXTURE(TEX, SAMP, UV) tex2D(SAMP, UV)
#else
//DirectX profile retains the original SM4 Texture2D + SamplerState bindings.
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

//R=current local type, G=last greater neighbour (L,U,R,D),
//B=Maxis transition mask, A=(tile X+tile Y)%2. Created on the CPU.
Texture2D TransitionTexture : register(t6);
SamplerState TransitionSampler : register(s6);

//Original grayscale masks loaded from individual trans*.tga files at startup.
//Packed IN MEMORY for GPU sampling; no atlas is shipped or written to disk.
//Columns: masks 00..15. Rows: transa?a, transa?b, transb?a, transb?b.
Texture2D TransitionMaskAtlasTexture : register(t7);
SamplerState TransitionMaskAtlasSampler : register(s7);

//Original city RoadMap.bmp indexed pixels, pre-encoded as two nibble selectors.
Texture2D RoadMapTexture : register(t8);
SamplerState RoadMapSampler : register(s8)
{
    Filter = Point;
    AddressU = Clamp;
    AddressV = Clamp;
};

//All 32 original road/roadcorner TGAs, packed only in VRAM at startup.
Texture2D RoadAtlasTexture : register(t9);
SamplerState RoadAtlasSampler : register(s9)
{
    Filter = Linear;
    AddressU = Clamp;
    AddressV = Clamp;
};

Texture2D RoadSizeTexture : register(t10);
SamplerState RoadSizeSampler : register(s10)
{
    Filter = Point;
    AddressU = Clamp;
    AddressV = Clamp;
};

#define SAMPLE_TEXTURE(TEX, SAMP, UV) TEX.Sample(SAMP, UV)
#endif
float2 RoadAtlasSize;
float2 RoadCellSize;
float RoadsEnabled;
float ForestEnabled;

//Reconstructed early-preview look: user-provided screenshots are the visual reference.
//This is NOT a recovered Maxis preview palette or modified game artwork.
//No new texture sampler: this adjusts already-sampled RGB values only.
float PreviewColorsEnabled;
float3 PreviewGrassTint;  //e.g. (1.08, 1.33, 0.55) to boost lush green
float3 PreviewWaterTint;  //e.g. (0.12, 0.52, 0.68) to deepen blue
float3 PreviewWaterBias;  //e.g. (0.00, 0.016, 0.045) to retain blue detail
float3 PreviewSandTint;   //muted, grey-beige shore (preview-inspired)
float3 PreviewSandBias;
float3 PreviewRockTint;   //less saturated, deep reddish-brown rock
float3 PreviewRockBias;
float3 PreviewSnowTint;   //cooler, slightly darker mountain snow
float3 PreviewSnowBias;

float3 ApplyPreviewColor(float3 rgb, float terrainId)
{
    if (PreviewColorsEnabled < 0.5)
        return rgb;

    //Local Hoodtest IDs: 0 grass, 1 water, 2 sand, 3 rock, 4 snow.
    //Apply before transition mixing to preserve the original shoreline masks.
    if (terrainId < 0.5)
        return saturate(rgb * PreviewGrassTint);
    if (terrainId < 1.5)
        return saturate(rgb * PreviewWaterTint + PreviewWaterBias);
    if (terrainId < 2.5)
        return saturate(rgb * PreviewSandTint + PreviewSandBias);
    if (terrainId < 3.5)
        return saturate(rgb * PreviewRockTint + PreviewRockBias);
    if (terrainId < 4.5)
        return saturate(rgb * PreviewSnowTint + PreviewSnowBias);

    return rgb;
}

//One bank per row: row 0 = road; row 1 = roadcorner.
//Padding is 2 pixels, with source edge replication. UVs sample texel centers
//in the actual source extent (main and corner images may have different sizes).
float4 SampleRoadSprite(float index, float bank, float2 localUV)
{
    float4 PackedSize = SAMPLE_TEXTURE(RoadSizeTexture, RoadSizeSampler, float2((index + 0.5) / 16.0, (bank + 0.5) / 2.0));
    float2 SpriteSize = floor(PackedSize.rb * 255.0 + 0.5) + floor(PackedSize.ga * 255.0 + 0.5) * 256.0;
    float2 Texel = float2(index, bank) * RoadCellSize + 2.5 + saturate(localUV) * (SpriteSize - 1.0);
    return SAMPLE_TEXTURE(RoadAtlasTexture, RoadAtlasSampler, Texel / RoadAtlasSize);
}

float4 CompositeRoad(float4 background, float4 overlay)
{
    //Straight alpha. Road TGA transparency is retained by FileManager.
    float alpha = saturate(overlay.a);
    return float4(lerp(background.rgb, overlay.rgb, alpha), background.a);
}

float BlockWidth;
float BlockHeight;
float2 AtlasTexelSize;
float TileSize; //Size of each tile in world units
float2 TerrainSizeInTiles; //Number of tiles in x and y in the TerrainTypeTexture
float2 TransitionMaskAtlasSize; //Runtime GPU texture dimensions (1088, 272).
float UseTransitionSetB;       //Used for manual preview (T key).
float AutomaticTransitionFamily; //Enable recovered scale-branch approximation.
float ProjectedTileHeightPixels; //3D-perspective equivalent of original Sy.
float BaseTextureRepeatTiles; //Maxis's 4-tile periodic base UV pattern.

//Local Hoodtest rank mapping, not yet confirmed as the original six Maxis IDs.
//0=grass, 1=water, 2=sand, 3=rock, 4=snow.
float2 AtlasOffset(float id)
{
    if (id < 0.5) 
        return float2(0.0, 0.0);                  //grass
    if (id < 1.5) 
        return float2(BlockWidth, 0.0);           //water
    if (id < 2.5) 
        return float2(0.0, BlockHeight);          //sand
    if (id < 3.5) 
        return float2(BlockWidth, BlockHeight);   //rock
    if (id < 4.5) 
        return float2(0.0, 2.0 * BlockHeight);    //snow
    
    return float2(0.0, 0.0);                                //invalid/black
}

float4 SampleAtlasType(float id, float2 perQuadUV)
{
    //Preserve the half-texel atlas inset from the prior tiling fix.
    float2 finalUV = AtlasOffset(id) + AtlasTexelSize + perQuadUV * (float2(BlockWidth, BlockHeight) - 2.0 * AtlasTexelSize);
    return SAMPLE_TEXTURE(AtlasTexture, AtlasSampler, finalUV);
}

//Original artwork is greyscale: white = secondary terrain, black = primary.
//Each tile has a two-pixel replicated border to avoid adjacent-mask bleeding.
float SampleOriginalTransitionMask(float mask, float parity, float2 uv,
                                   float primary, float secondary)
{
    float UseSetB = UseTransitionSetB;
    if (AutomaticTransitionFamily > 0.5)
    {
        //Original 0x10006AAD: use second texture-record pair when Sy < 16,
        //OR when neither of the two material IDs equals 4 or 5.
        //Treat transb as second pair provisionally: the exact transa/b mapping
        //and Hoodtest's compressed five-ID material order remain unverified.
        bool special = (abs(primary - 4.0) < 0.5 || abs(primary - 5.0) < 0.5 ||
                        abs(secondary - 4.0) < 0.5 || abs(secondary - 5.0) < 0.5);
        UseSetB = (ProjectedTileHeightPixels < 16.0 || !special) ? 1.0 : 0.0;
    }
    float row = (UseSetB > 0.5 ? 2.0 : 0.0) + parity;
    float2 uvPixel = float2(mask, row) * 68.0 + 2.5 + uv * 63.0;
    return SAMPLE_TEXTURE(TransitionMaskAtlasTexture, TransitionMaskAtlasSampler,
                                              uvPixel / TransitionMaskAtlasSize).r;
}

float4 MainPS(VertexOutput input) : SV_TARGET
{
    float2 WorldTile = float2(input.WorldPosition.x, -input.WorldPosition.y) / TileSize;
    float2 TileIndices = floor(WorldTile);
    float2 PerQuadUV = frac(WorldTile);
    float2 TileUV = saturate((TileIndices + 0.5) / TerrainSizeInTiles);

    float4 Transition = SAMPLE_TEXTURE(TransitionTexture, TransitionSampler, TileUV);
    float Primary = floor(Transition.r * 255.0 + 0.5);
    float Secondary = floor(Transition.g * 255.0 + 0.5);
    float Mask = floor(Transition.b * 255.0 + 0.5);
    float Parity = floor(Transition.a * 255.0 + 0.5);

    //Keep the original BMP bound as a diagnostic/fallback for missing data.
    float4 TerrainTypeColor = SAMPLE_TEXTURE(TerrainTypeTexture, TerrainTypeSampler, TileUV);
    if (Primary > 4.5)
    {
        //Actual terraintype.bmp uses RGB(12,0,255) for water.
        if (TerrainTypeColor.b > 0.9 && TerrainTypeColor.r < 0.15 && TerrainTypeColor.g < 0.15)
            Primary = 1.0;
        else
            Primary = 0.0;

        Secondary = Primary;
    }

    float4 vertexColor = SAMPLE_TEXTURE(VertexColorTexture, VertexColorSampler, input.TexCoord);
    //ViewerClient cycles the base terrain texture across 4x4 city tiles.
    //The transition masks, in contrast, occupy one tile each.
    float2 BaseUV = frac(WorldTile / BaseTextureRepeatTiles);
    float4 SampledTexture = SampleAtlasType(Primary, BaseUV);
    if (Primary != 1.0) //Water should not inherit terrain vertex-colour tint.
        SampledTexture *= vertexColor;

    float4 SecondaryTexture = SampleAtlasType(Secondary, BaseUV);
    if (Secondary != 1.0)
        SecondaryTexture *= vertexColor;

    //Grade each material BEFORE transition blending, preserving smooth shoreline masks.
    SampledTexture.rgb = ApplyPreviewColor(SampledTexture.rgb, Primary);
    SecondaryTexture.rgb = ApplyPreviewColor(SecondaryTexture.rgb, Secondary);

    if (Mask < 14.5 && abs(Primary - Secondary) > 0.5)
    {
        float Weight = SampleOriginalTransitionMask(Mask, Parity, PerQuadUV, Primary, Secondary);
        SampledTexture = lerp(SampledTexture, SecondaryTexture, Weight);
    }

    float ForestDensity = SAMPLE_TEXTURE(ForestDensityTexture, ForestDensitySampler, TileUV).r;
    if (ForestEnabled > 0.5 && ForestDensity > 0.1 && Primary != 1.0)
    {
        float4 ForestTypeColor = SAMPLE_TEXTURE(ForestTypeTexture, ForestTypeSampler, TileUV);
        float2 ForestTileUV;
        ForestTileUV.x = (floor(ForestTypeColor.r * 4.0) + frac(PerQuadUV.x * 50.0)) / 4.0;
        ForestTileUV.y = (floor(ForestTypeColor.g * 4.0) + frac(PerQuadUV.y * 50.0)) / 4.0;
        float4 ForestSample = SAMPLE_TEXTURE(ForestTexture, ForestSampler, ForestTileUV);
        if (ForestSample.a > 0.1)
            SampledTexture = lerp(SampledTexture, ForestSample, ForestSample.a);
    }

    //Original ViewerClient channel-2 encoding: bits 0..3 road00..15,
    //bits 4..7 roadcorner00..15. Both are drawn after the terrain material.
    //In Hoodtest we composite after the current forest placeholder as well,
    //so its repeated forest texture cannot paint across the roads.
    if (RoadsEnabled > 0.5 && WorldTile.x >= 0.0 && WorldTile.y >= 0.0 && WorldTile.x < TerrainSizeInTiles.x && WorldTile.y < TerrainSizeInTiles.y)
    {
        float4 RoadFlags = SAMPLE_TEXTURE(RoadMapTexture, RoadMapSampler, TileUV);
        float RoadIndex = floor(RoadFlags.r * 255.0 + 0.5);
        float CornerIndex = floor(RoadFlags.g * 255.0 + 0.5);
        
        if (RoadIndex > 0.5)
            SampledTexture = CompositeRoad(SampledTexture, SampleRoadSprite(RoadIndex, 0.0, PerQuadUV));
        if (CornerIndex > 0.5)
            SampledTexture = CompositeRoad(SampledTexture, SampleRoadSprite(CornerIndex, 1.0, PerQuadUV));
    }
    return SampledTexture;
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
