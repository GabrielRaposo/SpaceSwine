using Shooter;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MS_TimeBonus : MonoBehaviour
{
    [SerializeField] MS_DettachableEffect onCollectEffect;

    private void Start()
    {
        MS_Session session = GetComponentInParent<MS_Session>();
        if (session) 
        {
            session.OnReset += () => gameObject.SetActive(true);
        }
    }

    private void Update()
    {
        transform.eulerAngles = Vector3.zero;
    }

    public void Collect()
    {
        if (onCollectEffect != null)
            onCollectEffect.Call();

        MS_StageTimer.AddTime(1);

        gameObject.SetActive(false);
    }
}
