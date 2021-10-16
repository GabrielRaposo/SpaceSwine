#include "Assets/Shaders/CustomRandomShaderFunctions.hlsl"

void voronoiNoise_float(float2 uv, float time,float amount, out float Out, out float idOut, out float2 cellPos){
    
    Out = 1;    

    uv*=amount;
    
    float2 gv = frac(uv) - 0.5;
    float2 id = floor(uv); //baseCell
    
    float minDist = 100.0;
    float2 cellIndex = float2(0,0);
    
    float2 closestCell;
    float2 toClosestCell;
    
    cellPos = float2(0.0,0.0);    
    
    for(float y = -1.0; y<=1.0;y++)
    {
        for(float x =-1.0;x<=1.0;x++)
        {        
            float2 offs = float2(x,y);
            float2 cell = id+offs;
            
            float2 n = N22(float2(cell));            
            float2 p = offs+sin(n*time)*0.5;
            
            float2 cellPosition = cell+n;           
        
            p-=gv; //toCell
            
            float d = length(p);
            
            if(d<minDist)
            {
                minDist = d;
                closestCell = cell;
                toClosestCell = p;
                cellIndex = cell;                
                cellPos = cell/amount;
            }
        
        }
    }
    
    float minEdgeDistance = 100.0;
    
    for(float y = -1.0; y<=1.0;y++)
    {
        for(float x =-1.0;x<=1.0;x++)
        {        
            float2 offs = float2(x,y);
            float2 cell = id+offs;
            
            float2 n = N22(float2(id+offs));            
            float2 p = offs+sin(n*time)*0.5;
            
            float2 cellPosition = cell+n;
        
            p-=gv; //toCell
            
            
            float2 diffToClosestCell = abs(closestCell-cell); //?
            
            bool isClosestCell = diffToClosestCell.x + diffToClosestCell.y < 0.1;
            
            if(!isClosestCell)
            {
                float2 toCenter = (toClosestCell + p)*0.5;
                float2 cellDifference = normalize(p - toClosestCell);
                float edgeDistance = dot(toCenter, cellDifference);
                minEdgeDistance = min(minEdgeDistance, edgeDistance);
            }
        
        }
    }
    
    //Out = minDist;
    Out = minEdgeDistance;    
    idOut = N22(cellIndex);    
    
}