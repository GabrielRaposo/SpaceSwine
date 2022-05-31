using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

public class TrailerSceneCaller : MonoBehaviour
{
    [SerializeField] InputAction turnOnInput; // input para ligar
    [SerializeField] InputAction shakeInput;  // input para chamar o ScreenShake + Buyk Pulando + possíveis efeitos visuais

    [Space(10)]
    
    [SerializeField] [TextArea(minLines: 2, maxLines: 3)] List<string> shipDialogue;
    
    [Header("References")]
    [SerializeField] ShipDialogueBox shipDialogueBox;
    [SerializeField] GalaxyParallaxTester galaxyParallax;
    [SerializeField] Animator shipScreens;
    [SerializeField] ScreenShakeCaller screenShake;
    [SerializeField] GameObject relaxingBuyk;
    [SerializeField] GameObject startledBuyk;
    [SerializeField] ParticleSystem smokePS;
    [SerializeField] GameObject redLight;

    Sequence sequence;

    private void OnEnable() 
    {
        turnOnInput.performed += TurnOn;
        shakeInput.performed += Shake;

        turnOnInput.Enable();    
        shakeInput.Enable();
    }

    private void TurnOn (InputAction.CallbackContext ctx)
    {
        if (sequence != null)
        {
            sequence.Complete();
            sequence.Kill();
        }

        sequence = DOTween.Sequence();

        // -- Reseta paralaxe do fundo
        sequence.AppendCallback( () => galaxyParallax.PlaySequence() );
        sequence.AppendInterval(5.0f);

        // -- Mostra a caixa de texto
        float t1 = .5f;
        sequence.AppendCallback( () => shipScreens.SetTrigger("TurnOn") );
        sequence.AppendCallback( () => relaxingBuyk.GetComponentInChildren<Animator>().SetTrigger("LockTop") );
        sequence.AppendCallback( () => shipDialogueBox.SetShown(true, t1) );
        sequence.AppendInterval( t1 + .5f);

        // -- Leitura dos diálogos
        foreach (string s in shipDialogue)
        {
            sequence.AppendCallback( () => shipDialogueBox.Type(s) );

            float t2 = 3.0f; 
            if (s.Length < 50) 
                t2 = 1.0f; 
            else if (s.Length < 100) 
                t2 = 2.5f;

            t2 += s.Length * .015f;
            sequence.AppendInterval(t2);
        }

        sequence.AppendCallback( () => shipDialogueBox.Type(" ") );
        sequence.AppendCallback( () => shipDialogueBox.SetShown(false) );
    }

    private void Shake (InputAction.CallbackContext ctx)
    {
        if (sequence != null)
            sequence.Kill();

        screenShake.CallScreenshake();

        relaxingBuyk.SetActive(false);
        startledBuyk.SetActive(true);

        smokePS.Play();
        redLight.SetActive(true);
        shipScreens.SetTrigger("Break");

        Camera cam = Camera.main;
        if (cam)
        {
            float t = .3f;
            cam.DOOrthoSize(1.75f, t);
            cam.transform.DORotate(Vector3.forward * 2f, t);
        }

        shipDialogueBox.transform.DOPunchScale(Vector3.one * .8f, duration: .3f);
        shipDialogueBox.transform.DOLocalRotate(Vector3.forward * 3, duration: .1f);

        Sequence s = DOTween.Sequence();
        s.AppendCallback( () => shipDialogueBox.Type("<color=red><b>-- WARNING --\nHULL BREACH DETECTED", delay: .01f, instantText: true) );
        s.AppendInterval( 1f );
        s.AppendCallback( () => shipDialogueBox.Type("<color=red>LANDNG GEAR FAILURE DETECTED.") );
        s.AppendInterval( 1f );
        s.AppendCallback( () => shipDialogueBox.Type("<color=red>SOMEOTHER FAILURE DETECTED.") );
        s.AppendInterval( 1f );
        s.AppendCallback( () => shipDialogueBox.Type("<color=red>AND ANOTHER FAILURE DETECTED.") );
        s.AppendInterval( 1f );
        s.AppendCallback( () => shipDialogueBox.Type("<color=red>SOMETHING SOMETHING FAILURE DETECTED.") );

        //RaposUtil.WaitSeconds(this, 7, GameManager.ResetScene );
    }

    private void OnDisable() 
    {
        turnOnInput.Disable();    
        shakeInput.Disable();
    }
}
