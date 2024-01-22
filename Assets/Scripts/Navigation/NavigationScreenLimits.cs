using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationScreenLimits : MonoBehaviour
{
    [SerializeField] Vector2 size;

    [Header("References")]
    [SerializeField] PolygonCollider2D cameraCollider;
    [SerializeField] BoxCollider2D playerCollider;

    void Awake()
    {
        UpdateColliders();
    }

    private void OnValidate() 
    {
        UpdateColliders();
    }

    private void UpdateColliders()
    {
        if (cameraCollider)
        {
            Vector2[] points = 
            {
                new Vector2( size.x/2,  size.y/2 ),
                new Vector2( size.x/2, -size.y/2 ),
                new Vector2(-size.x/2, -size.y/2 ),
                new Vector2(-size.x/2,  size.y/2 )
            };
            cameraCollider.points = points;
        }

        if (playerCollider)
        {
            playerCollider.size = size;
        }
    }

    private void OnDrawGizmos() 
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere (transform.position + new Vector3( size.x/2,  size.y/2), radius: .1f);
        Gizmos.DrawWireSphere (transform.position + new Vector3(-size.x/2,  size.y/2), radius: .1f);
        Gizmos.DrawWireSphere (transform.position + new Vector3( size.x/2, -size.y/2), radius: .1f);
        Gizmos.DrawWireSphere (transform.position + new Vector3(-size.x/2, -size.y/2), radius: .1f);

        
        ExtDebug.DrawBox(transform.position, size/2f, Quaternion.identity, Color.blue);
    }
}
