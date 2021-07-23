using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AchievementList", menuName = "ScriptableObjects/Achievements")]
public class GameAchievementList : ScriptableObject
{
    public List<Achievement> Achievements;
}
