#include "Assets/Shaders/CustomRandomShaderFunctions.hlsl"

void voronoiNoise_float(float2 uv, float time, float amount, float manhatan, out float Out, out float idOut){
    
    Out = 1;    

    uv *= amount;

    float2 gv = frac(uv) - 0.5;
    float2 id = floor(uv);
    
    float minDist = 100.0;
    float2 cellIndex = float2(0,0);
    
    for(float y = -1.0; y<=1.0;y++)
    {
        for(float x =-1.0;x<=1.0;x++)
        {        
            float2 offs = float2(x,y);
            
            float2 n = N22(float2(id+offs));            
            float2 p = offs+sin(n*time)*0.5;
        
            p-=gv;
            float ed = length(p);
            float md = abs(p.x) +abs(p.y);
            
            float d = lerp(ed,md,manhatan);
            
            if(d<minDist)
            {
                minDist = d;
                cellIndex = id+offs;
            }
        
        }
    }
    
    Out = minDist;
    
    idOut = N22(cellIndex);
    
    

}