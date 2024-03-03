using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MS_ProgressDisplay : MonoBehaviour
{
    [SerializeField] TextMeshPro scoreDisplay;
    [SerializeField] TextMeshPro stageDisplay;

    public static MS_ProgressDisplay Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void UpdateScore (int score)
    {
        if (!scoreDisplay)
            return;

        scoreDisplay.text = "SCORE " + score.ToString("000"); 
    }

    public void UpdateStage (int stage)
    {
        if (!stageDisplay)
            return;

        stageDisplay.text = "STAGE " + stage.ToString("00");
    }
}
