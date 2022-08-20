using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Round Session", menuName = "ScriptableObjects/Round Session")]
public class RoundSessionData : ScriptableObject
{
    [Header("Include-Include")]
    public int startingIndex;
    public int lastIndex;

    [Space(10)]
    public BuildIndex outroScene;
    public int AbandonSpawnIndex;
    public int OutroSpawnIndex;
    public UnityAction OnSessionCompleted;
}