using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class CustomTimeCounter : MonoBehaviour
{
    [SerializeField] List<float> milestones;    
    [SerializeField] TextMeshProUGUI timerDisplay;
    [SerializeField] AK.Wwise.Event feedbackAKEvent;

    public UnityAction<int> OnTimeReached;

    float t;
    bool running;
    List<bool> milestonesReached;

    CanvasGroup canvasGroup;

    void Start()
    {
        if (milestones == null)
        {
            gameObject.SetActive(false);
            return;
        }

        milestonesReached = new List<bool>();

        for (int i = 0; i < milestones.Count; i++) 
            milestonesReached.Add(false);

        t = 0;
        UpdateDisplay();

        if (!Application.isEditor)
            canvasGroup.alpha = 0f;
    }

    private void UpdateDisplay()
    {
        timerDisplay.text = "Time: " + t.ToString("00.000");
    }

    public void Restart()
    {
        t = 0;
        UpdateDisplay();
        running = true;
    }

    public void Stop()
    {
        UpdateDisplay();
        running = false;
    }

    void Update()
    {
        if (!running)
            return;

        t += Time.deltaTime;
        UpdateDisplay();

        for (int i = 0; i < milestones.Count; i++)
        {
            if (t < milestones[i])
                return;

            if (milestonesReached[i])
                continue;

            milestonesReached[i] = true;
            OnTimeReached.Invoke(i);

            if (feedbackAKEvent != null && Application.isEditor)
                feedbackAKEvent.Post(gameObject);

            Debug.Log($"Milestone [{i}] reached.");
        }
    }
}
