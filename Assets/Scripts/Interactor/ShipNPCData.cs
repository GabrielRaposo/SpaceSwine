using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ship Data", menuName = "ScriptableObjects/Ship Data")]
public class ShipNPCData : NPCData
{
    public bool skippable;
    public ShipSceneType sceneType;
}
