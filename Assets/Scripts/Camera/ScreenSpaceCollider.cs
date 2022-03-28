using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenSpaceCollider : MonoBehaviour
{
    [SerializeField] Vector2 baseRatio;
    [SerializeField] float offset;

    public void SetSize (float value)
    {
        float modifiedValue = value / 3.5f;
        modifiedValue /= 5.5f;

        BoxCollider2D box = GetComponent<BoxCollider2D>();
        box.size = (baseRatio * modifiedValue) + (Vector2.one * offset);
    }
}
