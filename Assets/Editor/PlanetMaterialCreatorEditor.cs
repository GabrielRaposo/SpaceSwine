using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

[CustomEditor(typeof(PlanetMaterialCreator))]
public class PlanetMaterialCreatorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        PlanetMaterialCreator obj = target as PlanetMaterialCreator;
        
        //base.OnInspectorGUI();

        GUILayout.BeginHorizontal();
        
        obj.baseMaterial = (Material)EditorGUILayout.ObjectField("Base Material", obj.baseMaterial, typeof(Material));

        GUIStyle myButtonStyle = new GUIStyle(GUI.skin.button);
        myButtonStyle.fontSize = 13;

        myButtonStyle.hover.textColor = new Color(0.2f, 0.8f, 0.4f);

        myButtonStyle.fixedWidth = 40;
        
        if(GUILayout.Button("⇌", myButtonStyle))
            obj.ClarMaterial();
        
        GUILayout.EndHorizontal();
        
        obj.currentMaterial = (Material)EditorGUILayout.ObjectField("Material", obj.currentMaterial, typeof(Material));


    }
}
