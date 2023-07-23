using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DevLocker.Utils;

public class AdventureLogDisplay : MonoBehaviour
{
    [SerializeField] Transform logsParent;
    [SerializeField] TextMeshProUGUI labelDisplay;
    [SerializeField] AdventureLogTab baseTab;

    [SerializeField] List<SceneReference> exceptionScenes;

    CanvasGroup canvasGroup;
    List<AdventureLogTab> tabs;

    private void Awake() 
    {
        canvasGroup = GetComponent<CanvasGroup>();    
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
        canvasGroup.alpha = 1;
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

        tabs.Add (tabScript);

        if (!IsOnExceptionScene())
            canvasGroup.alpha = 1;
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

        tabToRemove.StrikeAndVanish 
        (
            () => 
            {
                tabs.Remove(tabToRemove);
                if (tabs.Count < 1)
                    canvasGroup.alpha = 0;

                Destroy(tabToRemove.gameObject);
            }
        );
    }
}
