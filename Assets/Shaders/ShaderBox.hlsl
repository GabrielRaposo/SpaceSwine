#ifndef ShaderBox

#define ShaderBox

float2 Unity_PolarCoordinates_float(float2 UV, float2 Center, float RadialScale, float LengthScale)
{
    float2 delta = UV - Center;
    float radius = length(delta) * 2 * RadialScale;
    float angle = atan2(delta.x, delta.y) * 1.0/6.28 * LengthScale;
    return float2(radius, angle);
}

float2 Polar(float2 UV, float scale)
{   
    return Unity_PolarCoordinates_float(UV, float2(0.5,0.5), 1.0, scale);
}

float2 Unity_Twirl_float(float2 UV, float2 Center, float Strength, float2 Offset)
{
    float2 delta = UV - Center;
    float angle = Strength * length(delta);
    float x = cos(angle) * delta.x - sin(angle) * delta.y;
    float y = sin(angle) * delta.x + cos(angle) * delta.y;
    return float2(x + Center.x + Offset.x, y + Center.y + Offset.y);
}

#endif
