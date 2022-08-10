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

float Remap2(float2 value, float2 from1, float2 to1, float2 from2, float2 to2)
{
    float2 a;

    a.x = Remap(value.x, from1.x, to1.x, from2.x, to2.x);
    a.y = Remap(value.y, from1.y, to1.y, from2.y, to2.y);

    return a;
}

float2 unity_gradientNoise_dir(float2 p)
{
    p = p % 289;
    float x = (34 * p.x + 1) * p.x % 289 + p.y;
    x = (34 * x + 1) * x % 289;
    x = frac(x / 41) * 2 - 1;
    return normalize(float2(x - floor(x + 0.5), abs(x) - 0.5));
}

float unity_gradientNoise(float2 p)
{
    float2 ip = floor(p);
    float2 fp = frac(p);
    float d00 = dot(unity_gradientNoise_dir(ip), fp);
    float d01 = dot(unity_gradientNoise_dir(ip + float2(0, 1)), fp - float2(0, 1));
    float d10 = dot(unity_gradientNoise_dir(ip + float2(1, 0)), fp - float2(1, 0));
    float d11 = dot(unity_gradientNoise_dir(ip + float2(1, 1)), fp - float2(1, 1));
    fp = fp * fp * fp * (fp * (fp * 6 - 15) + 10);
    return lerp(lerp(d00, d01, fp.y), lerp(d10, d11, fp.y), fp.x);
}

float Unity_GradientNoise_float(float2 UV, float Scale)
{
    return  unity_gradientNoise(UV * Scale) + 0.5;
}

#endif