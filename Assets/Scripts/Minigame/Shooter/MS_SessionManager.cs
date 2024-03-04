using Shooter;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MS_SessionManager : MonoBehaviour
{
    [SerializeField] MS_Session testSession;

    [Space(20)]

    [SerializeField] MS_Player player;

    [SerializeField] MS_Session firstSession;
    [SerializeField] MS_Session secondSession;

    [Space(20)]

    [SerializeField] Transform difficulty1Group;
    [SerializeField] Transform difficulty2Group;

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
        switch (level)
        {
            default:
                return GetRandomFromGroup(difficulty1Group);

            case 0:
                if (testSession != null)
                    return sessionsCompleted == 0 ? firstSession : testSession;
                return sessionsCompleted == 0 ? firstSession : secondSession;

            case 1:
                return GetRandomFromGroup(difficulty1Group);

            case 2:
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
