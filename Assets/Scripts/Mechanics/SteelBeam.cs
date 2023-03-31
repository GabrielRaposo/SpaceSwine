using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteelBeam : MonoBehaviour
{
    const float PLATFORM_OFFSET = .11f;

    [SerializeField] float length;

    [Header("References")]
    [SerializeField] CustomRotate customRotate;
    [SerializeField] SpriteRenderer visualComponent;
    [SerializeField] Transform colliders;
    [SerializeField] Transform leftHitbox;
    [SerializeField] Transform rightHitbox;
    [SerializeField] List<GravitationalPlatform> platforms;

    private void OnValidate() 
    {
        UpdateAttributes();
    }

    private void UpdateAttributes()
    {
        if (length <= 0)
            return;

        if (visualComponent)
            visualComponent.size = new Vector2(length, .44f);

        if (leftHitbox)
            leftHitbox.localPosition  = Vector2.left * length * .5f;

        if (rightHitbox)
            rightHitbox.localPosition = Vector2.right * length * .5f;

        foreach (GravitationalPlatform platform in platforms)
            platform.UpdateLength(length - PLATFORM_OFFSET);
    }

    void Start()
    {
        if (colliders != null)
        {
            SpriteRenderer[] spriteRenderers = colliders.GetComponentsInChildren<SpriteRenderer>();
            foreach (var sr in spriteRenderers)
                sr.enabled = false;
        }

        UpdateAttributes();
    }

    private void OnDrawGizmos() 
    {
        if (!customRotate || !customRotate.enabled)
            return;

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, (length / 2f ) + .1f);
    }
}
