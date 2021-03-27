using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaposUtil
{
    public static Vector2 AllignVectorWithTransform (Transform t, Vector2 v)
    {
        return (t.right * v.x) + (t.up * v.y);
    }
}

public static class Vector2Extension
{
    public static Vector2 SetX (this Vector2 v, float x)
    {
        return new Vector2 (x, v.y);        
    }
}
