using Shooter;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MS_StageTimer : MonoBehaviour
{
    [SerializeField] int startingTime;
    [SerializeField] Color baseColor;
    [SerializeField] Color dangerColor;
    [SerializeField] Color sessionColor;

    [SerializeField] int blinkThreshold;
    [SerializeField] float sessionBlinkDuration;

    [Header("References")]
    [SerializeField] MS_Player player;
    [SerializeField] TextMeshProUGUI timerDisplay;
    [SerializeField] Image clockImage;

    bool dangerBlinking;

    static float Time;
    static bool Running;
    static public MS_StageTimer Instance;

    CanvasGroup canvasGroup;

    private void Awake()
    {
        Instance = this;
        canvasGroup = GetComponent<CanvasGroup>();
    }

    void Start()
    {
        Time = startingTime;
        UpdateDisplay();
        Running = false;

        canvasGroup.alpha = 0;

        if (!player)
            return;
            
        player.OnFirstMove += () => 
        { 
            canvasGroup.alpha = 1;
            Running = true; 
        };

        SetColors (baseColor);
    }

    void Update()
    {
        if (!Running)
            return;

        if (MS_SessionManager.OnSessionTransition)
            return;

        if (Time > 0)
        {
            Time -= UnityEngine.Time.deltaTime;
            UpdateDisplay();

            if (Time < blinkThreshold)
            {
                if (!dangerBlinking)
                {
                    dangerBlinking = true;
                    StartCoroutine( DangerBlinkingColors() );
                }
            }
            else
            {
                if (dangerBlinking)
                {
                    dangerBlinking = false;
                    StopAllCoroutines();
                }
            }

            return;
        }

        Time = 0;
        UpdateDisplay();

        if (player)
            player.Die();

        Running = false;
        StopAllCoroutines();
        SetColors (dangerColor);
    }

    private void UpdateDisplay()
    {
        if (!timerDisplay)
            return;

        timerDisplay.text = Time.ToString("00");
    }

    public static void AddTime (int value)
    {
        Time += value;
        if (Time > 30)
            Time = 30;

        if (Instance)
            Instance.UpdateDisplay();
    }

    private void SetColors (Color color)
    {
        timerDisplay.color = clockImage.color = color;
    }

    public static void SetSessionBlink()
    {
        if (Instance)
        {
            Instance.dangerBlinking = false;
            Instance.StopAllCoroutines();
            Instance.StartCoroutine (Instance.SessionBlinkingColors());
        }
    }

    IEnumerator DangerBlinkingColors()
    {
        float blinkDelay = .1f;
        while (true)
        {
            SetColors (dangerColor);
            yield return new WaitForSeconds(blinkDelay);

            SetColors (baseColor);
            yield return new WaitForSeconds(blinkDelay);
        }
    }

    IEnumerator SessionBlinkingColors()
    {
        float t = 0;
        float blinkDelay = .15f;

        while (t < sessionBlinkDuration)
        {
            SetColors (sessionColor);
            yield return new WaitForSeconds(blinkDelay);
            t += blinkDelay;

            SetColors (baseColor);
            yield return new WaitForSeconds(blinkDelay);
            t += blinkDelay;
        }
    }
}
