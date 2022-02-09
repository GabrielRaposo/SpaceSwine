#include "Assets/Shaders/CustomRandomShaderFunctions.hlsl"

void GetScreenBorders_float(float2 Screen, float2 Resolution, float BorderSize, out float2 Out0, out float2 Out1, out float2 Out2, out float2 Out3, out float2 OutSum)
{
    Out0 = float2(0,0);
    Out1 = float2(0,0);
    Out2 = float2(0,0);
    Out3 = float2(0,0);
    OutSum = float2(0,0);

    if(Screen.y <= BorderSize)
    {
        Out0.x = Screen.x;
        Out0.y = Remap(Screen.y, 0, BorderSize, 0, 1);
    }

    if(Screen.x >= Resolution.x-BorderSize)
    {
        Out1.x = Screen.y;
        Out1.y = Remap(Screen.x, Resolution.x, Resolution.x-BorderSize, 0, 1);
    }

    if(Screen.y >= Resolution.y-BorderSize)
    {
        Out2.x = Screen.x;
        Out2.y = Remap(Screen.y, Resolution.y, Resolution.y-BorderSize, 0, 1);
    }

    if(Screen.x <= BorderSize)
    {
        Out3.x = Screen.y;
        Out3.y = Remap(Screen.x, 0, BorderSize, 0, 1);
    }

    if(Out0.x > float(0))
    {
        OutSum = Out0+Out2;
    }
    else if(Out2.x > float(0))
    {
        OutSum = Out0+Out2; 
    }
    else
    {
        OutSum = Out0+Out1+Out2+Out3;
    }
    
    
    
}