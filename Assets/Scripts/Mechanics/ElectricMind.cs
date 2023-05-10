﻿using System;
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
    [SerializeField] ElectricMindEffect lightningEffect;

    bool active;

    ElectricLock[] electricLocks;
    ElectricBall[] electricballs;
    List <ElectricLine> electricLines;
    List <ElectricMindEffect> electricEffects;
    List <Transform> targets;

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
        SetupEffects();

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

        targets = new List<Transform>();
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

    private void SetupEffects()
    {
        if (!lightningEffect)
            return;

        electricEffects = new List<ElectricMindEffect>();
        
        if (targets.Count < 2)
            return;

        for (int i = 0; i < targets.Count - 1; i++)
        {
            GameObject effectObj = Instantiate(lightningEffect.gameObject, linesGroup);
            effectObj.SetActive(true);

            ElectricMindEffect effectScript = effectObj.GetComponent<ElectricMindEffect>();
            effectScript.Setup(targets[i+1], targets[i]);

            electricEffects.Add (effectScript);
        }

        lightningEffect.gameObject.SetActive(false);

        //if (electricballs.Length < 4)
        //    return;

        //lightningEffect.Setup (electricballs[2].transform, electricballs[3].transform);
    }

    private void UpdateActivation()
    {
        foreach (ElectricLine l in electricLines)
            l.SetActivation(active);

        foreach (ElectricBall b in electricballs)
            b.SetActivation(active);

        foreach (ElectricMindEffect e in electricEffects)
            e.SetVisibility(active);

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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        var gizmosBallArray = GetComponentsInChildren<ElectricBall>();

        if (gizmosBallArray == null || gizmosBallArray.Length < 2)
            return;

        var electrickLocksGizmo = GetComponentsInChildren<ElectricLock>();
        
        List<Transform> targets = new List<Transform>();
        for (int i = 0; i < gizmosBallArray.Length; i++)
        {
            targets.Add(gizmosBallArray[i].transform);

            if (electrickLocksGizmo == null || electrickLocksGizmo.Length < 1)
                continue;

            Transform t = null;
            foreach (ElectricLock l in electrickLocksGizmo)
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
        {
            targets.Add(targets[0]);
        }

        Vector3[] positions = new Vector3[targets.Count];
        for (int i = 0; i < targets.Count; i++)
        {
            positions[i] = targets[i].position;
        }

        for (int i = 0; i < positions.Length - 1; i++)
        {
            Gizmos.DrawLine(positions[i], positions[i + 1]);
        }
    }
}
