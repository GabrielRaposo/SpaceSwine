using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatEffect : MonoBehaviour
{
    [SerializeField] float intensity;
    [SerializeField] float duration;
    [SerializeField] bool allignWithPlanet;
    [SerializeField] AnimationCurve curve;
    [SerializeField] LayerMask gravityLayerMask;

    Vector2 direction;
    Vector2 A, B;

    float t;
    float moveDirection = 1;

    bool moving;
    bool queued;

    void OnEnable()
    {
        moveDirection = 1;

        TurnOn();
    }

    public void TurnOn()
    {
        direction = Vector2.up;

        //if (allignWithPlanet)
        //    AllignWithGravity();
        //else if (queued)
        //    AllignWithParent();

        UpdateAnchorPoints();

        t = .5f;
        moving = true;
    }

    private void UpdateAnchorPoints()
    {
        A = direction * intensity * moveDirection;
        B = direction * intensity * moveDirection * -1;
    }

    public void LineUp()
    {
        return;

        queued = true;
        TurnOn();
    }

    private void Update() 
    {
        if (Time.timeScale < .1f || !moving) return;

        //if (allignWithPlanet)
        //    AllignWithGravity();
        //else if (queued)
        //    AllignWithParent();

        Vector2 lerpedPos = Vector2.Lerp(A, B, t / duration);
        lerpedPos *= curve.Evaluate(t / duration);

        transform.localPosition = lerpedPos;

        t += Time.deltaTime;
        if (t >= duration)
        {
            moveDirection *= -1;
            UpdateAnchorPoints();
            t = 0;
        }
    }

    private void AllignWithParent()
    {
        Transform parent = transform.parent;
        if (parent == null)
            return;

        direction = RaposUtil.AlignWithTransform(parent, Vector2.up);
    }

    private void AllignWithGravity()
    {
        Collider2D circleCast = Physics2D.OverlapCircle(transform.position, radius: .1f, gravityLayerMask);
        if (circleCast == null)
            return;

        GravityArea gravityArea = circleCast.GetComponentInChildren<GravityArea>();
        if (gravityArea == null)
            return;

        direction = (transform.position - gravityArea.transform.position).normalized;
    }
    
    private void OnDisable() 
    {
        queued = false;
        transform.localPosition = Vector2.zero;
    }
}
