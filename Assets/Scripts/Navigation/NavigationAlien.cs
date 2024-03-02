using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationAlien : MonoBehaviour
{
    [SerializeField] SpriteRenderer baseSprite;

    [SerializeField] Gradient selectedGradient;
    [SerializeField] Gradient deselectedGradient;
    [SerializeField] float gradientDuration;

    float t;
    bool highlighted;

    bool Highlighted
    {
        get { return highlighted; }
        set { highlighted = value; UpdateColor(); }
    }

    void Start()
    {
        SetSceneNavigationObject navObject = GetComponentInParent<SetSceneNavigationObject>();
        
        if (navObject == null)
            return;

        navObject.OnSelectAction   += () => Highlighted = true;
        navObject.OnDeselectAction += () => Highlighted = false;

        UpdateColor();
    }

    void Update()
    {
        UpdateColor();

        t += Time.unscaledDeltaTime;

        if (t > gradientDuration)
            t = 0;
    }

    private void UpdateColor() 
    {
        Gradient gradient = Highlighted ? selectedGradient : deselectedGradient;
        baseSprite.color = gradient.Evaluate(t / gradientDuration);
    }
}
