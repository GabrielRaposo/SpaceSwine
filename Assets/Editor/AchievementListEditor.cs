using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;
using Object = System.Object;

[CustomEditor(typeof(GameAchievementList))]
public class AchievementListEditor : Editor
{
    private int initListcount;

    private int achievementToRemove;

    private SerializedProperty m_StringBoolDictionary;

    private void OnEnable()
    {
        m_StringBoolDictionary = serializedObject.FindProperty("dicHold");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        achievementToRemove = -1;

        var obj = target as GameAchievementList;
        
        if (obj.Achievements == null) obj.Achievements = new List<Achievement>();

        EditorGUILayout.Space(30);
       
        EditorGUILayout.BeginHorizontal(); //A
        initListcount = EditorGUILayout.IntField("New list:", initListcount);
        if (initListcount < 0) initListcount = 0;
        if (GUILayout.Button("Reset list"))
        {
            obj.Achievements = new List<Achievement>();

            for (int i = 0; i < initListcount; i++)
            {
                var a = new Achievement();
                a.id = $"ACV_{(i+1).ToString("000")}";
                obj.Achievements.Add(a);
            }
        }
        EditorGUILayout.EndHorizontal(); //A0

        var localizationFile = LocalizationManager.LocalizationFile;
        
        float iconSize = 55f;

        for (int i = 0; i < obj.Achievements.Count; i++)
        {
            if(i!=0) GUILayout.Space(8);
            
            var a = obj.Achievements[i];
            
            EditorGUILayout.BeginVertical("Box"); //A

            EditorGUILayout.BeginHorizontal(); //BT

            
            var rect =EditorGUILayout.BeginVertical(GUILayout.MinWidth(iconSize), GUILayout.MinHeight(iconSize));//OBJ
            
            GUILayout.Space(iconSize);
            
            a.sprite = (Sprite)EditorGUI.ObjectField(rect, a.sprite, typeof(Sprite), a.sprite);
            
            EditorGUILayout.EndVertical();//OBJ0

            EditorGUILayout.BeginVertical();//TEXT
            
            EditorGUILayout.BeginHorizontal();//B

            a.id = EditorGUILayout.TextField("", a.id, GUILayout.MaxWidth(62));
            EditorGUILayout.LabelField(localizationFile.GetAchievementName(a.id), EditorStyles.boldLabel);
            
            if (GUILayout.Button("X", EditorStyles.miniButton, GUILayout.MaxWidth(30f)))
                achievementToRemove = i;
            
            EditorGUILayout.EndHorizontal(); //B0
            
            EditorGUILayout.LabelField(localizationFile.GetAchievementDescription(a.id));
            
            EditorGUILayout.EndVertical();//TEXT0
            
            EditorGUILayout.EndHorizontal();//BT0
            
            EditorGUILayout.BeginHorizontal();//C


            if (EditorTools.SettableButton("Normal", a.achievementType != AchievementType.Base))
            {
                a.achievementType = AchievementType.Base;
            }

            if (EditorTools.SettableButton("Number progress", a.achievementType != AchievementType.NumericalProgress))
            {
                a.achievementType = AchievementType.NumericalProgress;
            }

            if (EditorTools.SettableButton("Event progress", a.achievementType!= AchievementType.Events))
            {
                a.achievementType = AchievementType.Events;
            }

            EditorGUILayout.EndHorizontal(); //C0

            Rect r = EditorGUILayout.BeginVertical();
            
            switch (a.achievementType)
            {
                case AchievementType.Events:

                    int remove = -1;

                    for (int j = 0; j < a.events.Count; j++)
                    {
                        EditorGUILayout.BeginHorizontal();
                        a.events[j] = GUILayout.TextArea(a.events[j]);
                        if (GUILayout.Button("X", GUILayout.MaxWidth(50)))
                            remove = j;
                        EditorGUILayout.EndHorizontal();
                    }
                    
                    if(GUILayout.Button("Add"))
                        a.events.Add("");

                    if(remove!=-1)
                        a.events.RemoveAt(remove);
                    
                    break;
                
                case AchievementType.NumericalProgress:
                    a.amountRequired = EditorGUILayout.IntField("Amout Required", a.amountRequired);
                    break;
                default:
                    break;
            }
            
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndVertical(); //A0
        }
        
        if (achievementToRemove != -1)
        {
            obj.Achievements.RemoveAt(achievementToRemove);
        }

        if (GUILayout.Button("Add"))
        {
            var newAchievement = new Achievement {id = $"ACV_{(obj.Achievements.Count+1).ToString("000")}"};
            obj.Achievements.Add(newAchievement);
        }
        
        EditorUtility.SetDirty(obj);

    }

}
