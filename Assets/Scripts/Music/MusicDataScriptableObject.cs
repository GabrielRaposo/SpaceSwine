using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "Track_", menuName = "ScriptableObjects/Music Data") ]
public class MusicDataScriptableObject : ScriptableObject
{
    public string fileName;
    public string trackName;
    public string creatorName;
    public AK.Wwise.Event akEvent; 
}
