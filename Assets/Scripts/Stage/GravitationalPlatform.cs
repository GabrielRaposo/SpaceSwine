﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravitationalPlatform : GravitationalBody
{
    [Header("Values")]
    [SerializeField] Vector2 platformSize;
    [SerializeField] float gravityHeight = 1f;

    [Header("References")]
    [SerializeField] BoxCollider2D mainCollider;
    [SerializeField] SpriteRenderer outlineVisualComponent;
    [SerializeField] SpriteRenderer insideVisualComponent;
    [SerializeField] BoxCollider2D gravityArea;
    [SerializeField] GameObject leftWall;
    [SerializeField] GameObject rightWall;

    private void OnValidate() 
    {
        UpdateAttributes();
    }

    //public (float planetRadius, float gravityRadius) GetAttributes()
    //{
    //    return (planetRadius, gravityRadius);
    //}

    private void UpdateAttributes() 
    {
        if (mainCollider)
            mainCollider.size = platformSize;

        if (outlineVisualComponent)
            outlineVisualComponent.transform.localScale = Vector3.one * platformSize;

        if (insideVisualComponent)
            insideVisualComponent.transform.localScale = outlineVisualComponent.transform.localScale - (Vector3.one * .04f);

        if (gravityArea && gravityHeight > 0f)
        {
            gravityArea.size = new Vector2( platformSize.x, gravityHeight );
            gravityArea.offset = Vector2.up * gravityHeight * .5f;
        }

        if (leftWall)
        {
            leftWall.transform.localPosition = new Vector2 ((platformSize.x * -.5f) - .4f, .5f * gravityHeight);
            leftWall.transform.localScale = new Vector2(.25f, gravityHeight);
        }

        if (rightWall)
        {
            rightWall.transform.localPosition = new Vector2 ((platformSize.x * .5f) + .4f, .5f * gravityHeight);
            rightWall.transform.localScale = new Vector2(.25f, gravityHeight);
        }
    }

    private void OnDrawGizmosSelected() 
    {
        if (gravityHeight <= 0)
            return;

        //Gizmos.color = new Color(1, 1, 1, .25f);
        Gizmos.color = Color.yellow;
        Vector3 upperOffset = (transform.up * gravityHeight * 3);
        Vector3 leftOffset = transform.position + (transform.right * platformSize.x * .5f * -1f);
        Vector3 rightOffset = transform.position + (transform.right * platformSize.x * .5f *  1f);

        Gizmos.DrawLine( leftOffset,  leftOffset  + upperOffset );
        Gizmos.DrawLine( rightOffset, rightOffset + upperOffset );
    }
}
