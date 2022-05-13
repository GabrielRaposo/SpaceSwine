using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class CollectableThrowable : Collectable
{
    [SerializeField] AK.Wwise.Event OnThrowAKEvent;
    [SerializeField] SpriteRenderer visualComponent;
    
    [Header("Particles")]
    [SerializeField] ParticleSystem idleParticle;
    [SerializeField] ParticleSystem trailParticle;
    [SerializeField] ParticleSystem intenseTrailParticle;
    [SerializeField] GameObject destroyParticles;

    private IEnumerator rotationRoutine;

    private bool indestructible;
    
    public override void OnResetFunction() 
    {
        base.OnResetFunction();

        indestructible = false;
        
        if(rotationRoutine!=null)
            StopCoroutine(rotationRoutine);
        
        Collider2D collider2D = GetComponent<Collider2D>();
        if (collider2D)
            collider2D.enabled = true;

        FloatEffect floatEffect = GetComponentInChildren<FloatEffect>();
        if (floatEffect)
            floatEffect.enabled = true;

        visualComponent.transform.localEulerAngles = Vector3.zero;

        idleParticle.transform.position = trailParticle.transform.position = transform.position;
        idleParticle?.Clear();
        idleParticle?.Play();
        trailParticle?.Clear();
        trailParticle?.Stop();
        intenseTrailParticle?.Clear();
        intenseTrailParticle?.Stop();

        trailParticle.GetComponent<HierarchyController>()?.SetOriginalState();
        intenseTrailParticle.GetComponent<HierarchyController>()?.SetOriginalState();

        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
    }

    public override void Interact (CollectableInteraction interactor) 
    {
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
        if (hb)
        {
            if (hb.damage > 0)
            {
                Debug.Log($"{gameObject.name} <color=#dd0000><b>Take damage</b></color>");
                DestroyKey();
            }
                
        }

        GravitationalBody gravitationalBody = collision.GetComponent<GravitationalBody>();
        if (gravitationalBody)
        {
            ResetToCollectableState();
        }
    }

    public override void OnCollected() 
    {
        base.OnCollected();
        
        idleParticle?.Stop();
        trailParticle?.Stop();
        intenseTrailParticle?.Stop();
    }

    private void DestroyKey()
    {
        gameObject.SetActive(false);
        Instantiate(destroyParticles, transform.position, quaternion.identity);
    }

    private void ResetToCollectableState()
    {
        OnResetFunction();
    }

    private void OnDisable() 
    {
        idleParticle?.Stop();
        trailParticle?.Stop();
        intenseTrailParticle?.Stop();
    }
}
