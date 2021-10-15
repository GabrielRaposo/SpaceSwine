using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RaposUtil
{
    public static Vector2 AlignWithTransform (Transform t, Vector2 v)
    {
        return (t.right * v.x) + (t.up * v.y);
    }

    public static Vector2 RotateVector(Vector2 v, float a)
    {
        float radians = a * Mathf.Deg2Rad;
        float sin = Mathf.Sin(radians);
        float cos = Mathf.Cos(radians);

        float tx = v.x;
        float ty = v.y;

        return new Vector2(cos * tx - sin * ty, sin * tx + cos * ty);
    }

    public static double AngleDifference (double angle1, double angle2)
    {
        double diff = ( angle2 - angle1 + 180 ) % 360 - 180;
        return diff < -180 ? diff + 360 : diff;
    }

    public static IEnumerator Wait (int frames, UnityAction action)
    {
        if (frames > 0)
        {
            for (int i = 0; i < frames; i++)
                yield return new WaitForEndOfFrame();
        }

        action?.Invoke();
    }

    public static IEnumerator WaitSeconds (float duration, UnityAction action)
    {
        if (duration > 0)
        {
            yield return new WaitForSeconds(duration);
        }

        action?.Invoke();
    }
}

public static class Vector2Extension
{
    public static Vector2 SetX (this Vector2 v, float x)
    {
        return new Vector2 (x, v.y);        
    }
}

public static class RectTransformExtensions
 {
    public static void MoveX (this RectTransform rt, float x)
    {
        rt.offsetMin = new Vector2(x, rt.offsetMin.y);
        rt.offsetMax = new Vector2(x, rt.offsetMax.y);
    }

    public static void MoveY (this RectTransform rt, float y)
    {
        rt.offsetMin = new Vector2(rt.offsetMin.x, y);
        rt.offsetMax = new Vector2(rt.offsetMax.x, y);
    }

    public static void Move (this RectTransform rt, float x, float y)
    {
        rt.offsetMin = new Vector2(x, rt.offsetMin.y);
        rt.offsetMax = new Vector2(x, rt.offsetMax.y);

        rt.offsetMin = new Vector2(rt.offsetMin.x, y);
        rt.offsetMax = new Vector2(rt.offsetMax.x, y);
    }

    public static void SetLeft(this RectTransform rt, float left)
    {
        rt.offsetMin = new Vector2(left, rt.offsetMin.y);
    }
 
    public static void SetRight(this RectTransform rt, float right)
    {
        rt.offsetMax = new Vector2(-right, rt.offsetMax.y);
    }
 
    public static void SetTop(this RectTransform rt, float top)
    {
        rt.offsetMax = new Vector2(rt.offsetMax.x, -top);
    }
 
    public static void SetBottom(this RectTransform rt, float bottom)
    {
        rt.offsetMin = new Vector2(rt.offsetMin.x, bottom);
    }
 }