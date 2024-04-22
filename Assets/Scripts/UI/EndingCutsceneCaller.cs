using DevLocker.Utils;
using RedBlueGames.Tools.TextTyper;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class EndingCutsceneCaller : MonoBehaviour
{   
    [SerializeField] float frameTransitionDuration;
    [SerializeField] List<CutsceneStoryData> cutsceneStoryDatas; 
    [SerializeField] SceneReference creditsSceneReference;

    [Header("References")]
    [SerializeField] TextMeshProUGUI mainTextDisplay;
    [SerializeField] TextTyper mainTextTyper;
    [SerializeField] Image imageDisplay;
    [SerializeField] Image proceedIcon;
    [SerializeField] CanvasGroup imageCanvasGroup;
    [SerializeField] EndingSoundtrackSystem endingSoundtrack;

    int frameIndex;
    int textIndex;
    bool typeCompletedTrigger;
    bool readingInput;

    PlayerInputActions inputActions;
    Sequence s;

    private void OnEnable()
    {
        inputActions = new PlayerInputActions();
        inputActions.Enable();

        inputActions.UI.Confirm.performed += (ctx) => 
        {
            if (!readingInput)
                return;
            
            ProceedInput();
        };
    }

    void Start()
    {
        // -- Desligar todos os outros componentes de UI  


        // -- Setups
        proceedIcon.enabled = false;
        mainTextDisplay.text = string.Empty;
        imageDisplay.sprite = null;
        imageCanvasGroup.alpha = 0;

        frameIndex = textIndex = 0;

        StartCoroutine ( Call() );
    }

    void ProceedInput()
    {
        StartCoroutine( Proceed() );
    }

    IEnumerator Call()
    {
        yield return new WaitForSeconds( 2f );
        yield return Proceed();
    }

    IEnumerator Proceed() 
    {
        readingInput = false;
        proceedIcon.enabled = false;

        mainTextDisplay.text = string.Empty;
        
        if (frameIndex >= cutsceneStoryDatas.Count)
        {
            yield return EndCutscene();
            yield break;
        }

        CutsceneStoryData cutsceneStoryData = cutsceneStoryDatas[frameIndex % cutsceneStoryDatas.Count];

        bool done = false;
        if (textIndex < 1) // -- Mostra o frame
        {
            // mostra imagem primeiro, depois escreve o texto
            imageCanvasGroup.alpha = 0;
            imageDisplay.sprite = cutsceneStoryData.frameSprite;

            if (cutsceneStoryData.soundtrackState != -1)
                endingSoundtrack.ChangeFocus(cutsceneStoryData.soundtrackState);

            done = false;
            s = DOTween.Sequence();
            s.Append( imageCanvasGroup.DOFade(1f, frameTransitionDuration) );
            s.OnComplete(() => done = true );

            yield return new WaitUntil( () => done );
        }

        yield return new WaitForSeconds(.2f);

        (bool isValid, string text) data = LocalizationManager.GetStoryText(cutsceneStoryData.textTags[textIndex % cutsceneStoryData.textTags.Count]);
        if (!data.isValid)
        {
            DebugDisplay.Log("Invalid text tag");
            yield break;
        }

        typeCompletedTrigger = false;
        mainTextTyper.TypeText(data.text);
        yield return new WaitUntil( () => typeCompletedTrigger );

        // -- Advance Indexes
        textIndex++;
        if (textIndex >= cutsceneStoryData.textTags.Count)
        {
            textIndex = 0;
            frameIndex++;
        }

        readingInput = true;
        proceedIcon.enabled = true;
    }

    IEnumerator EndCutscene()
    {
        yield return new WaitForSeconds(1f);
        //DebugDisplay.Log("Cutscene ended");

        s = DOTween.Sequence();
        s.Append( imageCanvasGroup.DOFade(0f, duration: 3f) );
        s.AppendCallback( () => { endingSoundtrack.enabled = false; } );
        s.AppendInterval(1.1f);

        s.OnComplete( () => SceneTransition.LoadScene(creditsSceneReference.ScenePath, SceneTransition.TransitionType.BlackFade) );
    }

    public void OnTypeCompleted()
    {
        typeCompletedTrigger = true;
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }
}

[System.Serializable]
public class CutsceneStoryData
{
    public Sprite frameSprite;
    public int soundtrackState = -1;
    public List<string> textTags;
}
