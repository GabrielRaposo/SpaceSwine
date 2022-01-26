using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveIcon : MonoBehaviour
{
    CanvasGroup canvasGroup;

    static SaveIcon Instance;

    private void Awake() 
    {
        if (Instance != null)
            return;

        Instance = this;       
    }

    private void OnEnable() 
    {
        canvasGroup = GetComponent<CanvasGroup>();    
    }

    private void Start() 
    {
        SetLocalVisibility(false);    
    }

    public static void Show () 
    { 
        if (!Instance)
            return;

        Instance.SetActiveTemp();
    }

    private void SetActiveTemp() // Por enquanto espera por um tempo arbitrário, depois tentar achar uma forma de fazer direito
    {
        StopAllCoroutines();

        SetLocalVisibility(true);

        StartCoroutine
        (
            RaposUtil.WaitSeconds(3, () => SetLocalVisibility(false))
        );
    }

    private void SetLocalVisibility (bool state) 
    { 
        gameObject.SetActive(state);
        canvasGroup.alpha = state ? 1 : 0;
    }
}
