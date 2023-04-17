using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEditor;

public class StoryEventSaveConverter : MonoBehaviour
{
    /**
    static string path = "Assets/Resources/StoryEvents";

    public static List<StoryEventScriptableObject> GetStoryEventsFromAssets()
    {
        string[] GUIDs = AssetDatabase.FindAssets("", new string[] { eventsPath });
        List<StoryEventScriptableObject> storyEvents = new List<StoryEventScriptableObject>();

        //Debug.Log("paths.Length: " + GUIDs.Length);

        for (int i = 0; i < GUIDs.Length; i++)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(GUIDs[i]);
            //Debug.Log("assetPath: " + assetPath);
            StoryEventScriptableObject storyEvent
                = AssetDatabase.LoadAssetAtPath<StoryEventScriptableObject>(assetPath);

            if (storyEvent != null)
            {
                storyEvents.Add (storyEvent);
            }
        }        

        return storyEvents;
    }
    **/

    public static StoryEventScriptableObject[] GetStoryEventsFromResources()
    {
        StoryEventScriptableObject[] storyEvents = Resources.FindObjectsOfTypeAll<StoryEventScriptableObject>();     
        //Debug.Log("storyEvents.Length: " + storyEvents.Length);

        return storyEvents;
    }

    public static void FromSaveToAssets()
    {
        //List<StoryEventData> storyEventDatas = SaveManager.GetStoryEvents();
        //if (storyEventDatas.Count < 1)
        //    return;

        //StoryEventScriptableObject[] storyEvents = GetStoryEventsFromResources();
        //if ( storyEvents.Length < 1 )
        //    return;

        //for (int i = 0; i < storyEvents.Length; i++) 
        //{
        //    StoryEventData data = storyEventDatas.Find((d) => d.name == storyEvents[i].name);
        //    if (data == null)
        //        continue;

        //    storyEvents[i].State = data.state;
        //}
    } 

    public static void FromAssetsToSave()
    {
        //var storyEvents = GetStoryEventsFromResources();
        //if (storyEvents.Length < 1)
        //    return;

        //List<StoryEventData> dataList = new List<StoryEventData>();

        //for (int i = 0; i < storyEvents.Length; i++)
        //{
        //    dataList.Add( new StoryEventData( storyEvents[i].state, storyEvents[i].name ) );
        //}

        //SaveManager.SetAllStoryEvents(dataList);
    }
}
