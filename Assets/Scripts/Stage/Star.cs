using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Star : MonoBehaviour
{
    [Header("Values")]
    [SerializeField] int health;
    [SerializeField] float starRadius = 1f;

    [Header("References")]
    [SerializeField] CircleCollider2D mainCollider;
    [SerializeField] SpriteRenderer outlineVisualComponent;
    [SerializeField] SpriteRenderer insideVisualComponent;

    private void OnValidate() 
    {
        UpdateAttributes();
    }

    private void UpdateAttributes()
    {
        if (mainCollider)
            mainCollider.radius = starRadius;

        if (outlineVisualComponent)
            outlineVisualComponent.transform.localScale = Vector3.one * starRadius * 2f;

        if (insideVisualComponent)
            insideVisualComponent.transform.localScale = outlineVisualComponent.transform.localScale - (Vector3.one * .04f);
    }

    public void Collect (Collectable collectable)
    {
        TakeHealth();

        collectable.gameObject.SetActive(false);
    }

    public void TakeHealth()
    {
        health--;

        if (health < 1)
        {
            gameObject.SetActive(false);
        }
    }
}
