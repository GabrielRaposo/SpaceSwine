using Shooter;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MS_SessionManager : MonoBehaviour
{
    [SerializeField] MS_Player player;

    [SerializeField] MS_Session firstSession;
    [SerializeField] MS_Session secondSession;

    [SerializeField] Transform difficulty1Group;

    MS_Session currentSession;
    MS_ProgressDisplay progressDisplay;

    int sessionsCompleted;
    int level;

    void Start()
    {
        progressDisplay = MS_ProgressDisplay.Instance;
        UpdateStageDisplay();

        DeactivateAllSessions(transform);
        DeactivateAllSessions(difficulty1Group);

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
        switch (level)
        {
            case 0:
                return sessionsCompleted == 0 ? firstSession : secondSession;

            case 1:
            default:
                return GetRandomFromGroup(difficulty1Group);

            //default:
            //    return firstSession;
        }
    }

    private void SetNewSession()
    {
        currentSession = GetSessionByLevel();
        currentSession.gameObject.SetActive(true);
        currentSession.Setup(this);
    }

    public void NotifyCompletedSession()
    {
        sessionsCompleted++;
        UpdateStageDisplay();
        UpdateLevel();

        player.ClearActiveBullets();
        
        if (currentSession != null)
            currentSession.gameObject.SetActive(false);
        
        SetNewSession();
    }

    private void UpdateLevel()
    {
        if (sessionsCompleted > 5)
        {
            level = 2;
            return;
        }

        if (sessionsCompleted > 1)
        {
            level = 1;
            return;
        }
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
}
