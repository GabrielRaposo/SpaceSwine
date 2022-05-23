using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarperGroup : MonoBehaviour
{
    void Start()
    {
        Warper[] children = GetComponentsInChildren<Warper>();
        if (children.Length < 2)
            return;

        for (int i = 0; i < children.Length; i++)
        {
            int next = (i + 1) % children.Length;
            children[i].Setup(children[next]);
        }
    }
}
