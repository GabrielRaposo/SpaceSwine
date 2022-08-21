using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationShipSoundController : MonoBehaviour
{
    [SerializeField] float volumeFade;

    [Header("WWise Events")]
    [SerializeField] AK.Wwise.Event startMovementAKEvent;
    [SerializeField] AK.Wwise.Event loopMovementAKEvent;
    [SerializeField] AK.Wwise.Event endMovementAKEvent;
    [SerializeField] AK.Wwise.RTPC movementRTPC;

    float parameterVolume;

    enum State {None, Intro, Loop, Outro};
    State state;

    private void OnEnable() 
    {
        parameterVolume = 0;
        state = State.None;
    }

    public void ReadInput (Vector2 input, float intensity)
    {

        if (movementRTPC != null)
        {
        //Debug.Log("intensity: " + intensity);
            // 0 -> 100  :  100 -> 0
            parameterVolume = (intensity / 1.84f) * 100;
            movementRTPC.SetGlobalValue(parameterVolume);
        }

        if (input != Vector2.zero)
        {
            //parameterVolume    


            PlaySoundLogic();
        }
        else
        {
            StopSoundLogic();
        }
    }

    private void PlaySoundLogic()
    {
        if (startMovementAKEvent == null || loopMovementAKEvent == null || endMovementAKEvent == null)
            return;

        //Debug.Log("Enter " + state);
        switch (state)
        {
            default:
            case State.None:
            case State.Outro:
                KillAllSounds();
                // -- Inicia parâmetro
                startMovementAKEvent.Post(gameObject);
                state = State.Intro;
                Debug.Log("Inicia");
                break;

            case State.Intro:
                // -- Cresce parâmetro
                if (!startMovementAKEvent.IsPlaying(gameObject))
                {
                    loopMovementAKEvent.Post(gameObject);
                    state = State.Loop;
                    Debug.Log("Troca pro loop");
                }
                break;

            case State.Loop:
                if (!loopMovementAKEvent.IsPlaying(gameObject))
                {
                    loopMovementAKEvent.Post(gameObject);
                    state = State.Loop;
                    Debug.Log("Mantendo o loop");
                }
                break;
        }
    }

    private void StopSoundLogic()
    {
        if (startMovementAKEvent == null || loopMovementAKEvent == null || endMovementAKEvent == null)
            return;

        switch (state)
        {
            default:
                if (startMovementAKEvent.IsPlaying(gameObject))
                    startMovementAKEvent.Stop(gameObject);

                if (loopMovementAKEvent.IsPlaying(gameObject))
                    loopMovementAKEvent.Stop(gameObject);
                
                if (!endMovementAKEvent.IsPlaying(gameObject))
                    endMovementAKEvent.Post(gameObject);

                state = State.Outro;
                break;

            case State.None:
            case State.Outro:
                break;
        }
    }

    private void KillAllSounds()
    {
        if (startMovementAKEvent != null)
            startMovementAKEvent.Stop(gameObject);

        if (loopMovementAKEvent != null)
            loopMovementAKEvent.Stop(gameObject);

        if (endMovementAKEvent != null)
            endMovementAKEvent.Stop(gameObject);
    }

    private void OnDisable() 
    {
        parameterVolume = 0;
        KillAllSounds();
        state = State.None;
    }

    private void OnDestroy() 
    {
        KillAllSounds();
        state = State.None;    
    }
}
