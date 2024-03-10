using Shooter;
using System.Collections;
using System.Collections.Generic;
using Traveler;
using UnityEngine;

public class MT_Collectable : MonoBehaviour
{
    [SerializeField] int score;
    [SerializeField] float cooldownToBlink;
    [SerializeField] float blinkDuration;
    [SerializeField] MS_DettachableEffect dettachableEffect;

    Animator animator;

    float t;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void Spawn (Vector2 position)
    {
        animator = GetComponent<Animator>();
        animator.SetTrigger("Reset");
        animator.SetBool("OnTransition", false);

        transform.localPosition = position;
        gameObject.SetActive(true);

        t = 0;
    }

    bool IsOnCooldownBlink => t > cooldownToBlink;

    private void Update()
    {
        if (!MT_Player.HasMoved)
            return;

        if (MT_Player.HasLost)
        {
            animator.enabled = false;
            return;
        }

        t += Time.deltaTime;

        if (IsOnCooldownBlink)
            animator.SetBool("OnTransition", true);

        if (t > cooldownToBlink + blinkDuration)
            SelfDestruct();
    }

    private void SelfDestruct ()
    {
        gameObject.SetActive(false);
        MT_Bullet bullet = MT_BulletPool.Instance.GetAimBullet();

        Vector2 direction = Vector2.up;
        if (MT_Player.Instance)
            direction = (MT_Player.Instance.transform.position - transform.position).normalized;
        
        bullet.Shoot (direction * 3f, transform.position);
    }

    public void OnCollect()
    {
        if (dettachableEffect)
            dettachableEffect.Call();

        MT_ScoreManager.Instance.ChangeScore( (int)(score * (IsOnCooldownBlink ? 1.5f : 1f)) );
        gameObject.SetActive(false);
    }
}
