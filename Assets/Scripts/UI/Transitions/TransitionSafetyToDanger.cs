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

    [Header("References")]
    [SerializeField] CanvasGroup mainCanvasGroup;    
    [SerializeField] CanvasGroup assetsGroup;
    [SerializeField] Image fillImage;

    [Header("Icons")]
    [SerializeField] RectTransform iconsAnchor;
    [SerializeField] Animator safetyIconAnimator;
    [SerializeField] Animator dangerIconAnimator;

    [Header("Stripes")]
    [SerializeField] CanvasGroup stripesGroup;
    [SerializeField] Image safetyStripe;
    [SerializeField] Image dangerStripe;

    Sequence mainSequence;

    void Start()
    {
        SetState( danger: true );

        mainCanvasGroup.alpha = 0;
        gameObject.SetActive(false);
    }

    private void SetState (bool danger)
    {
        iconsAnchor.anchoredPosition = new Vector2(danger ? iconSlideX : 0, iconsAnchor.anchoredPosition.y);

        safetyStripe.color =  danger ? new Color(1,1,1,0) : Color.white;
        dangerStripe.color = !danger ? new Color(1,1,1,0) : Color.white;
    }

    public void CallTransition (int index, bool safetyToDanger)
    {
        mainCanvasGroup.alpha = 0;
        assetsGroup.alpha = 0;
        SetState( danger: !safetyToDanger );
        gameObject.SetActive(true);

        StartCoroutine( SafetyToDangerTransition(index, safetyToDanger) );
    }

    private IEnumerator SafetyToDangerTransition(int index, bool safetyToDanger)
    {
        SceneTransition.OnTransition = true;
        bool done = false;

        SoundtrackManager soundtrackManager = SoundtrackManager.Instance;
        if (soundtrackManager)
            soundtrackManager.FadeOutMusic(2.0f);

        if (mainSequence != null)
            mainSequence.Kill();

        // -- Fade-in do fundo
        mainSequence = DOTween.Sequence();
        mainSequence.Append( mainCanvasGroup.DOFade(1, fadeDuration) );
        mainSequence.OnComplete( () => done = true );
        mainSequence.SetUpdate(isIndependentUpdate: true);

        yield return new WaitUntil( () => done );
        done = false;

        // -- Inicia carregamento de cena
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(index);
        while (!asyncOperation.isDone)
        {
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForEndOfFrame();
        asyncOperation.allowSceneActivation = false;

        // -- Fade-in dos ícones na tela
        mainSequence = DOTween.Sequence();
        mainSequence.Append( assetsGroup.DOFade(1, fadeDuration) );
        mainSequence.OnComplete( () => done = true );
        mainSequence.SetUpdate(isIndependentUpdate: true);

        yield return new WaitUntil( () => done );
        done = false;

        // -- Faz animação de Safety -> Danger
        mainSequence = DOTween.Sequence();
        mainSequence.AppendInterval(1.0f);
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
                soundtrackManager.SkipTrack(1);            
        } );
        mainSequence.AppendInterval(2.0f);
        mainSequence.OnComplete( () => done = true );
        mainSequence.SetUpdate(isIndependentUpdate: true);

        yield return new WaitUntil( () => done );
        done = false;

        // -- Libera apresentação da cena
        asyncOperation.allowSceneActivation = true;
        DOTween.KillAll();
        yield return new WaitForEndOfFrame();


        // -- Fade-out da tela de transição
        mainSequence = DOTween.Sequence();
        mainSequence.Append( mainCanvasGroup.DOFade(0, slideDuration) );
        mainSequence.OnComplete( () => done = true );
        mainSequence.SetUpdate(isIndependentUpdate: true);
        
        yield return new WaitUntil( () => done );


        gameObject.SetActive(false);
        SceneTransition.OnTransition = false;
    }

    private Sequence SwitchStripes(bool toDanger)
    {
        Image first  = toDanger ? safetyStripe : dangerStripe;
        Image second = toDanger ? dangerStripe : safetyStripe;

        first.color = Color.white;
        second.color = new Color(1,1,1,0);
        
        Sequence s = DOTween.Sequence();

        s.Append( stripesGroup.DOFade(0, slideDuration/2f) );
        s.AppendCallback( () => {
            first.color  = new Color(1,1,1,0);
            second.color = Color.white;
        } );
        s.Append( stripesGroup.DOFade(1, slideDuration/2f) );
        
        return s;
    }
}
