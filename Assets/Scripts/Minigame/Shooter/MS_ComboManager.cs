using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MS_ComboManager : MonoBehaviour
{
    [Header("Values")]
    [SerializeField] float comboHoldDuration;

    [Header("References")]
    [SerializeField] Image fillImage;

    float t;
    static int Combo;

    CanvasGroup canvasGroup;

    static public MS_ComboManager Instance;

    private void Awake()
    {
        Instance = this;

        canvasGroup = GetComponent<CanvasGroup>();
    }

    void Start()
    {
        canvasGroup.alpha = 0;    
        Combo = 0;
        t = 0;

        UpdateDisplays ();
    }

    private void UpdateDisplays ()
    {
        if (fillImage)
            fillImage.fillAmount = t / comboHoldDuration;

        canvasGroup.alpha = (t == 0 || ComboMultiplier == 1f) ? 0 : 1;
    }

    private void Update()
    {
        if (t > 0)
        {
            t -= Time.deltaTime;
            
            if (t <= 0)
                Combo = 0;
        }
        else t = 0;
        
        UpdateDisplays ();
    }

    public void NotifyHit()
    {
        Combo++;
        t = comboHoldDuration;

        UpdateDisplays ();
    }

    public static float ComboMultiplier => 1 + (Combo > 3 ? .5f : 0f);
}
