using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    public static void Show (Color color) 
    { 
        if (!Instance)
            return;

        Instance.SetActiveTemp(color);
    }

    private void SetActiveTemp (Color color) // Por enquanto espera por um tempo arbitrário, depois tentar achar uma forma de fazer direito
    {
        StopAllCoroutines();

        Image image = GetComponentInChildren<Image>();
        if (image)
            image.color = color;

        SetLocalVisibility(true);

        StartCoroutine
        (
            RaposUtil.WaitSecondsRealtime(1, () => SetLocalVisibility(false))
        );
    }

    private void SetLocalVisibility (bool state) 
    { 
        gameObject.SetActive(state);
        canvasGroup.alpha = state ? 1 : 0;

        //Debug.Log("SetLocalVisibility: " + state);
    }
}
