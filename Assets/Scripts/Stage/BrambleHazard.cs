using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

[RequireComponent(typeof(SpriteShapeController))]
public class BrambleHazard : MonoBehaviour
{
    const float DURATION_MODIFIER_THRESHOLD = .1f; 

    [SerializeField] float movementDuration;
    [SerializeField] float movementIntensity;
    [SerializeField] AnimationCurve movementCurve;

    Spline spline;
    List<Vector3> originalPoints;

    float t;

    private void Awake() 
    {
        SpriteShapeController shapeController = GetComponent<SpriteShapeController>();
        spline = shapeController.spline;

        originalPoints = new List<Vector3>();
        for (int i = 0; i < spline.GetPointCount(); i++)
        {
            originalPoints.Add (spline.GetPosition(i));
        }

        movementDuration *= 1 + Random.Range (-DURATION_MODIFIER_THRESHOLD, DURATION_MODIFIER_THRESHOLD);
        t = Random.Range(0f, movementDuration);
    }

    void Update()
    {
        t += Time.deltaTime;
        if (t > movementDuration)
            t = movementDuration;

        float modifier = movementCurve.Evaluate(t / movementDuration);
    
        for (int i = 0; i < spline.GetPointCount() && i < originalPoints.Count; i++)
        {
            Vector3 direction = spline.GetLeftTangent(i).normalized;
            if (direction == Vector3.zero)
                continue;

            Vector3 movement = direction * modifier * movementIntensity;
            spline.SetPosition(i, originalPoints[i] + movement);
        }

        if (t == movementDuration)
            t = 0;
    }
}
