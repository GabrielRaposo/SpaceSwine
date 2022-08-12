using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Curve
{
    private Vector2 p1;
    private Vector2 p2;
    private Vector2 p3;
    private Vector2 p4;

    private float lenght;
    
    public Curve(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4)
    {
        this.p1 = p1;
        this.p2 = p2;
        this.p3 = p3;
        this.p4 = p4;

        lenght = GetLengthApproximately(0f,1f);
    }

    public Vector2 GetPoint(float x)
    {
        return Mathf.Pow(1f - x, 3f) * p1
               + 3 * Mathf.Pow(1f - x, 2f) * x * p2
               + 3 * (1f - x) * Mathf.Pow(x, 2f) * p3
               + Mathf.Pow(x, 3f) * p4;
    }

    public float GetRotationAtPoint(float point)
    {
        var pos = GetPoint(point);
        var prevPos = GetPoint(point - 0.05f);

        return Mathg.AngleOfTheLineBetweenTwoPoints(prevPos, pos);
    }
    public Vector2 GetNormalizedPoint(float x)
    {
        x = Mathf.Clamp(x, 0f, 1f);
        
        float targetLenght = lenght * x;
        
        const float tolerance = 0.01f;
        
        float leftLimit = 0f;
        float rightLimit = 1f;

        float testPoint;

        int safety = 0;
        
        while (true)
        {
            testPoint = (leftLimit + rightLimit) / 2f;

            if (safety == 0)
                testPoint = x;
            
            float resultLenght = GetLengthApproximately(0f, testPoint, 50f);
            
            if(Mathf.Abs(resultLenght-targetLenght) < tolerance)
                break;

            if (resultLenght < targetLenght)
            {
                leftLimit = testPoint;
            }
            else
            {
                rightLimit = testPoint;
            }

            safety++;
            if(safety>5000)
                break;

        }

        return GetPoint(testPoint);
    }
    
    
    public float GetLengthApproximately( float startNormalizedT, float endNormalizedT, float accuracy = 50f )
    {
        if( endNormalizedT < startNormalizedT )
        {
            float temp = startNormalizedT;
            startNormalizedT = endNormalizedT;
            endNormalizedT = temp;
        }

        if( startNormalizedT < 0f )
            startNormalizedT = 0f;

        if( endNormalizedT > 1f )
            endNormalizedT = 1f;

        float step = AccuracyToStepSize( accuracy ) * ( endNormalizedT - startNormalizedT );

        float length = 0f;
        Vector3 lastPoint = GetPoint( startNormalizedT );
        for( float i = startNormalizedT + step; i < endNormalizedT; i += step )
        {
            Vector3 thisPoint = GetPoint( i );
            length += Vector3.Distance( thisPoint, lastPoint );
            lastPoint = thisPoint;
        }

        length += Vector3.Distance( lastPoint, GetPoint( endNormalizedT ) );

        return length;
    }
    
    private float AccuracyToStepSize( float accuracy )
    {
        if( accuracy <= 0f )
            return 0.2f;

        return Mathf.Clamp( 1f / accuracy, 0.001f, 0.2f );
    }
}
