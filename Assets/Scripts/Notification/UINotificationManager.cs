using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UINotificationManager : MonoBehaviour
{
    static List <UINotification> notificationsList = new List <UINotification>();

    static UINotificationManager Instance;

    public UINotificationManager()
    {
        if (Instance != null)
            return;

        Instance = this;
    }

    private void Start() 
    {
        StartCoroutine (WaitForSaveManager());
    }

    private IEnumerator WaitForSaveManager()
    {
        yield return new WaitUntil ( () => SaveManager.Initiated );
        
        LoadNotificationsList();
    }

    private static void LoadNotificationsList()
    {
        notificationsList = SaveManager.GetUINotifications();

        PrintList();
    }

    public static void UpdateListFromSave (List<UINotification> savedList)
    {
        if (savedList.Count < 1)
            return;

        foreach (UINotification n in savedList)
        {
            Create (n.id);

            if (n.GetState() == UINotification.State.Used)
                Use (n.id);
        }
    }

    public static void Create (string notificationID)
    {
        foreach (UINotification n in notificationsList)
        {
            if (n.id.CompareTo(notificationID) == 0)
                return;
        }
        
        notificationsList.Add (new UINotification() { id = notificationID });

        SaveManager.SetUINotifications (notificationsList);

        #if UNITY_EDITOR
            PrintList();
        #endif
    }

    private static UINotification GetById (string notificationID)
    {
        UINotification notification = null; 
        foreach (UINotification n in notificationsList)
        {
            if (n.id.CompareTo (notificationID) == 0)
            {
                notification = n;
                break;
            }
        }
        return notification;
    }

    public static bool Check (string notificationID)
    {
        if (notificationsList.Count < 1)
            return false;

        UINotification notification = GetById (notificationID);
        if (notification == null)
            return false;

        if (notification.GetState() == UINotification.State.Used)
            return false;

        return true;
    }

    public static void Use (string notificationID)
    {
        if (notificationsList.Count < 1)
            return;

        UINotification notification = GetById (notificationID);
        if (notification == null)
            return;

        notification.Use();

        SaveManager.SetUINotifications (notificationsList);
    }

    private static void PrintList()
    {
        if (notificationsList.Count < 1)
            return;

        string s = "Notifications ::: \n";
        foreach (UINotification n in notificationsList)
            s += n.id + "\n";

        Debug.Log(s);
    }
}
