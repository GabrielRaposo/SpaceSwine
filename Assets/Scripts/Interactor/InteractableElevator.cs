﻿using System.Collections;
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
    [SerializeField] Color fadeOutBaseColor;
    [SerializeField] Color fadeOutBurstColor;
    [SerializeField] SpriteRenderer fadeOutLights;

    bool active;

    private void Start() 
    {
        SetupColliderPosition();
        SetActivation(startActive);

        testInput.performed += (c) => { SetActivation(!active); };
        testInput.Enable();
    }

    public void SetActivation (bool value)
    {
        mainSpriteSwapper?.SetSpriteState(value ? 0 : 1);
        lightsObject?.SetActive(value);
        if (wavesParticleSystem)
        {
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

    #if UNITY_EDITOR
    private void Update() 
    {
        if (Application.isPlaying)
            return;

        SetupColliderPosition();
    }
    #endif

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
            spaceJumper.LaunchIntoDirection(transform.up);
        }        
    }

    private void LaunchParticlesRoutine()
    {
        launchParticleSystem?.Play();
        wavesParticleSystem?.Stop();
        StartCoroutine( RaposUtil.WaitSeconds(2f, () => wavesParticleSystem?.Play() ) );
        if (fadeOutLights)
        {
            fadeOutLights.color = fadeOutBurstColor;
            fadeOutLights.DOColor(fadeOutBaseColor, .5f);
        }
    }

    protected override void HighlightState (bool value) 
    {
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
