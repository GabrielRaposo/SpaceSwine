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

            { 
            //string n = transform.GetChild(i).name;
            //if (n.Contains(")")) 
            //{
            //    string[] splits = n.Split(')');
            //    if (splits.Length > 1 && splits[1].Trim() != string.Empty) {
            //        finalName += splits[1];
            //    }
            //}
            }

            string oldName = transform.GetChild(i).name;
            if (oldName.Contains("Round (") && oldName.Length >= finalName.Length)
            {
                char[] splits = oldName.ToCharArray();

                int endIndex = finalName.Length;
                for (int a = 0; a < oldName.Length; a++)
                {
                    if (oldName[a] == ')')
                    {
                        endIndex = a + 2;
                        break;
                    }
                }

                oldName = string.Empty;
                for (int b = endIndex; b < splits.Length; b++)
                {
                    oldName += splits[b];
                }

                transform.GetChild(i).name = finalName + " " + oldName;
                continue;
            }

            transform.GetChild(i).name = finalName + " " + transform.GetChild(i).name;
        }
    }
}

