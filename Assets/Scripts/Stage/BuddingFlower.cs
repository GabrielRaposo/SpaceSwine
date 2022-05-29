using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BuddingFlower : MonoBehaviour
{
    [SerializeField] Transform visualComponent;
    [SerializeField] SpriteRenderer energyArrow;
    [SerializeField] GameObject centerLight;
    [SerializeField] AK.Wwise.Event openAKEvent;
    
    [Header("Particle System")]
    [SerializeField] ParticleSystem closedParticleSystem;
    [SerializeField] ParticleSystem openParticleSystem;
    [SerializeField] ParticleSystem burstParticleSystem;
    [SerializeField] ParticleSystem powerUpParticleSystem;
    [SerializeField] ParticleSystem energyBurstParticleSystem;
    [SerializeField] ParticleSystem energyParticleSystem;

    bool isOpen;
    Animator animator;
    BuddingFlowerGroup group;

    void Start()
    {
        animator = GetComponent<Animator>();

        group = GetComponentInParent<BuddingFlowerGroup>();
        if (group)
        {
            group.AddFlower(this);
            
            if (energyArrow)
            {
                Vector2 distance = group.DoorPosition() - transform.position;
                float angleDifference = Vector2.SignedAngle(Vector2.right, distance.normalized);
                float magnitude = distance.magnitude - .75f;
                if (magnitude < 0)
                    magnitude = 0;

                energyArrow.transform.eulerAngles = Vector3.forward * angleDifference;
                energyArrow.size = new Vector2(magnitude, energyArrow.size.y);
            }
        }

        Round round = GetComponentInParent<Round>();
        if (round)
        {
            round.OnReset += ResetComponents;
        }
    }

    private void ResetComponents()
    {
        if (visualComponent)
        {
            visualComponent.DOKill();
            visualComponent.localScale = Vector3.one;
        }

        if (centerLight)
            centerLight.SetActive(false);

        if (energyArrow) 
            energyArrow.gameObject.SetActive(false);
        
        if (burstParticleSystem) 
        {
            burstParticleSystem.Stop();
            burstParticleSystem.Clear();
        }

        closedParticleSystem?.Play();
        
        if (openParticleSystem)
        {
            openParticleSystem.Stop();
            openParticleSystem.Clear();
        }

        if (powerUpParticleSystem)
        {
            powerUpParticleSystem.Stop();
            powerUpParticleSystem.Clear();
        }

        if (energyBurstParticleSystem)
        {
            energyBurstParticleSystem.Stop();
            energyBurstParticleSystem.Clear();
        }

        if (energyParticleSystem)
        {
            energyParticleSystem.Stop();
            energyParticleSystem.Clear();
        }

        if (openAKEvent != null && openAKEvent.IsPlaying(gameObject))
        {
            openAKEvent.Stop(gameObject);
        }

        SetState(false);
    }

    private void SetState (bool value)
    {
        isOpen = value;
        animator.SetBool("Open", value);

        if (value && group)
        {
            if (closedParticleSystem)
                closedParticleSystem.Stop();

            if (openParticleSystem)
                openParticleSystem.Play();

            openAKEvent?.Post(gameObject);
            group.Activate();
        }
    }

    private void OnTriggerEnter2D (Collider2D collision) 
    {
        if (isOpen)
            return;

        if (!collision.CompareTag("Player"))
            return;

        SetState(true);
    }

    public void PunchScale()
    {
        if (!visualComponent)
            return;

        visualComponent.DOKill();
        visualComponent.localScale = Vector3.one;

        visualComponent.DOPunchScale (Vector3.one * .3f, .15f);

        burstParticleSystem?.Play();
    }

    public void PreLightUp()
    {
        if (powerUpParticleSystem)
            powerUpParticleSystem.Play();
    }

    public void LightUp()
    {
        if (openParticleSystem)
            openParticleSystem.Stop();

        if (energyBurstParticleSystem)
            energyBurstParticleSystem.Play();

        if (energyParticleSystem)
            energyParticleSystem.Play();

        if (centerLight)
            centerLight.SetActive (true);

        if (energyArrow) 
            energyArrow.gameObject.SetActive (true);
    }
}
