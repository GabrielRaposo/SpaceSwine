using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecorationPlacer : MonoBehaviour
{
    public GameObject grassReference;
    public List<GameObject> miscObjectsReference;

    public List<GameObject> grass;
    public List<GameObject> miscObjects;

    [Range(0,360)]public float grassRotationOffset;
    public int grassAmount;
    public float grassSpacing;
    public float grassHeightOffset;

}
