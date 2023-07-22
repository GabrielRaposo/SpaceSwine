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

        // -- Esconde o Display se estiver em uma das cenas de exceção
        if (exceptionScenes.Count > 0)
        {
            foreach (SceneReference r in exceptionScenes)
            {
                if (GameManager.IsOnScene(r.ScenePath))
                {
                    DebugDisplay.Call("Is on exception scene!");
                    return;
                }
            }
        }

        AdventureLogManager manager = AdventureLogManager.Instance;
        if (!manager)
            return;

        manager.CallForUpdate (this);   
    }

    public void Setup ( List <string> logTexts )
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

        if (logTexts.Count < 1)
        {
            canvasGroup.alpha = 0;
            return;
        }

        // -- Monta a nova lista
        for (int i = 0; i < logTexts.Count; i++)
        {
            GameObject newTab = Instantiate (baseTab.gameObject, logsParent);
            AdventureLogTab tabScript = newTab.GetComponent<AdventureLogTab>();
            
            tabScript.Setup(logTexts[i]);

            tabs.Add(tabScript);
        }
        canvasGroup.alpha = 1;
    }

}
