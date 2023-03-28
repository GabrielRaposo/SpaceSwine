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

    ElectricLock[] electricLocks;
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

        electricLocks = GetComponentsInChildren<ElectricLock>();

        SetupLocks();
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

        List<Transform> targets = new List<Transform>();
        for (int i = 0; i < electricballs.Length; i++)
        {
            targets.Add( electricballs[i].transform );

            if (electricLocks == null || electricLocks.Length < 1)
                continue;

            Transform t = null;
            foreach (ElectricLock l in electricLocks)
            {
                if (l.transform.GetSiblingIndex() - 2 == i)
                {
                    t = l.GetConnectionPoint();
                    break;
                }
            }

            if (t != null)
                targets.Add(t);
        }
        if (loop && targets.Count > 0)
            targets.Add(targets[0]);

        for (int i = 0; i < targets.Count - 1; i++)
        {
            GameObject lineObject = Instantiate (electricLine.gameObject, linesGroup);

            ElectricLine lineScript = lineObject.GetComponent<ElectricLine>();
            if (lineScript) 
            {
                lineScript.Setup(autoUpdate, targets[i + 1], targets[i]);
                electricLines.Add(lineScript);
            }
        }

        electricLine.gameObject.SetActive(false);
    }

    private void SetupLocks()
    {
        if (electricLocks == null || electricLocks.Length < 1)
            return;

        foreach (ElectricLock l in electricLocks)
        {
            l.Setup(this);
        }
    }

    private void UpdateActivation()
    {
        foreach (ElectricLine l in electricLines)
            l.SetActivation(active);

        foreach (ElectricBall b in electricballs)
            b.SetActivation(active);

        if (electricLocks != null)
        {
            foreach (ElectricLock l in electricLocks)
                l.SetState(active);
        }
    }

    public void ToggleActivation()
    {
        SetActivation(!active);
    }

    public void SetActivation(bool value)
    {
        if (value == active)
            return;

        active = value;
        UpdateActivation();
    }
}
