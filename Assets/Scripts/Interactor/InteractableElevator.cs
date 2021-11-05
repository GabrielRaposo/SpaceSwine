using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

[ExecuteInEditMode]
public class InteractableElevator : Interactable
{
    [SerializeField] InputAction testInput;

    [Header("Elevator References")]
    [SerializeField] bool startActive;
    [Space(5)]
    [SerializeField] SpriteSwapper mainSpriteSwapper;
    [SerializeField] GameObject lightsObject;
    [SerializeField] ParticleSystem wavesParticleSystem;
    [SerializeField] ParticleSystem launchParticleSystem;
    [SerializeField] List<ParticleSystem> subParticles;
    [SerializeField] Color fadeOutBaseColor;
    [SerializeField] Color fadeOutBurstColor;
    [SerializeField] SpriteRenderer fadeOutLights;
    [SerializeField] GameObject inputHelper;

    bool active;

    private void Start() 
    {
        inputHelper?.SetActive(false);

        SetupColliderPosition();
        SetActivation(startActive);

        //testInput.performed += (c) => { SetActivation(!active); };
        //testInput.Enable();
    }

    public void SetActivation (bool value)
    {
        mainSpriteSwapper?.SetSpriteState(value ? 0 : 1);
        lightsObject?.SetActive(value);
        if (wavesParticleSystem)
        {
            float angle = Vector2.SignedAngle(Vector2.up, transform.up);
            ParticleSystem.MainModule mainModule = wavesParticleSystem.main;
            mainModule.startRotation = angle * Mathf.Deg2Rad * -1f;

            if (value)
                wavesParticleSystem.Play();
            else
                wavesParticleSystem.Stop();
        }
        if (fadeOutLights)
        {
            fadeOutLights.DOKill();
            fadeOutLights.gameObject.SetActive(value);
            fadeOutLights.color = fadeOutBaseColor;
        }

        active = value;
    }

    public override void SetInteraction(bool value) 
    {
        base.SetInteraction(value);

        Debug.Log("elevator interaction");
        SetActivation(value);
    }

#if UNITY_EDITOR
    private void Update() 
    {
        if (Application.isPlaying)
            return;

        SetupColliderPosition();
    }
    #endif

    private void LateUpdate() 
    {
        if (!Application.isPlaying)
            return;

        UpdateParticlesAngle();    
    }

    private void SetupColliderPosition()
    {
        PlanetBlock planetBlock = GetComponent<PlanetBlock>();
        if (!planetBlock)
            return;

        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
        if (!boxCollider)
            return;

        boxCollider.offset = Vector2.up * (planetBlock.GetSize() - .35f);
    }

    public override void Interaction (PlayerInteractor interactor) 
    {
        if (!active)
            return;

        base.Interaction(interactor);

        if (interactor)
        {
            SpaceJumper spaceJumper = interactor.GetComponent<SpaceJumper>();
            if (!spaceJumper)
                return;

            LaunchParticlesRoutine();
            spaceJumper.LaunchIntoDirection(transform.up, multiplier: 2.0f);
        }        
    }

    private void LaunchParticlesRoutine()
    {
        inputHelper?.SetActive(false);

        if (launchParticleSystem)
        {
            UpdateParticlesAngle();
            launchParticleSystem?.Play();
        }
        wavesParticleSystem?.Stop();
        StartCoroutine( RaposUtil.WaitSeconds(2f, () => wavesParticleSystem?.Play() ) );
        if (fadeOutLights)
        {
            fadeOutLights.color = fadeOutBurstColor;
            fadeOutLights.DOColor(fadeOutBaseColor, .5f);
        }
    }

    private void UpdateParticlesAngle()
    {
        float angle = Vector2.SignedAngle(Vector2.up, transform.up);
        ParticleSystem.MainModule mainModule = launchParticleSystem.main;
        mainModule.startRotation = angle * Mathf.Deg2Rad * -1f;       

        foreach (ParticleSystem s in subParticles)
        {
            mainModule = s.main;
            mainModule.startRotation = angle * Mathf.Deg2Rad * -1f; 
        }
    }

    protected override void HighlightState (bool value) 
    {
        inputHelper?.SetActive(value);
    }

    public override void IconState (bool value) 
    {
        //if (ballonSpriteSwapper)
        //    ballonSpriteSwapper.SetVisibility(value);
    }

    private void OnDrawGizmos() 
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(  transform.position, transform.position + (transform.up * 10) );
    }
}
