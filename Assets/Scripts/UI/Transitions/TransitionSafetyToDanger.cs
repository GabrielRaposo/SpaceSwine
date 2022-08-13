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
    [SerializeField] ImageMaterialInterface materialInterface;

    [Header("Icons")]
    [SerializeField] RectTransform iconsAnchor;
    [SerializeField] Animator safetyIconAnimator;
    [SerializeField] Animator dangerIconAnimator;

    [Header("Stripes Bottom")]
    [SerializeField] CanvasGroup stripesGroup;
    [SerializeField] CanvasGroup safetyStripes;
    [SerializeField] CanvasGroup dangerStripes;

    [Header("Playlists - TEMP: sistematizar depois")]
    [SerializeField] PlaylistScriptableObject safetyPlaylist;
    [SerializeField] PlaylistScriptableObject dangerPlaylist;

    Sequence mainSequence;
    private static readonly int Play = Animator.StringToHash("Play");

    void Start()
    {
        SetState( danger: true );

        mainCanvasGroup.alpha = 0;
        gameObject.SetActive(false);
    }

    private void SetState (bool danger)
    {
        iconsAnchor.anchoredPosition = new Vector2(danger ? iconSlideX : 0, iconsAnchor.anchoredPosition.y);

        safetyStripes.alpha =  danger ? 0 : 1;
        dangerStripes.alpha =  danger ? 1 : 0;
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
        RoundsManager.BlockSpawn = true;
        SetPlaylistOnStart.Block = true;
        bool done = false;

        if (mainSequence != null)
            mainSequence.Kill();

        SoundtrackManager soundtrackManager = SoundtrackManager.Instance;
        if (soundtrackManager)
            soundtrackManager.FadeOutMusic(2.0f);

        fillImage.enabled = false;

        // -- Fade-in do fundo
        mainCanvasGroup.alpha = 1.0f;
        materialInterface.animator.SetTrigger(Play);
        materialInterface.onAnimationEnd = () =>
        {
            done = true;
        };

        yield return new WaitUntil( () => done );
        done = false;

        fillImage.enabled = true;

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
            {
                soundtrackManager.SetPlaylist(safetyToDanger ? dangerPlaylist : safetyPlaylist );
            }
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

        fillImage.enabled = false;

        RoundsManager.BlockSpawn = false;
        gameObject.SetActive(false);
        SceneTransition.OnTransition = false;
        SetPlaylistOnStart.Block = false; 
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
