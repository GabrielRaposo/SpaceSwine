﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using DG.Tweening;

public class TrailerSceneCaller : MonoBehaviour
{
    [SerializeField] InputAction turnOnInput; // input para ligar
    [SerializeField] InputAction shakeInput;  // input para chamar o ScreenShake + Buyk Pulando + possíveis efeitos visuais
    [SerializeField] InputAction hideInput;

    [Space(10)]
    
    [SerializeField] [TextArea(minLines: 2, maxLines: 3)] List<string> shipDialogue;
    [SerializeField] List<string> trailerDialogueID;
    
    [Header("References")]
    [SerializeField] PlayerCharacter player;
    [SerializeField] GameObject bedBuyk;
    [SerializeField] ShipDialogueBox shipDialogueBox;
    [SerializeField] GalaxyParallaxTester galaxyParallax;
    [SerializeField] Animator shipScreens;
    [SerializeField] ScreenShakeCaller screenShake;
    [SerializeField] GameObject relaxingBuyk;
    [SerializeField] GameObject startledBuyk;
    [SerializeField] ParticleSystem smokePS;
    [SerializeField] GameObject redLight;
    [SerializeField] CanvasGroup redCanvasGroup;
    [SerializeField] Image blackImage;
    [SerializeField] ShowIntroWarnings introWarnings;
    [SerializeField] HoldToSkipScene holdToSkipScene;
    [SerializeField] AnimationCurve redCanvasCurve;

    Sequence sequence;
    Sequence endtextS;

    public static bool AutoStart = false;

    #if UNITY_EDITOR
    private void OnEnable() 
    {
        turnOnInput.performed += TurnOn;
        shakeInput.performed += Shake;
        hideInput.performed += Hide;

        turnOnInput.Enable();    
        shakeInput.Enable();
        hideInput.Enable();
    }
    #endif

    private void Start() 
    {
        if (!AutoStart)
        {
            shipScreens.gameObject.SetActive(false);
            if (galaxyParallax)
                galaxyParallax.enabled = false;
            if (holdToSkipScene)
                holdToSkipScene.enabled = false;

            return;
        }

        AutoStart = false;

        player.gameObject.SetActive(false);
        if (bedBuyk)
            bedBuyk.SetActive(true);
        if (redCanvasGroup)
            redCanvasGroup.alpha = 0;
        if (blackImage)
            blackImage.enabled = false;

        TurnOn( new InputAction.CallbackContext() );        
    }

    private void TurnOn (InputAction.CallbackContext ctx)
    {
        GameManager.BlockCharacterInput = true;
        if (holdToSkipScene)
                holdToSkipScene.enabled = true;

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
        sequence.AppendInterval( t1 + .3f);

        // -- Leitura dos diálogos
        for (int i = 0; i < trailerDialogueID.Count; i++)
        {
            string s = shipDialogue[i % shipDialogue.Count];
            (bool isValid, string value) localizedText = LocalizationManager.GetShipText(trailerDialogueID[i]);
            if (localizedText.isValid)
            {
                s = localizedText.value;
            }
            Debug.Log($"localizedText: {localizedText.isValid}, {localizedText.value}");

            int local = i;
            sequence.AppendCallback
            (
                () => { 
                    shipDialogueBox.Type(s); 

                    if (local == shipDialogue.Count - 1)
                    {
                        RaposUtil.WaitSeconds( this, 2, () => { 
                            Shake (new InputAction.CallbackContext());
                        });
                    }
                }
            );

            float t2 = 3.0f; 
            t2 = CustomDurations(i);
            t2 += s.Length * .015f;
            sequence.AppendInterval(t2);
        }

        sequence.AppendCallback( () => shipDialogueBox.Type(" ") );
        sequence.AppendCallback( () => shipDialogueBox.SetShown(false) );
    }

    float CustomDurations(int i)
    {
        switch(i)
        {
            default:
            case 0:
                return 2.5f;

            case 1:
            case 2:
                return 4f;

            case 3:
                return 2f;

            case 4:
                return 10.0f;
        }
    }

    private void Shake (InputAction.CallbackContext ctx)
    {
        if (sequence != null)
            sequence.Kill();

        if (SoundtrackManager.Instance)
            SoundtrackManager.Instance.Stop();
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

        endtextS = DOTween.Sequence();
        endtextS.AppendCallback( () => shipDialogueBox.Type("<color=#E32327><b>-- WARNING --\nFRONTAL COLLISION ", delay: .01f, instantText: true) );
        endtextS.AppendInterval( 1f );
        endtextS.AppendCallback( () => shipDialogueBox.Type("<color=#E32327>HULL BREACH DETECTED") );
        endtextS.AppendInterval( 1f );
        endtextS.AppendCallback( () => shipDialogueBox.Type("<color=#E32327>ELECTRICAL SYSTEM FAILURE DETECTED") );
        endtextS.AppendInterval( 1f );
        endtextS.AppendCallback( () => shipDialogueBox.Type("<color=#E32327>FUEL TANK FAILURE DETECTED") );
        endtextS.AppendInterval( 1f );
        endtextS.AppendCallback( () => shipDialogueBox.Type("<color=#E32327>BACK THRUSTERS FAILURE DETECTED") );
        endtextS.AppendInterval( 1f );
        endtextS.AppendCallback( () => shipDialogueBox.Type("<color=#E32327>LANDING GEAR FAILURE DETECTED") );
        endtextS.AppendInterval( 1f );
        endtextS.AppendCallback( () => shipDialogueBox.Type("<color=#E32327>SOMETHING-SOMETHING FAILURE DETECTED") );

        StartCoroutine( ScreenFadeOut() );        
    }

    private IEnumerator ScreenFadeOut()
    {
        yield return new WaitForSeconds(1.0f);

        if (introWarnings)
            introWarnings.CallInOrder();

        yield return new WaitForSeconds(1.5f);

        float t = 3.55f;
        if (redCanvasGroup)
        {
            //redCanvasGroup.DOFade(1, duration: t)
            //    .OnComplete( () => {} );
            DOVirtual.Float
            (
                from: 0, to: 1, duration: t,
                f => redCanvasGroup.alpha = redCanvasCurve.Evaluate(f)
            );
        }

        yield return new WaitForSeconds(t);

        if (blackImage)
            blackImage.enabled = true;

        if (endtextS != null)
            endtextS.Kill();

        yield return new WaitForSeconds(2);

        CallNextScene();
    }

    private void Hide(InputAction.CallbackContext ctx)
    {
        if (player)
            player.SetHiddenState(true);

        shipScreens.SetTrigger("TurnOn");
    }

    public void CallNextScene()
    {
        if (SoundtrackManager.Instance)
            SoundtrackManager.Instance.Stop();

        GameManager.BlockCharacterInput = false;
        GameManager.GoToScene( BuildIndex.World0Exploration );
    }

    #if UNITY_EDITOR
    private void OnDisable() 
    {
        turnOnInput.Disable();    
        shakeInput.Disable();
        hideInput.Disable();
    }
    #endif
}
