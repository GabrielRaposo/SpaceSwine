using System.Collections;
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
    }

    //private void OnDrawGizmos() 
    //{
    //    if (gravityRadius <= 0)
    //        return;

    //    Gizmos.color = new Color(1, 1, 1, .25f);
    //    Gizmos.DrawWireSphere(transform.position, planetRadius + gravityRadius);
    //}
}
