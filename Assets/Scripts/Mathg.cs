using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Mathg
{
    public static Vector3 AngleToDirection(float angle)
    {
        return ((Vector3) (Quaternion.Euler(0, 0, angle) * Vector2.right)).normalized;
    }
    
    public static Vector3 AngleToDirection(float angle, Vector2 forward, bool invertRotation)
    {
        return ((Vector3) (Quaternion.Euler(0, 0, angle*(invertRotation?-1:1)) * forward.normalized)).normalized;
    }

    public static float VectorToAngle(Vector3 vector)
    {
        return Vector3.Angle(Vector3.up, vector);
    }



    public static float AngleOfTheLineBetweenTwoPoints(Vector3 start, Vector3 end)
    {
        Vector3 rotationVector = end - start;

        float side = (end.x > start.x) ? -1 : 1;
        
        return VectorToAngle(rotationVector) * side;
    }

    public static float Remap (this float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }
    
    public static Vector2 Remap(this Vector2 value, Vector2 from1, Vector2 to1, Vector2 from2, Vector2 to2)
    {
        float x = value.x.Remap(from1.x, to1.x, from2.x, to2.x);
        float y = value.y.Remap(from1.y, to1.y, from2.y, to2.y);
    
        return new Vector2(x, y);
    }
    
    public static Vector3 Remap(this Vector3 value, Vector3 from1, Vector3 to1, Vector3 from2, Vector3 to2)
    {
        float x = value.x.Remap(from1.x, to1.x, from2.x, to2.x);
        float y = value.y.Remap(from1.y, to1.y, from2.y, to2.y);
        float z = value.z.Remap(from1.z, to1.z, from2.z, to2.z);
    
        return new Vector3(x, y, z);
    }
    
}