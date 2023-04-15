using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using DG.Tweening;
using RedBlueGames.Tools.TextTyper;

public class TrailerSceneCaller : MonoBehaviour
{
    [SerializeField] InputAction turnOnInput; // input para ligar
    [SerializeField] InputAction shakeInput;  // input para chamar o ScreenShake + Buyk Pulando + possíveis efeitos visuais
    [SerializeField] InputAction hideInput;

    [Space(10)]
    
    [SerializeField] [TextArea(minLines: 2, maxLines: 3)] List<string> shipDialogue;
    [SerializeField] List<string> trailerDialogueID;
    [SerializeField] List<string> warningTextsID;
    
    [Header("WWise")]
    //[SerializeField] AK.Wwise.Event screenLightUpAKEvent;
    [SerializeField] AK.Wwise.Event explosionAKEvent;
    [SerializeField] AK.Wwise.RTPC intensityAKEvent;

    [Header("References")]
    [SerializeField] PlayerCharacter player;
    [SerializeField] GameObject bedBuyk;
    [SerializeField] ShipDialogueBox shipDialogueBox;
    [SerializeField] GalaxyParallaxTester galaxyParallax;
    [SerializeField] ShipScreensOverlay shipScreens;
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
    [SerializeField] TextTyper shipTextTyper;

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
        if (intensityAKEvent != null)
            intensityAKEvent.SetGlobalValue(0);

        if (!AutoStart)
        {
            //shipScreens.gameObject.SetActive(false);
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
        sequence.AppendCallback( () => 
        {
            shipScreens.TurnOn();
        });
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
            //Debug.Log($"localizedText: {localizedText.isValid}, {localizedText.value}");

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
        shipScreens.Break();

        if (explosionAKEvent != null)
            explosionAKEvent.Post(gameObject);

        Camera cam = Camera.main;
        if (cam)
        {
            float t = .3f;
            cam.DOOrthoSize(1.75f, t);
            cam.transform.DORotate(Vector3.forward * 2f, t);
        }

        shipDialogueBox.transform.DOPunchScale(Vector3.one * .8f, duration: .3f);
        shipDialogueBox.transform.DOLocalRotate(Vector3.forward * 3, duration: .1f);


        // -- Chamada dos textos de perigo
        if (warningTextsID.Count < 2)
            return;

        if (shipTextTyper)
        {
            shipTextTyper.SetTypeSpeedValues(printDelaySetting: .01f, punctuationDelayMultiplier: 1f);
        }

        endtextS = DOTween.Sequence();

        (bool isValid, string value) warningText = LocalizationManager.GetShipText( warningTextsID[0] );
        endtextS.AppendCallback( () => shipDialogueBox.Type(warningText.value, delay: .01f, instantText: true) );
        
        for (int i = 1; i < warningTextsID.Count; i++)
        {
            endtextS.AppendInterval( 1.1f );

            (bool isValid, string value) localWarningText = LocalizationManager.GetShipText(warningTextsID[i]);
            endtextS.AppendCallback( () => shipDialogueBox.Type(localWarningText .value) );
        }
        //endtextS.AppendCallback( () => shipDialogueBox.Type("<color=#E32327>HULL BREACH DETECTED") );
        //endtextS.AppendInterval( 1f );
        //endtextS.AppendCallback( () => shipDialogueBox.Type("<color=#E32327>ELECTRICAL SYSTEM FAILURE DETECTED") );
        //endtextS.AppendInterval( 1f );
        //endtextS.AppendCallback( () => shipDialogueBox.Type("<color=#E32327>FUEL TANK FAILURE DETECTED") );
        //endtextS.AppendInterval( 1f );
        //endtextS.AppendCallback( () => shipDialogueBox.Type("<color=#E32327>BACK THRUSTERS FAILURE DETECTED") );
        //endtextS.AppendInterval( 1f );
        //endtextS.AppendCallback( () => shipDialogueBox.Type("<color=#E32327>LANDING GEAR FAILURE DETECTED") );
        //endtextS.AppendInterval( 1f );
        //endtextS.AppendCallback( () => shipDialogueBox.Type("<color=#E32327>SOMETHING-SOMETHING FAILURE DETECTED") );

        StartCoroutine( ScreenFadeOut() );        
    }

    private IEnumerator ScreenFadeOut()
    {
        float t1 = 1.0f;
        float t2 = 1.5f;
        float t3 = 3.55f;

        DOVirtual.Float
        (
            from: 0, to: 100, duration: t1 + t2 + t3,
            (f) =>  
            {
                //Debug.Log("f: " + f);
                if (intensityAKEvent != null)
                    intensityAKEvent.SetGlobalValue(f);
            }
        );

        yield return new WaitForSeconds(t1);

        if (introWarnings)
            introWarnings.CallInOrder();

        yield return new WaitForSeconds(t2);

        if (redCanvasGroup)
        {
            //redCanvasGroup.DOFade(1, duration: t)
            //    .OnComplete( () => {} );
            DOVirtual.Float
            (
                from: 0, to: 1, duration: t3,
                f => redCanvasGroup.alpha = redCanvasCurve.Evaluate(f)
            );
        }

        yield return new WaitForSeconds(t3);

        if (endtextS != null)
            endtextS.Kill();

        if (shipDialogueBox)
            shipDialogueBox.StopType();

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

        //shipScreens.SetTrigger("TurnOn");
        shipScreens.TurnOn();
    }

    public void CallNextScene()
    {
        if (SoundtrackManager.Instance)
            SoundtrackManager.Instance.Stop();

        GameManager.BlockCharacterInput = false;
        GameManager.GoToScene( "Assets/Scenes/World0TutorialScene.unity" );
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
