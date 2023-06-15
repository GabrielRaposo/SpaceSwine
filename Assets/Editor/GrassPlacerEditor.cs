using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GrassPlacer))]
public class GrassPlacerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var obj = target as GrassPlacer;
        int previousGrassAmount = obj.grassAmount;
        
        base.OnInspectorGUI();

        if (GUILayout.Button("Shuffle Grass"))
            ShuffleGrass(obj);
        
        if(obj.grassReference == null) return;

            if (obj.grass == null) obj.grass = new List<GameObject>();

        int dif = obj.grassAmount - previousGrassAmount;

        GUILayout.Space(10);
        GUILayout.Label("Add new objects", new GUIStyle(GUI.skin.label){fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter});
        
        MiscObjectsPlacer(obj);
        GUILayout.Space(5);
        MiscObjects(obj);
        
        
        GrassAmountManager(dif, obj);
        SetGrassSpacing(obj);
        SceneView.RepaintAll();
        EditorUtility.SetDirty(obj);
    }

    private void MiscObjectsPlacer(GrassPlacer obj)
    {
        int labelheight = 48;
        
        if (obj.miscObjectsReference == null)
            obj.miscObjectsReference = new List<GameObject>();

        GUILayout.BeginHorizontal();
        
        for (int i = 0; i < obj.miscObjectsReference.Count; i++)
        {
            var o = obj.miscObjectsReference[i];
            if(o ==null)
                continue;
            
            var image = AssetPreview.GetAssetPreview(o);
            
            if (GUILayout.Button(image, GUILayout.Width(labelheight), GUILayout.Height(labelheight)))
            {
                var newObj = Instantiate(o, obj.transform);
                obj.miscObjects.Add(newObj);
                var rand = newObj.GetComponent<RandomizeSpriteFromPool>();
                if(rand!=null)
                    rand.RandomizeSprite();
            }
        }
        GUILayout.EndHorizontal();
        
    }
    
    private void MiscObjects(GrassPlacer obj)
    {
        int labelSize = 50;
        int objectToRemove = -1;
        
        List<List<GameObject>> allGroups;

        allGroups = new List<List<GameObject>>();

        foreach (GameObject referenceObj in obj.miscObjectsReference)
        {
            var l = new List<GameObject>();

            foreach (GameObject miscObject in obj.miscObjects)
            {
                if(miscObject.name.Contains(referenceObj.name))
                    l.Add(miscObject);
            }
            
            allGroups.Add(l);
            
        }

        foreach (List<GameObject> list in allGroups)
        {
            if(list.Count == 0) continue;
            
            GUILayout.BeginVertical("Box");
            
            for (var i = 0; i < list.Count; i++)
            {
                
                if(i!=0)
                    GUILayout.Label("---------------------------------------------------------------------------", new GUIStyle(GUI.skin.label){alignment = TextAnchor.MiddleCenter}, GUILayout.Height(4));
                GUILayout.Space(2);
                
                GameObject gameObject = list[i];
                GUILayout.BeginHorizontal(GUILayout.Height(labelSize)); //A
                MiscObjectControls(gameObject, obj.miscObjects.IndexOf(gameObject));
                GUILayout.EndHorizontal(); //A0
                GUILayout.Space(2);
            }
            
            GUILayout.EndVertical();
        }
        
        if(objectToRemove == -1)
            return;
        
        DestroyImmediate(obj.miscObjects[objectToRemove]);
        obj.miscObjects.RemoveAt(objectToRemove);

        void MiscObjectControls(GameObject gameObject, int index)
        {
            GUILayout.Label(AssetPreview.GetAssetPreview(gameObject), GUILayout.Height(labelSize), GUILayout.Width(labelSize));
            GUILayout.BeginHorizontal(GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
            
            //SpaceControll
            var attach = gameObject.GetComponent<AttachToPlanet>();
            if (attach != null)
            {
                GUILayout.BeginVertical();
                
                    GUILayout.BeginHorizontal();
                GUILayout.Label("^v", GUILayout.Width(16));
                attach.heightOffset = GUILayout.HorizontalSlider(attach.heightOffset, -2f, 2f, GUILayout.Height(labelSize/2), GUILayout.ExpandWidth(true));
                    GUILayout.EndHorizontal();
                    
                    GUILayout.BeginHorizontal();
                GUILayout.Label("R", GUILayout.Width(16));
                attach.angle = GUILayout.HorizontalSlider(attach.angle, 0f, 360f, GUILayout.Height(labelSize/2));
                    GUILayout.EndHorizontal();
                GUILayout.EndVertical();
            }

            //Shuffle
            var rand = gameObject.GetComponent<RandomizeSpriteFromPool>();
            if (rand != null)
            {
                GUILayout.BeginVertical();
                if(GUILayout.Button("Shuflle", GUILayout.ExpandHeight(true)))
                    rand.RandomizeSprite();

                GUILayout.BeginHorizontal();
                for (int i = 0; i < rand.sprites.Count; i++)
                {
                    int spriteIndex = i;
                    if (GUILayout.Button(spriteIndex.ToString()))
                        rand.ForceSpecificSprite(spriteIndex);
                }
                GUILayout.EndHorizontal();
                
                GUILayout.EndVertical();    
            }
            
            GUILayout.BeginVertical(GUILayout.Width(5));
            if (GUILayout.Button("X", GUILayout.ExpandHeight(true)))
            {
                objectToRemove = index;
            }

            GUILayout.EndVertical();
            
            GUILayout.EndHorizontal();
        }
    }

    private void ShuffleGrass(GrassPlacer obj)
    {
        for (int i = 0; i < obj.grass.Count; i++)
        {
            var rand = obj.grass[i].GetComponent<RandomizeSpriteFromPool>();
            if(rand == null) continue;
            rand.RandomizeSprite();
        }
    }

    private static void GrassAmountManager(int dif, GrassPlacer obj)
    {
        if (dif > 0)
        {
            // Add objects
            for (int i = 0; i < dif; i++)
            {
                int index = i;
                GameObject newGrassObject = Instantiate(obj.grassReference, obj.transform);
                obj.grass.Add(newGrassObject);

                AttachToPlanet attachToPlanet = newGrassObject.GetComponent<AttachToPlanet>();
            }
        }
        else if (dif < 0)
        {
            // Remove objects
            int removalCount = Mathf.Abs(dif);
            for (int i = 0; i < removalCount; i++)
            {
                int lastIndex = obj.grass.Count - 1;
                GameObject grassObject = obj.grass[lastIndex];
                obj.grass.RemoveAt(lastIndex);
                DestroyImmediate(grassObject);
            }
        }
    }

    private static void SetGrassSpacing(GrassPlacer obj)
    {
        for (int i = 0; i < obj.grass.Count; i++)
        {
            var attach = obj.grass[i].GetComponent<AttachToPlanet>();
            attach.angle = (obj.grassSpacing * i) + obj.grassRotationOffset;
            attach.Attach();
        }
    }
}
