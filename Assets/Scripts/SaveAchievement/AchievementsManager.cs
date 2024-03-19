using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

public enum AchievementEnum
{   //Adicionar achievements aqui
    IKnowAShortcut,
    AsimovsLaws,
    HistoricalPreservation,
    Beatmaker,
    TheBody,
    ElectromagneticInterference,
    TheSpirit,
    Cryptogeologist,
    FruitsOfLabor,
    TheMind,
    Aficionado,
    LofiBeats,
    JumperMaster,
    ShooterMaster,
    LooperMaster,
    JumperExtra,
    ShooterExtra,
    LooperExtra,
    SundayAfternoon
}

public static class AchievementsManager
{
    private const string achievementListAddress = "AchievementList";
    public static List<AchievementLog> currentAchievementLog;
    public static List<Achievement> achievementList;
    //public static Action<Achievement> achievementAnimation;

    private static GameAchievementList loadedList;

    private static Dictionary<AchievementEnum, string> EnumStringDictionary;

    public static bool turnOffAchievments = false;

    static AchievementsManager()
    {
        EnumStringDictionary = new Dictionary<AchievementEnum, string>
        {   //Inicializar dicionário com os ids aqui
            { AchievementEnum.IKnowAShortcut , "$0"},
            { AchievementEnum.AsimovsLaws , "$1"},
            { AchievementEnum.HistoricalPreservation , "$2"},
            { AchievementEnum.Beatmaker , "$3"},
            { AchievementEnum.TheBody , "$4"},
            { AchievementEnum.ElectromagneticInterference , "$5"},
            { AchievementEnum.TheSpirit , "$6"},
            { AchievementEnum.Cryptogeologist , "$7"},
            { AchievementEnum.FruitsOfLabor , "$8"},
            { AchievementEnum.TheMind , "$9"},
            { AchievementEnum.Aficionado , "$10"},
            { AchievementEnum.LofiBeats , "$11"},
            { AchievementEnum.JumperMaster , "$12"},
            { AchievementEnum.ShooterMaster , "$13"},
            { AchievementEnum.LooperMaster , "$14"},
            { AchievementEnum.JumperExtra , "$15"},
            { AchievementEnum.ShooterExtra , "$16"},
            { AchievementEnum.LooperExtra , "$17"},
            { AchievementEnum.SundayAfternoon , "$18"},
        };
    }
    
    public static List<Achievement> GetNewAchievementList()
    {
        GameAchievementList resourcesList = Resources.Load(achievementListAddress) as GameAchievementList;
        if (resourcesList == null)
        {
            Debug.LogError("Cant find achievement");
            return new List<Achievement>();
        }

        var newList = new List<Achievement>();

        foreach (var a in resourcesList.Achievements)
            newList.Add(new Achievement(a));
        
        return newList;
    }

    public static void SetCurrentList(List<AchievementLog> l)
    {
        achievementList = GetNewAchievementList();
        currentAchievementLog = l;
    }

    private static Achievement GetAchievement(string id)
    {
        if (achievementList == null)
        {
            Debug.LogError("No Achievement list");
            return null;
        }
        
        var a = achievementList.Find(x => x.id == id);
        
        if (a == null)
        {
            Debug.LogError($"Achievement not found {id}");
            return null;
        }
        return a;
    }

    private static AchievementLog GetAchievementLog(string id)
    {
        if (currentAchievementLog == null)
        {
            Debug.LogError("No achievement log");
            return null;
        }

        var a = currentAchievementLog.Find(x => x.Achievement.id == id);

        if (a == null)
        {
            Debug.LogError($"Achievement not found {id}");
            return null;
        }
        return a;

    }

    public static string GetAchievementId(AchievementEnum achievementEnum)
    {
        if(!EnumStringDictionary.ContainsKey(achievementEnum)) return "";
        EnumStringDictionary.TryGetValue(achievementEnum, out string achievementId);
        return achievementId;
    }

    private static Achievement GetAchievement(AchievementEnum achievementEnum)
    {
        if(!EnumStringDictionary.ContainsKey(achievementEnum)) return null;
        EnumStringDictionary.TryGetValue(achievementEnum, out string achievementId);
        return GetAchievement(achievementId);
    }

    public static bool GetAchievementState(string id)
    {
        var a = GetAchievementLog(id);

        if (a == null) return false;

        return a.achieved;
    }

    public static bool GetAchievementState(AchievementEnum achievementEnum)
    {
        if (turnOffAchievments) return false;
        
        if(!EnumStringDictionary.TryGetValue(achievementEnum, out string s)) return false;

        return GetAchievementState(s);

    }

    private static void SetAchievementState(string id,bool state)
    {
        if (turnOffAchievments) return;
        
        if (currentAchievementLog == null) return;
        
        var a = GetAchievementLog(id);

        if(a == null) return;
        if(a.achieved == state) return;
        
        a.achieved = state;
        //Debug.Log($"<color=#45de00>Setting achievement progress for - {LocalizationManager.GetAchievementName(a.Achievement.id)} -> {state}</color>");
        DebugDisplay.Log($"<color=#45de00>Setting achievement progress for - {LocalizationManager.GetAchievementName(a.Achievement.id)} -> {state}</color>");
        
        //if(state) achievementAnimation.Invoke(a.Achievement);
    }

    public static void SetAchievementState(AchievementEnum achivEnum, bool state)
    {
        if (turnOffAchievments) return;
        
        if(!EnumStringDictionary.TryGetValue(achivEnum, out string s)) return;
        
        SetAchievementState(s, state);
    }

    private static void ProgressAchievementAndComplete(string id, int amount)
    {
        if (turnOffAchievments) return;
        
        if (currentAchievementLog == null) return;
        AchievementLog a = GetAchievementLog(id);

        if(a == null) return;
        if(a.achieved) return;
        
        if(a.Achievement.achievementType != AchievementType.NumericalProgress) return;
        
        a.AddProgress(amount);
        
        Debug.Log($"<color=#45de00>Progress of {a.Achievement.id}: {a.GetProgress()}/{a.GetAmountRequired()}</color>");

        if (a.GetProgress() >= a.GetAmountRequired())
        {
            a.achieved = true;
            //achievementAnimation.Invoke(a.Achievement);
        }
    }

    public static void ProgressAchievementAndComplete(AchievementEnum achievEnum, int amount)
    {
        if(!EnumStringDictionary.ContainsKey(achievEnum)) return;

        EnumStringDictionary.TryGetValue(achievEnum, out string achievementId);
        
        ProgressAchievementAndComplete(achievementId, amount);
    }

    private static void SetAchievementProgressPart(string id, string code, bool value)
    {
        if(turnOffAchievments) return;
        
        if(currentAchievementLog == null) return;
        var a = GetAchievementLog(id);

        if(a == null) return;
        
        if(a.Achievement.achievementType != AchievementType.Events) return;
        
        a.SetCodeProgress(code, value);
        Debug.Log($"<color=#45de00>SetAchievementProgressPart: {id} code:{code} progress:{a.GetProgress()}/{a.GetAmountRequired()}</color>");

        if(a.achieved) return;

        if (a.GetProgress() >= a.GetAmountRequired())
        {
            a.achieved = true;
            //achievementAnimation.Invoke(a.Achievement);
        }
    }

    public static void SetAchievementProgressPart(AchievementEnum achievementEnum, string code, bool value)
    {
        if(!EnumStringDictionary.ContainsKey(achievementEnum)) return;

        EnumStringDictionary.TryGetValue(achievementEnum, out string achievementId);
        
        SetAchievementProgressPart(achievementId, code, value);
    }
    
}

public enum AchievementType
{
    Base,
    NumericalProgress,
    Events
} 

[Serializable]
public class Achievement
{
    //public Sprite sprite;
    
    public AchievementType achievementType;
    
    public string id;
    
    public int amountRequired;

    public List<string> events;
    
    public Achievement()
    {
        id = "";
        events = new List<string>();
    }
    
    public Achievement(Achievement a)
    {
        this.id = a.id;
        this.achievementType = a.achievementType;

        this.amountRequired = a.amountRequired;

        this.events = new List<string>();
        foreach (string achievementEvent in a.events)
        {
            this.events.Add(achievementEvent);
        }
    }
}

[System.Serializable]
public class AchievementLog
{
    private Achievement achievement;
    public Achievement Achievement => achievement;
    public bool achieved;
    public int progress;
    public List<AchievementEvent> events;

    public AchievementLog(Achievement achievement)
    {
        this.achievement = new Achievement(achievement);
        if (achievement.achievementType == AchievementType.Events)
        {
            foreach (string e in achievement.events)
            {
                events.Add(new AchievementEvent(e, false));
            }
        }
    }
    public virtual int GetProgress()
    {
        switch (Achievement.achievementType)
        {
            case AchievementType.NumericalProgress:
                return progress;
                
            case AchievementType.Events:

                int a = 0;

                foreach (AchievementEvent achievementEvent in events)
                {
                    if (achievementEvent.concluded) a++;
                }

                return a;
        }

        return -1;
    }
    
    public virtual void SetProgress(int value)
    {
        if(Achievement.achievementType!= AchievementType.NumericalProgress) return;
        progress = value;
    }

    public virtual void AddProgress(int value)
    {
        if(Achievement.achievementType!= AchievementType.NumericalProgress) return;
        progress += value;
    }
    
    public void SetCodeProgress(string code, bool value)
    {
        AchievementEvent e = events.Find(x => x.code == code);
        
        if(e == null) return;

        e.concluded = value;
        
        Debug.Log($"<color=#45de00>Event Progress (code: {code}):  {GetProgress()}/{GetAmountRequired()}</color>");
    }

    public virtual int GetAmountRequired()
    {
        switch (Achievement.achievementType)
        {
            case AchievementType.NumericalProgress:
                return Achievement.amountRequired;
            
            case AchievementType.Events:
                return events.Count;
        }
        return -1;
    }
}

    [Serializable]
public class AchievementEvent
{
    public string code;
    public bool concluded;

    public AchievementEvent()
    {
        
    }
    public AchievementEvent(string achievementEventCode, bool achievementEventConcluded)
    {
        this.code = achievementEventCode;
        this.concluded = achievementEventConcluded;
    }
}