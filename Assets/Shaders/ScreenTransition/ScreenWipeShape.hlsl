#include "Assets/Shaders/ShaderBox.hlsl"
#include "Assets/Shaders/CustomRandomShaderFunctions.hlsl"

void screenWipeShape_float(float2 uv, float x, float twirlStrenght, float blobRange, float rangeScale, float seed, out float Out)
{
    float p;

    uv = Unity_Twirl_float(uv, float2(0.5,0.5), twirlStrenght, float2(0,0));    
        
    p = Unity_PolarCoordinates_float(uv, float2(0.5,0.5),1.0,1.0).y;    


    
    p = Unity_GradientNoise_float(float2(seed,p), rangeScale);

    p = Remap(p, 0.0,1.0,-blobRange,0.0);

    p+=x;

    float circle;

    
    uv-=float2(0.5,0.5);
    circle = uv.x*uv.x+uv.y*uv.y;
    circle = Remap(circle, 0.0, 0.23, 0, 1.0);

    
    Out = step(p, circle);
    
}

