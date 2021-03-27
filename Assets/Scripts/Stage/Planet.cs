using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class Planet : MonoBehaviour
{
    [Header("Values")]
    [SerializeField] float planetRadius = 1f;
    [SerializeField] float gravityRadius = 1f;

    [Header("References")]
    [SerializeField] CircleCollider2D mainCollider;
    [SerializeField] SpriteRenderer visualComponent;
    [SerializeField] CircleCollider2D gravityArea;

    void Start()
    {
    }

    void Update()
    {
        
    }

    private void OnValidate() 
    {
        UpdateAttributes();
    }

    private void UpdateAttributes()
    {
        if (mainCollider)
            mainCollider.radius = planetRadius;

        if (visualComponent)
            visualComponent.transform.localScale = Vector3.one * planetRadius * 2f;

        if (gravityArea && gravityRadius > 0f)
            gravityArea.radius = planetRadius + gravityRadius;
    }

    private void OnDrawGizmos() 
    {
        if (gravityRadius <= 0)
            return;

        Gizmos.color = new Color(1,1,1,.25f);
        Gizmos.DrawWireSphere(transform.position, planetRadius + gravityRadius);
    }
}
