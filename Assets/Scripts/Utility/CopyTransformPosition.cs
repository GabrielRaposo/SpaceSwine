using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyTransformPosition : MonoBehaviour
{
    [SerializeField] Transform target;

    void Update()
    {
        if (!target)
            return;

        transform.position = target.position;
    }
}
