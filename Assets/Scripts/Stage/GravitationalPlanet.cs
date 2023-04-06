using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class GravitationalPlanet : GravitationalBody
{
    [Header("Values")]
    [SerializeField] float planetRadius  = 1f;
    [SerializeField] float gravityRadius = 1f;
    [SerializeField] bool useCustomRadiusSize;

    [Header("References")]
    [SerializeField] CircleCollider2D mainCollider;
    [SerializeField] SpriteRenderer outlineVisualComponent;
    [SerializeField] SpriteRenderer insideVisualComponent;
    [SerializeField] CircleCollider2D gravityArea;

    private PlanetMaterialCreator materialCreator;
    
    private void OnValidate()
    {
        if (materialCreator == null)
            materialCreator = GetComponent<PlanetMaterialCreator>();
        
        if(materialCreator)
            materialCreator.SetPixelSize();
        
        UpdateAttributes();
    }

    public (float planetRadius, float gravityRadius) GetAttributes()
    {
        return (planetRadius, gravityRadius);
    }

    private void UpdateAttributes()
    {
        if (mainCollider)
            mainCollider.radius = planetRadius;

        if (outlineVisualComponent)
            outlineVisualComponent.transform.localScale = Vector3.one * planetRadius * 2f;

        if (insideVisualComponent)
        {
            insideVisualComponent.transform.localScale = outlineVisualComponent.transform.localScale - (Vector3.one * .04f);
            if (useCustomRadiusSize)
            {
                insideVisualComponent.transform.localScale *= 1.15f;
            }
        }

        if (gravityArea && gravityRadius > 0f)
            gravityArea.radius = planetRadius + gravityRadius;
    }

    private void OnDrawGizmos() 
    {
        if (gravityRadius <= 0)
            return;

        Gizmos.color = new Color(1, 1, 1, .25f);
        Gizmos.DrawWireSphere(transform.position, planetRadius + gravityRadius);
    }

}
