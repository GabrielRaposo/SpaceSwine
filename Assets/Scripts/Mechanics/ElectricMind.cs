using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricMind : MonoBehaviour
{
    [SerializeField] bool loop;
    [SerializeField] bool autoUpdate;
    [SerializeField] ElectricLine electricLine;
    [SerializeField] Transform linesGroup;

    ElectricBall[] electricballs;

    private void Start() 
    {
        if (electricLine == null)
        {
            gameObject.SetActive(false);
            return;
        }

        electricballs = GetComponentsInChildren<ElectricBall>();
        if (electricballs.Length < 2)
        {
            gameObject.SetActive(false);
            return;    
        }

        SetupChain();
    }

    private void SetupChain()
    {
        int max = electricballs.Length - (loop ? 0 : 1); 
        for (int i = 0; i < max; i++) 
        {
            GameObject lineObject = Instantiate (electricLine.gameObject, linesGroup);
            
            ElectricLine lineScript = lineObject.GetComponent<ElectricLine>();
            if (lineScript) lineScript.Setup(autoUpdate, BallAt(i+1).transform, BallAt(i).transform);
        }

        electricLine.gameObject.SetActive(false);
    }

    ElectricBall BallAt (int index)
    {
        return electricballs[index % electricballs.Length];
    }
}
