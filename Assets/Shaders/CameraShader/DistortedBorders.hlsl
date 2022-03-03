#include "Assets/Shaders/CustomRandomShaderFunctions.hlsl"

void DistortedBorder_float(float x, float amplitude, float frequency,float time, float4 frequencies, float4 timeFactors, float4 weight , out float y)
{
 
    //float amplitude = 1.;
    //float frequency = 1.;
    y = sin(x * frequency);
    float t = 0.01*(-time*130.0);
    y += sin(x*frequency*frequencies.x + t*timeFactors.x)*weight.x;
    y += sin(x*frequency*frequencies.y + t*timeFactors.y)*weight.y;
    y += sin(x*frequency*frequencies.z + t*timeFactors.z)*weight.z;
    y += sin(x*frequency*frequencies.w + t*timeFactors.w)*weight.w;
    y *= amplitude*0.03;   
    
}