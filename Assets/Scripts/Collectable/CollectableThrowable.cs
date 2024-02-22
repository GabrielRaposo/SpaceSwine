using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class CollectableThrowable : Collectable
{
    [SerializeField] AK.Wwise.Event OnThrowAKEvent;
    [SerializeField] AK.Wwise.Event BounceBackAKEvent;

    [Header("Throw Data")]
    [SerializeField] SpriteRenderer visualComponent;
    [SerializeField] float bounceBackDuration;
    [SerializeField] float bounceBackMultiplier;
    [SerializeField] AnimationCurve bounceBackCurve;

    [Header("Particles")]
    [SerializeField] Animator breakAnimator;
    [SerializeField] ParticleSystem idleParticle;
    [SerializeField] ParticleSystem trailParticle;
    [SerializeField] ParticleSystem intenseTrailParticle;
    [SerializeField] ParticleSystem bounceParticle;
    [SerializeField] GameObject destroyParticles;

    float bounceBackCount;
    Vector2 bounceBackDirection;

    private IEnumerator rotationRoutine;
    private bool indestructible;
    
    public override void OnResetFunction() 
    {
        base.OnResetFunction();
        LocalReset();
    }

    private void LocalReset (bool clearParticles = true) 
    {
        indestructible = false;
        
        if (rotationRoutine != null)
            StopCoroutine(rotationRoutine);
        
        Collider2D collider2D = GetComponent<Collider2D>();
        if (collider2D)
            collider2D.enabled = true;

        if (innerCollider)
            innerCollider.enabled = true;

        FloatEffect floatEffect = GetComponentInChildren<FloatEffect>();
        if (floatEffect)
            floatEffect.enabled = true;

        visualComponent.transform.localEulerAngles = Vector3.zero;

        idleParticle.transform.position = trailParticle.transform.position = transform.position;
        
        bounceBackDirection = Vector2.zero;
        bounceBackCount = -1;

        if (clearParticles)
        {
            idleParticle?.Clear();
            trailParticle?.Clear();
            intenseTrailParticle?.Clear();
        }

        idleParticle?.Play();
        trailParticle?.Stop();
        intenseTrailParticle?.Stop();

        trailParticle.GetComponent<HierarchyController>()?.SetOriginalState();
        intenseTrailParticle.GetComponent<HierarchyController>()?.SetOriginalState();

        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
    }

    public override void Interact (CollectableInteraction interactor) 
    {
        if (innerCollider)
            innerCollider.enabled = true;

        interactor.LaunchInput();
    }

    public void LaunchSetup()
    {
        trailParticle?.Play();
        OnThrowAKEvent?.Post(gameObject);
            
        rotationRoutine = RotateCoroutine();
        StartCoroutine(rotationRoutine);
    }

    private IEnumerator RotateCoroutine()
    {
        while (true)
        {
            while (Time.timeScale < 1)
                yield return new WaitForFixedUpdate();

            visualComponent.transform.Rotate(Vector3.forward, 7.5f);
            yield return null;
        }
    }

    public void SetIndestructible(bool value)
    {
        indestructible = value;
    }

    public override void TriggerEvent(Collider2D collision) 
    {
        //Debug.Log("vel: " + GetComponent<Rigidbody2D>().velocity);

        Lock l = collision.GetComponent<Lock>();
        if (l)
        {
            if (l.Collect(this))
            {
                transform.position = l.transform.position;
                return;
            }
        }
        
        if (indestructible) return;
        
        base.TriggerEvent(collision);

        LockGravityField lgf = collision.GetComponent<LockGravityField>();
        if (lgf)
        {
            if (lgf.GetCollectable(this))
            {
                trailParticle.GetComponent<HierarchyController>()?.SetParent(null);
                intenseTrailParticle.GetComponent<HierarchyController>()?.SetParent(null);
                intenseTrailParticle.Play();
                return;
            }
        }

        Hitbox hb = collision.GetComponent<Hitbox>();
        if (hb && hb.damage > 0)
        {
            if(hb.damageType!= DamageType.Electricity)
                DestroyCollectable();
        } 
            

        GravitationalBody gravitationalBody = collision.GetComponent<GravitationalBody>();
        if (gravitationalBody)
        {
            BubblePlanet bubblePlanet = gravitationalBody.GetComponent<BubblePlanet>();
            if (bubblePlanet)
            {
                if (bubblePlanet.IsConcealedCollectable(this))
                    return;

                bubblePlanet.CollectableContactInteraction(this);
            }
            
            BounceBack (gravitationalBody);
        }
    }

    public override void OnCollected() 
    {
        base.OnCollected();
        
        if (rotationRoutine != null)
            StopCoroutine(rotationRoutine);

        visualComponent.transform.localEulerAngles = Vector3.zero;

        bounceBackDirection = Vector2.zero;
        bounceBackCount = -1;

        idleParticle?.Stop();
        trailParticle?.Stop();
        intenseTrailParticle?.Stop();
    }

    private void DestroyCollectable()
    {
        breakAnimator.SetTrigger("Reset");
        breakAnimator.transform.SetParent(null);
        breakAnimator.transform.position = transform.position;

        ParticleSystem particleSystem = breakAnimator.GetComponent<ParticleSystem>();
        if (particleSystem != null)
            particleSystem.Play();

        gameObject.SetActive(false);
        //Instantiate(destroyParticles, transform.position, quaternion.identity);
    }

    private void ResetToCollectableState ()
    {
        base.OnResetFunction();

        LocalReset(clearParticles: false);
    }
    
    private void BounceBack (GravitationalBody gravitationalBody)
    {
        //Debug.Log("Bounce back: " + gameObject.name);

        if (bounceParticle)
        {
            bounceParticle.transform.position = transform.position;
            bounceParticle.Play();
        }

        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        if (gravitationalBody.GetType() == typeof(GravitationalPlanet))
            bounceBackDirection = (transform.position - gravitationalBody.transform.position).normalized;
        else
            bounceBackDirection = - rb.velocity.normalized;

        bounceBackCount = bounceBackDuration;

        //Debug.Log("rb.velocity: " + rb.velocity);
        rb.velocity = Vector2.zero;

        BubblePlanet bubblePlanet = gravitationalBody.GetComponent<BubblePlanet>();
        if (bubblePlanet)
        {
            Round round = GetComponentInParent<Round>();
            if (round)
                transform.SetParent(round.transform);
            else
                transform.SetParent(null);
        }
        else transform.SetParent(gravitationalBody.transform); 
           

        if (rotationRoutine != null)
            StopCoroutine(rotationRoutine);


        FloatEffect floatEffect = GetComponentInChildren<FloatEffect>();
        if (floatEffect)
        {
            floatEffect.enabled = false;
            floatEffect.enabled = true;
        }

        visualComponent.transform.localEulerAngles = Vector3.zero;

        if (BounceBackAKEvent != null)
            BounceBackAKEvent.Post(gameObject);
    }

    private void FixedUpdate() 
    {
        if (bounceBackCount <= 0 || bounceBackDirection == Vector2.zero)
            return;

        float curveValue = bounceBackCurve.Evaluate (bounceBackCount / bounceBackDuration);
        transform.position += (Vector3) bounceBackDirection * curveValue * bounceBackMultiplier * Time.fixedDeltaTime;

        bounceBackCount -= Time.fixedDeltaTime;

        if (bounceBackCount <= 0)
        {
            bounceBackDirection = Vector2.zero;
            bounceBackCount = -1;
            
            ResetToCollectableState ();
        }
    }

    private void OnDisable() 
    {
        idleParticle?.Stop();
        trailParticle?.Stop();
        intenseTrailParticle?.Stop();
    }
}
