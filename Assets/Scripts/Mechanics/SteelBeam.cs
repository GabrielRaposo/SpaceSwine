using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteelBeam : MonoBehaviour
{
    const float PLATFORM_OFFSET = .11f;

    [SerializeField] float length;
    
    [Header("Land")]
    [SerializeField] float landFeedbackDuration;
    [SerializeField] float landPushbackMultiplier;
    [SerializeField] AnimationCurve landPushbackCurve;

    [Header("Exit")]
    [SerializeField] float exitFeedbackDuration;
    [SerializeField] float exitFeedbackMultiplier;
    [SerializeField] AnimationCurve exitWiggleCurve;

    [Header("References")]
    [SerializeField] CustomRotate customRotate;
    [SerializeField] SpriteRenderer visualComponent;
    [SerializeField] Transform colliders;
    [SerializeField] Transform leftHitbox;
    [SerializeField] Transform rightHitbox;
    [SerializeField] List<GravitationalPlatform> platforms;

    float landFeedbackTime;
    float exitFeedbackTime;

    float contactPointModifier;
    float savedRotation;
    Transform player;

    private void OnValidate() 
    {
        UpdateAttributes();
    }

    private void UpdateAttributes()
    {
        if (length <= 0)
            return;

        if (visualComponent)
            visualComponent.size = new Vector2(length, .44f);

        if (leftHitbox)
            leftHitbox.localPosition  = Vector2.left * length * .5f;

        if (rightHitbox)
            rightHitbox.localPosition = Vector2.right * length * .5f;

        foreach (GravitationalPlatform platform in platforms)
            platform.UpdateLength(length - PLATFORM_OFFSET);
    }

    void Start()
    {
        if (colliders != null)
        {
            SpriteRenderer[] spriteRenderers = colliders.GetComponentsInChildren<SpriteRenderer>();
            foreach (var sr in spriteRenderers)
                sr.enabled = false;
        }

        foreach (GravitationalPlatform p in platforms)
        {
            p.OnLandAction += OnLandAction;
            p.OnPlayerExitAction += OnPlayerExitAction;
        }

        UpdateAttributes();

        Round round = GetComponentInParent<Round>();
        if (round)
        {
            round.OnReset += () => 
            {
                exitFeedbackTime = landFeedbackTime = 0;
            };
        }
    }

    private void UpdateContactPointModifier()
    {
        if (player == null)
            return;

        contactPointModifier = 1f;
        Vector3 positionOffset = player.position - transform.position;
        positionOffset = RaposUtil.RotateVector(positionOffset, - transform.eulerAngles.z);

        if (positionOffset.y > 0)
            contactPointModifier = -1f;
    }

    private void OnLandAction (Transform player)
    {
        this.player = player;
        UpdateContactPointModifier();

        landFeedbackTime = landFeedbackDuration;
        exitFeedbackTime = 0;
    }

    private void OnPlayerExitAction()
    {
        customRotate.PauseRotation();

        UpdateContactPointModifier();
        player = null;

        savedRotation = transform.eulerAngles.z;
        exitFeedbackTime = exitFeedbackDuration;
        landFeedbackTime = 0;
    }

    private void FixedUpdate() 
    {
        if (landFeedbackTime > 0)
        {
            landFeedbackTime -= Time.fixedDeltaTime;
            LandFeedback();

            if (landFeedbackTime  <= 0)
                transform.localPosition = Vector3.zero;

            return;
        }

        if (exitFeedbackTime > 0)
        {
            exitFeedbackTime -= Time.fixedDeltaTime;

            ExitFeedback();
            
            if (exitFeedbackTime <= 0)
            {
                transform.eulerAngles = Vector3.forward * savedRotation;
                customRotate.ResumeRotation();
            }
        }
    }

    private void LandFeedback()
    {
        if (player == null)
            return;

        transform.localPosition = 
            transform.up * 
            contactPointModifier * 
            landPushbackMultiplier * 
            landPushbackCurve.Evaluate( 1 - landFeedbackTime/landFeedbackDuration ); 
    }

    private void ExitFeedback()
    {
        //float angle = exitWiggleCurve.Evaluate(1 - exitFeedbackTime/exitFeedbackDuration) * exitFeedbackMultiplier;
        //transform.eulerAngles = Vector3.forward * (savedRotation + angle);

        transform.localPosition = 
            1.5f *
            transform.up * 
            contactPointModifier * 
            landPushbackMultiplier * 
            landPushbackCurve.Evaluate( 1 - exitFeedbackTime/exitFeedbackDuration); 
    }

    private void OnDrawGizmos() 
    {
        if (!customRotate || !customRotate.enabled)
            return;

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, (length / 2f ) + .1f);
    }
}
