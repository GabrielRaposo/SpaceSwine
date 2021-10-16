using UnityEngine;
using UnityEditor;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

class GradientTextureCreator : EditorWindow
{
    private AnimationCurve curve;
    private int size;
    private string textureName;
    
    [MenuItem("Window/Gradient Creator")] 
    public static void  ShowWindow ()
    {
        EditorWindow.GetWindow(typeof(GradientTextureCreator));
    }
    
    void OnGUI ()
    {
        if(curve == null)
            curve = new AnimationCurve();
        
        curve = EditorGUILayout.CurveField("Curve",curve);
        size = EditorGUILayout.IntField("Size", size);
        textureName = EditorGUILayout.TextField("Path", textureName);
        
        if(GUILayout.Button("Create Texture"))
            CreateTexture();
        
    }

    public void CreateTexture()
    {
        Texture2D tex = new Texture2D(size, 1);

        for (int i = 0; i < size; i++)
        {
            float s = size;
            
            float value = curve.Evaluate(i / s);
            
            tex.SetPixel(i,0, new Color(value, value, value));
            tex.Apply();
        }

        byte[] bytes = tex.EncodeToPNG();
        
        string savePath = Application.dataPath+ "/Textures/" + textureName + ".png";
        
        
        File.WriteAllBytes(savePath, bytes);
        
        AssetDatabase.Refresh();
    }
    
}