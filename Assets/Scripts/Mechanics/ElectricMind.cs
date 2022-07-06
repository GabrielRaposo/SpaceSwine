using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricMind : MonoBehaviour
{
    [SerializeField] bool startActive;
    [SerializeField] bool loop;
    [SerializeField] bool autoUpdate;
    [SerializeField] ElectricLine electricLine;
    [SerializeField] Transform linesGroup;

    bool active;

    ElectricBall[] electricballs;
    List<ElectricLine> electricLines;

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

        active = startActive;
        UpdateActivation();

        Round round = GetComponentInParent<Round>();
        if (round)
        {
            round.OnReset += () =>
            {
                active = startActive;
                UpdateActivation();
            };
        }
    }

    private void SetupChain()
    {
        electricLines = new List<ElectricLine>();

        int max = electricballs.Length - (loop ? 0 : 1); 
        for (int i = 0; i < max; i++) 
        {
            GameObject lineObject = Instantiate (electricLine.gameObject, linesGroup);
            
            ElectricLine lineScript = lineObject.GetComponent<ElectricLine>();
            if (lineScript) 
            {
                lineScript.Setup(autoUpdate, BallAt(i+1).transform, BallAt(i).transform);
                electricLines.Add(lineScript);
            }
        }

        electricLine.gameObject.SetActive(false);
    }

    ElectricBall BallAt (int index)
    {
        return electricballs[index % electricballs.Length];
    }

    private void UpdateActivation()
    {
        foreach (ElectricLine l in electricLines)
            l.SetActivation(active);

        foreach (ElectricBall b in electricballs)
            b.SetActivation(active);
    }

    public void ToggleActivation()
    {
        active = !active;
        UpdateActivation();
    }
}
