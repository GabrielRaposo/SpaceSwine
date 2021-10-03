using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenameRoundsByOrder : MonoBehaviour
{
    public int world;

    public void UpdateNames()
    {
        int index = 0;
        for (int i = 0; i < transform.childCount; i++)
        {
            string stageIndex = string.Empty;
            
            stageIndex = index.ToString();
            index++;

            string finalName = "Round (" + world + "-" + stageIndex + ")";
            
            //string n = transform.GetChild(i).name;
            //if (n.Contains(")"))
            //{
            //    string[] splits = n.Split(')');
            //    if (splits.Length > 1 && splits[1].Trim() != string.Empty)
            //    {
            //        finalName += splits[1];
            //    }
            //}

            transform.GetChild(i).name = finalName;
        }
    }
}

