using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    [SerializeField] float ratio = 1;
    [SerializeField] Transform followTarget;

    void Start()
    {
        if (!followTarget)
        {
            enabled = false;
            return;
        }
    }

    void Update()
    {
        float localRatio = 0.1f * ratio;
        transform.localPosition = followTarget.position * localRatio * -1f;
    }
}
