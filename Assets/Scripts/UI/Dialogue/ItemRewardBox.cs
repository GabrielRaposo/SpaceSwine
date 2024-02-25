using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Events;

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
    UnityAction AfterRewardAction;

    public static ItemRewardBox Instance;

    void Awake()
    {
        if (Instance == null) 
        {
            Instance = this;
        }

        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
    }

    public void Call (int reward, PlayerInteractor interactor, UnityAction AfterRewardAction)
    {
        this.AfterRewardAction = AfterRewardAction;

        BlockInputs(true, interactor);

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
        s.AppendInterval(1f);
        s.OnComplete( () => 
            { 
                if (AfterRewardAction == null)
                {
                    BlockInputs(false, interactor);
                    return;
                }

                AfterRewardAction.Invoke();
            }
        );
    }

    private void BlockInputs (bool value, PlayerInteractor interactor)
    {
        GameManager.OnDialogue = value;
        DialogueSystem.BlockInputs = value;
        GameManager.BlockCharacterInput = value;

        if (interactor)
        {
            PlatformerCharacter platformerCharacter = interactor.GetComponent<PlatformerCharacter>();
            if (value)
            {
                platformerCharacter?.KillInputs();
            }
        }
    }
}
