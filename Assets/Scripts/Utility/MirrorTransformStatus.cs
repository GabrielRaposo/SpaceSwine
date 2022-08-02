using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorTransformStatus : MonoBehaviour
{
    [SerializeField] Transform target;

    void Start()
    {
        if (target == null)
        {
            enabled = false;
            return;
        }

        MirrorTarget();
    }

    void Update()
    {
        if (!target.hasChanged)
            return;

        if (!target.gameObject.activeInHierarchy)
            gameObject.SetActive(false);

        MirrorTarget();
    }

    private void MirrorTarget()
    {
        transform.position = target.position;
        transform.rotation = target.rotation;        
        //transform.localScale = target.localScale;
    }
}
