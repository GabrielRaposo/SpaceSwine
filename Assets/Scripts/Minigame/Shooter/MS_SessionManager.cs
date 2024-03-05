﻿using Shooter;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MS_SessionManager : MonoBehaviour
{
    [SerializeField] MS_Session testSession;

    [Space(20)]


    [SerializeField] MS_Session firstSession;
    [SerializeField] MS_Session secondSession;
    [SerializeField] MS_Session sunSession;

    [Space(20)]

    [SerializeField] Transform difficulty1Group;
    [SerializeField] Transform difficulty2Group;

    [Space(20)]

    [SerializeField] MS_Player player;
    
    MS_Session currentSession;
    MS_ProgressDisplay progressDisplay;

    int sessionsCompleted;
    int level;

    public static bool OnSessionTransition;

    void Start()
    {
        progressDisplay = MS_ProgressDisplay.Instance;
        UpdateStageDisplay();

        DeactivateAllSessions(transform);
        DeactivateAllSessions(difficulty1Group);
        DeactivateAllSessions(difficulty2Group);

        SetNewSession();
    }
    
    private void UpdateStageDisplay()
    {
        if (progressDisplay == null)
            return;

        progressDisplay.UpdateStage(sessionsCompleted);
    }

    private MS_Session GetSessionByLevel()
    {
        if ( sessionsCompleted % 15 == 0 )
            return sunSession;

        switch (level)
        {
            case 0:
                if (testSession != null)
                    return sessionsCompleted == 0 ? firstSession : testSession;
                return sessionsCompleted == 0 ? firstSession : secondSession;

            case 1:
                return GetRandomFromGroup( GetGroupByChance (9,1,0) );

            case 2:
                return GetRandomFromGroup( GetGroupByChance (6,4,0) );

            case 3:
                return GetRandomFromGroup( GetGroupByChance (3,7,0) );

            case 4:
                return GetRandomFromGroup( GetGroupByChance (1,7,2) );

            case 5:
                return GetRandomFromGroup( GetGroupByChance (1,4,5) );

            case 6:
                return GetRandomFromGroup( GetGroupByChance (1,2,7) );

            case 7:
                return GetRandomFromGroup( GetGroupByChance (0,1,9) );

            default:
                //return GetRandomFromGroup(difficulty3Group);
                return GetRandomFromGroup(difficulty2Group);
        }
    }

    private void SetNewSession()
    {
        currentSession = GetSessionByLevel();
        currentSession.Setup(this);
    }

    public void NotifyCompletedSession()
    {
        sessionsCompleted++;
        UpdateStageDisplay();
        UpdateLevel();

        player.ClearActiveBullets();
        if ( (sessionsCompleted - 1) % 5 == 0 )
            player.RestoreAmmo();

        SetNewSession();
    }

    private void UpdateLevel()
    {

        //if (sessionsCompleted > 4)
        //{
        //    level = 2;
        //    return;
        //}

        if ((sessionsCompleted + 1) % 3 == 0)
        {
            level++;
            Debug.Log("Level up: " + level);
        }

        //if (sessionsCompleted > 1)
        //{
        //    level = 1;
        //    return;
        //}



    }

    private void DeactivateAllSessions (Transform group)
    {
        for (int i = 0; i < group.childCount; i++) 
        {
            MS_Session s = group.GetChild(i).GetComponent<MS_Session>();

            if (!s) 
                continue;

            s.gameObject.SetActive(false);
        }

    }

    private MS_Session GetRandomFromGroup (Transform group)
    {
        List<MS_Session> sessions = new List<MS_Session>();
        for (int i = 0; i < group.childCount; i++) 
        {
            MS_Session s = group.GetChild(i).GetComponent<MS_Session>();

            if (!s) 
                continue;

            if (s == currentSession && transform.childCount > 1)
                continue;

            sessions.Add(s);
        }

        if (sessions.Count < 1)
            return null;

        return sessions[Random.Range(0, sessions.Count)];   
    }

    private Transform GetGroupByChance (int group1Ratio, int group2Ratio, int group3ratio)
    {
        int r = Random.Range(0, group1Ratio + group2Ratio + group3ratio);

        if (r < group1Ratio)
            return difficulty1Group;
        
        if (r < group1Ratio + group2Ratio)
            return difficulty2Group;

        return difficulty2Group;

    }
}
