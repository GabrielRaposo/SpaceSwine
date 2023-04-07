using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

[RequireComponent(typeof(SpriteShapeController))]
public class BrambleHazard : MonoBehaviour
{
    const float DURATION_MODIFIER_THRESHOLD = .1f; 

    [SerializeField] int branchIndex = -1;

    [Header("Decoration")]
    [SerializeField] Transform decorationBulb;
    [SerializeField] GameObject decorationPrefab;

    [Header("Slight Movement")]
    [SerializeField] float movementDuration;
    [SerializeField] float movementIntensity;
    [SerializeField] AnimationCurve movementCurve;

    Spline spline;
    Spline parentSpline;
    List<Vector3> originalPoints;
    SpriteShapeController shapeController;

    float t;

    private void Awake() 
    {
        shapeController = GetComponent<SpriteShapeController>();
        spline = shapeController.spline;

        originalPoints = new List<Vector3>();
        for (int i = 0; i < spline.GetPointCount(); i++)
        {
            originalPoints.Add (spline.GetPosition(i));
        }

        movementDuration *= 1 + Random.Range (-DURATION_MODIFIER_THRESHOLD, DURATION_MODIFIER_THRESHOLD);
        t = Random.Range(0f, movementDuration);
    }

    private void Start() 
    {
        SetupBranch();
        SetupDecorations();
       
        if (!decorationBulb)
            return;

        decorationBulb.gameObject.SetActive (spline.isOpenEnded);
        UpdateBulb();
    }

    private void SetupBranch()
    {
        if (branchIndex < 0)
            return;

        Transform parent = transform.parent;
        if (parent == null)
            return;

        BrambleHazard parentBramble = parent.GetComponent<BrambleHazard>();
        if (!parentBramble)
            return;

        parentSpline = parentBramble.GetComponent<SpriteShapeController>().spline;
        transform.localPosition = 
            parentSpline.GetPosition (branchIndex % parentSpline.GetPointCount()) 
            - GetComponent<SpriteShapeController>().spline.GetPosition(0);
    }

    private void UpdateBulb()
    {
        Vector2 direction = spline.GetLeftTangent(spline.GetPointCount() - 1);
        float angle = Vector2.SignedAngle (Vector2.left, direction.normalized);
        decorationBulb.localEulerAngles = Vector3.forward * angle;

        decorationBulb.localPosition = spline.GetPosition(spline.GetPointCount() - 1);
    }

    private void OnValidate() 
    {
        SetupBranch();
    }

    private void SetupDecorations()
    {
        if (decorationPrefab == null)
            return;

        for (int i = 1; i < spline.GetPointCount() - 1; i++)
        {
            GameObject decoObject = Instantiate(decorationPrefab, transform);
            BrambleHazardDecoration decoScript = decoObject.GetComponent<BrambleHazardDecoration>();
            if (decoScript)
                decoScript.Setup(spline, i);
        }

        decorationPrefab.SetActive(false);
    }

    void Update()
    {
        if (parentSpline != null)
        {
            transform.localPosition = 
                parentSpline.GetPosition (branchIndex % parentSpline.GetPointCount()) - spline.GetPosition(0);
        }

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

        if (decorationBulb)
            UpdateBulb();

        if (t == movementDuration)
            t = 0;
    }
}
