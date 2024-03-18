using Shooter;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MS_TimeBonus : MonoBehaviour
{
    [SerializeField] MS_DettachableEffect onCollectEffect;
    [SerializeField] SpriteSwapper bonusSpriteSwapper;
    [SerializeField] AK.Wwise.Event OnCollectAKEvent;

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
        int time = GetTime();

        MS_StageTimer.AddTime(time);
        //Debug.Log("time: " + time);

        if (bonusSpriteSwapper != null)
            bonusSpriteSwapper.SetSpriteState(time - 1);

        if (onCollectEffect != null)
            onCollectEffect.Call();

        if (OnCollectAKEvent != null)
            OnCollectAKEvent.Post(gameObject);

        gameObject.SetActive(false);
    }

    private int GetTime()
    {
        int level = MS_SessionManager.Instance.GetLevel();

        if (level < 4)
            return 3;

        if (level < 6)
            return 2;

        return 1;
    }
}
