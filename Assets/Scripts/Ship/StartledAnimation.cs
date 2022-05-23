using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartledAnimation : MonoBehaviour
{
    [SerializeReference] SpriteRenderer leftPhone;
    [SerializeReference] SpriteRenderer rightPhone;

    int sortingLayerLeft;
    int sortingLayerRight;

    int sortingOrderLeft;
    int sortingOrderRight;

    void Start()
    {
        if (!leftPhone && !rightPhone)
            return;

        sortingLayerLeft  = leftPhone.sortingLayerID;
        sortingLayerRight = rightPhone.sortingLayerID;

        sortingOrderLeft  = leftPhone.sortingOrder;
        sortingOrderRight = rightPhone.sortingOrder;
    }

    public void LowerSortingLayer()
    {
        leftPhone.sortingLayerID  = 0;
        rightPhone.sortingLayerID = 0;

        leftPhone.sortingOrder  = -50;
        rightPhone.sortingOrder = -50; 
    }

    public void RestoreBasePrintOrder()
    {
        leftPhone.sortingLayerID  = sortingLayerLeft;    
        rightPhone.sortingLayerID = sortingLayerRight;

        leftPhone.sortingOrder  = sortingOrderLeft;
        rightPhone.sortingOrder = sortingOrderRight; 
    }
}
