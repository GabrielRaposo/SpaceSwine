using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Round Session", menuName = "ScriptableObjects/Round Session")]
public class RoundSessionData : ScriptableObject
{
    public int startingIndex;
    public int lastIndex;
    public BuildIndex outroScene;
    public int spawnIndex;
    public UnityAction OnSessionCompleted;
}