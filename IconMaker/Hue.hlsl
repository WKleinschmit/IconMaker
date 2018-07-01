sampler2D implicitInputSampler : register(S0);

float3 HUEtoRGB(in float H)
{
    float R = abs(H * 6 - 3) - 1;
    float G = 2 - abs(H * 6 - 2);
    float B = 2 - abs(H * 6 - 4);
    return saturate(float3(R, G, B));
}

float4 main(float2 uv : TEXCOORD) : COLOR
{
    float hue = (atan2((1 - uv.x) - 0.5, uv.y - 0.5) / 6.283185307179586476925286766559) + 0.5;
    return float4(HUEtoRGB(hue), 1.0);
}