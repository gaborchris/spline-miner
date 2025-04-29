// SdfCompute.fx
// A simple Jump-Flooding SDF builder (approximate but very fast)

Texture2D<uint>    Occupancy    : register(t0);      // 1 = solid, 0 = empty
RWTexture2D<uint2> NearestSolid : register(u0);      // stores nearest solid coord
RWTexture2D<float>  SdfOut       : register(u1);      // final distance map

cbuffer Params : register(b0)
{
    int Width;
    int Height;
    int Step;  // current jump-size (will half each pass)
}

[numthreads(8,8,1)]
void JfaFlood(uint3 id : SV_DispatchThreadID)
{
    int2 pos = int2(id.xy);
    if (pos.x >= Width || pos.y >= Height) return;

    // On first pass, initialize NearestSolid to self if solid, or (−1,−1) if empty
    uint2 best = NearestSolid[pos];
    if (Step == 0)
    {
        if (Occupancy[pos] == 1) best = uint2(pos);
        else              best = uint2(0xFFFFFFFF, 0xFFFFFFFF);
    }

    // Sample the eight neighbors at +/- Step
    for (int dy = -1; dy <= 1; ++dy)
    for (int dx = -1; dx <= 1; ++dx)
    {
        int2 samplePos = pos + int2(dx, dy) * Step;
        if (samplePos.x < 0 || samplePos.y < 0 ||
            samplePos.x >= Width || samplePos.y >= Height) continue;

        uint2 candidate = NearestSolid[samplePos];
        if (candidate.x == 0xFFFFFFFF) continue;

        float2 fpos       = float2(pos);
        float2 fbest      = float2(best);
        float2 fcand      = float2(candidate);
        float  distBest   = distance(fbest, fpos);
        float  distCand   = distance(fcand, fpos);
        if (distCand < distBest) best = candidate;
    }

    NearestSolid[pos] = best;
}

[numthreads(8,8,1)]
void FinalizeSdf(uint3 id : SV_DispatchThreadID)
{
    int2 pos = int2(id.xy);
    if (pos.x >= Width || pos.y >= Height) return;
    uint2 nearest = NearestSolid[pos];
    if (nearest.x == 0xFFFFFFFF)
    {
        SdfOut[pos] = 1e6; // far away
    }
    else
    {
        SdfOut[pos] = distance(float2(nearest), float2(pos));
    }
}
