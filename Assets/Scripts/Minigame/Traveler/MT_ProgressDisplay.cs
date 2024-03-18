using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MT_ProgressDisplay : MonoBehaviour
{
    [SerializeField] TextMeshPro scoreDisplay;

    static public MT_ProgressDisplay Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void UpdateScore (int value)
    {
        if (!scoreDisplay)
            return;

        scoreDisplay.text = "SCORE " + value.ToString("000");
    }
}
