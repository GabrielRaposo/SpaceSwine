using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "AdventureLog_", menuName = "ScriptableObjects/Adventure Log") ]
public class AdventureLogScriptableObject : ScriptableObject
{
    public string fallbackText;
    public string logTextID;

    [Space(10)]

    public List<StoryEventScriptableObject> activationEventKeys;
    public StoryEventScriptableObject completionEventKey;
}
