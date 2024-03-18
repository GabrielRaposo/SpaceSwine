using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class TransitionSafetyToDanger : MonoBehaviour
{
    [SerializeField] float iconSlideX;
    [SerializeField] float fadeDuration;
    [SerializeField] float slideDuration;
    [SerializeField] float hold1Duration;
    [SerializeField] float hold2Duration;
    [SerializeField] float musicFadeOut;
    [SerializeField] float musicFadeIn;

    [Header("References")]
    [SerializeField] CanvasGroup mainCanvasGroup;    
    [SerializeField] CanvasGroup assetsGroup;
    [SerializeField] Image fillImage;
    [SerializeField] ImageMaterialInterface materialInterface;

    [Header("Icons")]
    [SerializeField] RectTransform iconsAnchor;
    [SerializeField] Animator safetyIconAnimator;
    [SerializeField] Animator dangerIconAnimator;

    [Header("Stripes Bottom")]
    [SerializeField] CanvasGroup stripesGroup;
    [SerializeField] CanvasGroup safetyStripes;
    [SerializeField] CanvasGroup dangerStripes;

    [Header("Audio")]
    [SerializeField] AK.Wwise.Event twistedTransitionAKEvent;
    [SerializeField] AK.Wwise.Event safetyToDangerAKEvent;
    [SerializeField] AK.Wwise.Event dangerToSafetyAKEvent;

    //[Header("Playlists - TEMP: sistematizar depois")]
    //[SerializeField] PlaylistScriptableObject safetyPlaylist;
    //[SerializeField] PlaylistScriptableObject dangerPlaylist;

    Sequence mainSequence;
    private static readonly int Play = Animator.StringToHash("Play");

    public static bool OnTransition { get; private set; }

    void Start()
    {
        SetState( danger: true );

        mainCanvasGroup.alpha = 0;
        gameObject.SetActive(false);

        OnTransition = false;
    }

    private void SetState (bool danger)
    {
        iconsAnchor.anchoredPosition = new Vector2(danger ? iconSlideX : 0, iconsAnchor.anchoredPosition.y);

        safetyStripes.alpha =  danger ? 0 : 1;
        dangerStripes.alpha =  danger ? 1 : 0;
    }

    public void CallTransition (string path, bool safetyToDanger)
    {
        mainCanvasGroup.alpha = 0;
        assetsGroup.alpha = 0;
        SetState( danger: !safetyToDanger );
        gameObject.SetActive(true);

        StartCoroutine( SafetyToDangerTransition(path, safetyToDanger) );
    }

    private IEnumerator SafetyToDangerTransition(string path, bool safetyToDanger)
    {
        OnTransition = true;
        GameManager.BlockCharacterInput = true;

        SceneTransition.OnTransition = true;
        PlayerTransitionState.BlockSpawn = true;
        RoundsManager.BlockSpawn = true;

        PlayerTransitionState.EnterState = PlayerTransitionState.State.OutOfPortal;

        bool done = false;

        if (mainSequence != null)
            mainSequence.Kill();

        SoundtrackManager soundtrackManager = SoundtrackManager.Instance;
        if (soundtrackManager)
            soundtrackManager.FadeOutAndPause(musicFadeOut);

        fillImage.enabled = false;

        // -- Transição de vórtice
        mainCanvasGroup.alpha = 1.0f;
        materialInterface.animator.SetTrigger(Play);
        if (twistedTransitionAKEvent != null)
            twistedTransitionAKEvent.Post(gameObject);
        materialInterface.onAnimationEnd = () =>
        {
            done = true;
        };

        yield return new WaitUntil( () => done );
        done = false;

        fillImage.enabled = true;
        DOTween.Clear();

        // -- Inicia carregamento de cena
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(path);
        while (!asyncOperation.isDone)
        {
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForEndOfFrame();
        asyncOperation.allowSceneActivation = false;

        // -- Fade-in dos ícones na tela
        mainSequence = DOTween.Sequence();
        mainSequence.Append( assetsGroup.DOFade(.75f, fadeDuration) );
        mainSequence.OnComplete( () => done = true );
        mainSequence.SetUpdate(isIndependentUpdate: true);

        if (safetyToDanger)
        {
            if (safetyToDangerAKEvent != null)
                safetyToDangerAKEvent.Post(gameObject);
        }
        else
        {
            if (dangerToSafetyAKEvent != null)
                dangerToSafetyAKEvent.Post(gameObject);
        }

        yield return new WaitUntil( () => done );
        done = false;

        // -- Faz animação de Safety -> Danger
        mainSequence = DOTween.Sequence();
        mainSequence.AppendInterval(hold1Duration);
        mainSequence.AppendCallback( () => 
        {
            if (safetyToDanger)
                dangerIconAnimator.SetTrigger("TipRight"); 
            else
                safetyIconAnimator.SetTrigger("TipLeft");
        });

        float startX = safetyToDanger ? 0 : iconSlideX;
        float endX   = safetyToDanger ? iconSlideX : 0;
        mainSequence.Append ( 
            DOVirtual.Float(startX, endX, slideDuration, (f) => iconsAnchor.MoveX(f) )
                .SetEase(Ease.OutCirc)
        );
        mainSequence.Join( SwitchStripes (safetyToDanger) );
        mainSequence.AppendCallback( () => 
        {
            if (soundtrackManager)
                soundtrackManager.ResumeAndFadeIn(musicFadeIn);
        } );
        mainSequence.AppendInterval(hold2Duration);
        mainSequence.OnComplete( () => done = true );
        mainSequence.SetUpdate(isIndependentUpdate: true);

        yield return new WaitUntil( () => done );
        done = false;

        // -- Libera apresentação da cena
        asyncOperation.allowSceneActivation = true;
        yield return new WaitForEndOfFrame();

        // -- Fade-out da tela de transição
        //DOTween.Clear();
        mainSequence = DOTween.Sequence();
        mainSequence.Append( mainCanvasGroup.DOFade(0, slideDuration) );
        mainSequence.OnComplete( () => done = true );
        mainSequence.SetUpdate(isIndependentUpdate: true);
        
        yield return new WaitUntil( () => done );

        materialInterface.animator.SetTrigger("Reset");
        
        yield return new WaitForSecondsRealtime(.1f);

        fillImage.enabled = false;

        RoundsManager.BlockSpawn = false;
        PlayerTransitionState.BlockSpawn = false;
        gameObject.SetActive(false);
        SceneTransition.OnTransition = false;

        OnTransition = false;
        GameManager.BlockCharacterInput = false;
        //Debug.Log(">>> OUT");
    }

    private Sequence SwitchStripes(bool toDanger)
    {
        CanvasGroup first  = toDanger ? safetyStripes : dangerStripes;
        CanvasGroup second = toDanger ? dangerStripes : safetyStripes;

        first.alpha = 1;
        second.alpha = 0;
        
        Sequence s = DOTween.Sequence();

        s.Append( stripesGroup.DOFade(0, slideDuration/2f) );
        s.AppendCallback( () => {
            first.alpha  = 0;
            second.alpha = 1;
        } );
        s.Append( stripesGroup.DOFade(1, slideDuration/2f) );
        
        return s;
    }
}
