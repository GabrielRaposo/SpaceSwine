using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DevLocker.Utils;
using DG.Tweening;

public class AdventureLogDisplay : MonoBehaviour
{
    [SerializeField] float fadeDuration;

    [Header("References")]
    [SerializeField] Transform logsParent;
    [SerializeField] TextMeshProUGUI labelDisplay;
    [SerializeField] AdventureLogTab baseTab;

    [Header("WWise")]
    [SerializeField] AK.Wwise.Event addAKEvent;
    [SerializeField] AK.Wwise.Event removeAKEvent;

    [SerializeField] List<SceneReference> exceptionScenes;

    CanvasGroup canvasGroup;
    List<AdventureLogTab> tabs;

    Sequence fadeSequence;

    public static List<AdventureLogScriptableObject> UpdatedList;

    private void Awake() 
    {
        canvasGroup = GetComponent<CanvasGroup>();
        UpdatedList = new List<AdventureLogScriptableObject>();
    }

    private void Start() 
    {
        canvasGroup.alpha = 0;

        if (IsOnExceptionScene())
            return;

        AdventureLogManager manager = AdventureLogManager.Instance;
        if (!manager)
            return;

        manager.CallForUpdate (this);   
    }

    private void SetFadeAnimation (bool value)
    {
        if (!canvasGroup)
            return;

        if (fadeSequence != null)
            fadeSequence.Kill();

        fadeSequence = DOTween.Sequence();
        fadeSequence.Append 
        (
            canvasGroup.DOFade(value ? 1f : 0f, fadeDuration)
        );

        fadeSequence.SetUpdate( isIndependentUpdate: true );
    }

    private bool IsOnExceptionScene()
    {
        if (exceptionScenes.Count > 0)
        {
            foreach (SceneReference r in exceptionScenes)
            {
                if (GameManager.IsOnScene(r.ScenePath))
                {
                    //DebugDisplay.Call ("Is on exception scene!");
                    return true;
                }
            }
        }

        return false;
    }

    public void Setup ( List <AdventureLogScriptableObject> logList )
    {
        if (!baseTab)
        {
            gameObject.SetActive(false);
            return;
        }

        // -- Por enquanto só limpa a lista de tabs anterior sempre que ocorre uma atualização
        if (tabs != null)
        {
            foreach (AdventureLogTab t in tabs)
                Destroy (t.gameObject);
        }

        tabs = new List<AdventureLogTab>();
        baseTab.SetActiveState(false);

        UpdatedList = logList;

        if (logList.Count < 1)
        {
            canvasGroup.alpha = 0;
            return;
        }

        // -- Monta a nova lista
        for (int i = 0; i < logList.Count; i++)
        {
            GameObject newTab = Instantiate (baseTab.gameObject, logsParent);
            AdventureLogTab tabScript = newTab.GetComponent<AdventureLogTab>();
            
            tabScript.Setup (logList[i]);

            tabs.Add(tabScript);
        }

        //canvasGroup.alpha = 1;
        SetFadeAnimation(true);
    }

    public void AddToList (AdventureLogScriptableObject log)
    {
        if (tabs.Count > 0)
        {
            foreach (AdventureLogTab tab in tabs)
            {
                if (tab.data == log)
                {
                    return;
                }
            }
        }

        GameObject newTab = Instantiate (baseTab.gameObject, logsParent);
        AdventureLogTab tabScript = newTab.GetComponent<AdventureLogTab>();
            
        tabScript.Setup (log);
        tabScript.SlideInAndStay( ()=>{} );

        tabs.Add (tabScript);
        if (UpdatedList != null)
            UpdatedList.Add(log);

        if (addAKEvent != null)
            addAKEvent.Post(gameObject);

        if (!IsOnExceptionScene())
            //canvasGroup.alpha = 1;
            SetFadeAnimation(true);
    }

    public void RemoveFromList (AdventureLogScriptableObject log)
    {
        AdventureLogTab tabToRemove = null; 
        if (tabs.Count > 0)
        {
            foreach (AdventureLogTab tab in tabs)
            {
                if (tab.data == log)
                {
                    tabToRemove = tab;
                    continue;
                }
            }
        }

        if (tabToRemove == null)
            return;

        if (removeAKEvent != null)
            removeAKEvent.Post(gameObject);

        tabToRemove.StrikeAndVanish 
        (
            () => 
            {
                tabs.Remove(tabToRemove);
                if (UpdatedList.Contains(log))
                    UpdatedList.Remove(log);

                if (tabs.Count < 1)
                {
                    //canvasGroup.alpha = 0;
                    SetFadeAnimation(false);
                }

                Destroy(tabToRemove.gameObject);
            }
        );
    }
}
