#ifndef CustomRandomShaderFunctions

#define CustomRandomShaderFunctions

float2 N22(float2 p){

    float3 a = frac(p.xyx * float3(123.34,234.34,354.65));
    a+= dot(a, a+34.45);
    return frac(float2(a.x*a.y, a.y*a.z));
}

float Remap (float value, float from1, float to1, float from2, float to2)
{
    return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
}

#endif