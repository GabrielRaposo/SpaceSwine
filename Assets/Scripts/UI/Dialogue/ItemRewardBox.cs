using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[RequireComponent(typeof(CanvasGroup))]
public class ItemRewardBox : MonoBehaviour
{
    [Header("Values")]
    [SerializeField] float showDuration;

    [Header("References")]
    [SerializeField] AK.Wwise.Event rewardAKEvent;
    [SerializeField] Image iconDisplay;
    [SerializeField] TextMeshProUGUI rewardTextDisplay;
    [SerializeField] List<string> rewardTextIDs;
    [SerializeField] List<Sprite> rewardIcons;

    CanvasGroup canvasGroup;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
    }

    private void Start()
    {
        this.WaitSeconds( 4f, () => Call(1, null) );
    }

    public void Call (int reward, PlayerInteractor interactor)
    {
        GameManager.OnDialogue = true;
        if (interactor)
        {
            PlatformerCharacter platformerCharacter = interactor.GetComponent<PlatformerCharacter>();
            platformerCharacter?.KillInputs();
        }

        reward -= 1;

        if (rewardTextIDs.Count < 1 || rewardIcons.Count < 1)
            return;

        Sprite icon = rewardIcons[reward % rewardIcons.Count];
        string textID = rewardTextIDs[reward % rewardIcons.Count];

        {
            iconDisplay.sprite = icon;
            rewardTextDisplay.text = LocalizationManager.GetUiText(textID, $"You got the Relic #{reward+1}");
            if (rewardAKEvent != null)
                rewardAKEvent.Post(gameObject);
        }

        float duration = .2f;

        Sequence s = DOTween.Sequence();
        s.Append( canvasGroup.DOFade(1f, duration) );
        s.Join ( GetComponent<RectTransform>().DOPunchScale(Vector3.one * .2f, duration, vibrato: 0, elasticity: 0) );
        s.AppendInterval(showDuration);
        s.Append( canvasGroup.DOFade(0f, duration).SetEase(Ease.Linear) );
        s.OnComplete( () => 
            { 
                BlockInputs(false);
                // After reward action 
            }
        );
    }

    private void BlockInputs (bool value)
    {
        GameManager.OnDialogue = value;

    }
}
