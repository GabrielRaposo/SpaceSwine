using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GravitationalPlanet))]
public class BubblePlanet : CustomSurface, ConcealingBody
{
    [Space(10)]
    [SerializeField] Collectable concealedCollectable;

    [Space(5)]
    [SerializeField] float feedbackDuration;

    [Header("Land Feedback")]
    [SerializeField] float landPushback;
    [SerializeField] AnimationCurve pushbackCurve;

    [Header("Tremble Feedback")]
    [SerializeField] float trembleMultiplier;
    [SerializeField] AnimationCurve trembleCurve;
    
    [Header("Soft Floating")]
    [SerializeField] float softFloatMultiplier;
    [SerializeField] float softFloatDuration;
    [SerializeField] AnimationCurve softFloatCurve;

    [Header("Effects")]
    [SerializeField] ParticleSystem onLandPS;
    [SerializeField] ParticleSystem burstPS;
    [SerializeField] AK.Wwise.Event burstAKEvent;

    [Header("References")]
    [SerializeField] Collider2D gravityCollider;
    [SerializeField] Transform distortionAnchor;
    [SerializeField] List<SpriteRenderer> renderers;

    bool isActive;
    float floatTime;
    float feedbackTime;
    Transform player;

    CircleCollider2D coll2D;
    GravitationalPlanet gravitationalPlanet;

    private void Awake() 
    {
        coll2D = GetComponent<CircleCollider2D>();
        gravitationalPlanet = GetComponent<GravitationalPlanet>();
    }

    private void Start() 
    {
        Initiate();

        gravitationalPlanet.OnLandAction += OnPlayerLand;
        gravitationalPlanet.OnPlayerExitAction += Burst;

        Round round = GetComponentInParent<Round>();
        if (!round)
            return;

        round.OnReset += Initiate;
    }

    private void Initiate()
    {
        if (concealedCollectable)
        {
            concealedCollectable.transform.position = transform.position;
            concealedCollectable.transform.SetParent(transform);
            concealedCollectable.SetConcealingBody(this);
        }

        player = null;

        floatTime = feedbackTime = 0;

        distortionAnchor.eulerAngles = Vector3.zero;
        foreach (SpriteRenderer r in renderers)
            r.transform.eulerAngles = Vector3.zero;

        SetState(true);
    }

    private void OnValidate() 
    {
        if (Application.isPlaying)
            return;

        transform.localPosition = Vector2.zero;

        if (concealedCollectable)
            concealedCollectable.transform.position = transform.position;
    }

    public void Burst()
    {
        if (player)
        {
            SpaceJumper spaceJumper = player.GetComponent<SpaceJumper>();
            Vector2 direction = player.transform.position - transform.position;
            spaceJumper.TryLaunchIntoDirection(direction);
        }
        player = null;

        if (concealedCollectable && concealedCollectable.transform.parent == transform)
            concealedCollectable.transform.SetParent (null);

        burstPS?.Play();
        if (burstAKEvent != null)
            burstAKEvent.Post(gameObject);

        SetState(false);
    }

    public void OnPlayerLand (Transform player)
    {
        this.player = player;

        feedbackTime = feedbackDuration;

        onLandPS.Play();
    }

    private void SetState (bool value)
    {
        if (!value && transform.parent != null)
        {
            CustomMove customMove = transform.parent.GetComponent<CustomMove>();
            if (customMove)
                customMove.PauseMovement();
        }

        foreach (SpriteRenderer renderer in renderers)
            renderer.enabled = value;

        coll2D.enabled = value;
        gravityCollider.enabled = value;

        isActive = value;
    }
    private void FixedUpdate() 
    {
        AllignDistortionAnchor();

        if (feedbackTime > 0)
        {
            feedbackTime -= Time.fixedDeltaTime;

            Tremble();
            Pushback();

            if (feedbackTime <= 0)
            {
                distortionAnchor.localScale = Vector3.one;
                SetComponentsPosition(Vector2.zero);

                floatTime = 0;
            }
            return;
        }

        SoftFloat();
    }

    private void AllignDistortionAnchor()
    {
        if (player == null)
            return;

        Vector2 direction = (player.position - transform.position).normalized;
        float angle = Vector2.SignedAngle(Vector2.up, direction);

        distortionAnchor.eulerAngles = Vector3.forward * angle;
        foreach (SpriteRenderer r in renderers)
            r.transform.eulerAngles = Vector3.zero;
    }

    private void Tremble()
    {
        float t = 1 - (feedbackTime / feedbackDuration);

        float y = trembleCurve.Evaluate(t);
        float yy = 1f + ((y - 1f) * trembleMultiplier);
        distortionAnchor.localScale = new Vector2(2f - yy, yy);
    }

    private void Pushback()
    {
        if (player == null)
            return;

        float t = 1 - (feedbackTime / feedbackDuration);

        Vector2 direction = (transform.position - player.position).normalized;
        float intensity = pushbackCurve.Evaluate(t) * landPushback;
        
        SetComponentsPosition(direction * intensity);
    }

    private void SoftFloat()
    {
        transform.localPosition = distortionAnchor.up * softFloatCurve.Evaluate(floatTime/softFloatDuration); 

        floatTime += Time.fixedDeltaTime;

        if (floatTime > softFloatDuration)
            floatTime = 0;
    }

    private void SetComponentsPosition (Vector2 localPosition)
    {
        transform.localPosition = localPosition;
    }

    public void CollectableContactInteraction (Collectable collectable)
    {
        if (concealedCollectable == collectable)
            return;

        Burst();
    }

    public bool IsConcealedCollectable (Collectable collectable) => concealedCollectable == collectable;

    public bool IsActive() 
    {
        return isActive;
    }
}
