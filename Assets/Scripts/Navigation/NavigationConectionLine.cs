using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationConectionLine : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] SpriteRenderer mainRenderer;
    [SerializeField] Gradient idleGradient;
    [SerializeField] float gradientDuration;
    [SerializeField] Color overrideColor = Color.clear;
    
    float t;

    void OnValidate()
    {
        if (target == null)
            return;

        float distance = Vector2.Distance (transform.position, target.position);
        mainRenderer.size = new Vector2(distance, mainRenderer.size.y);

        float angle = Vector2.SignedAngle(Vector2.left, (Vector2)(target.position - transform.position));
        transform.eulerAngles = Vector3.forward * angle;
    }

    private void OnEnable()
    {
        mainRenderer.enabled = false;
        
        this.Wait (frames:2, action: () => 
        {
            if (target == null || !target.gameObject.activeSelf)
            {
                gameObject.SetActive (false);
                return;
            }

            if (overrideColor == Color.clear)
            {
                NavigationObject parentPlanet = GetComponentInParent<NavigationObject>();
                if (parentPlanet != null)
                {
                    mainRenderer.color = parentPlanet.GetSpriteColor;
                }
            } else mainRenderer.color = overrideColor;

            mainRenderer.enabled = true;            
        });
    }

    private void Update()
    {
        if (!mainRenderer.enabled)
            return;

        Color c = mainRenderer.color;
        c.a = idleGradient.Evaluate(t / gradientDuration).a;
        mainRenderer.color = c;

        t += Time.deltaTime;

        if (t > gradientDuration)
            t = 0;
    }
}
