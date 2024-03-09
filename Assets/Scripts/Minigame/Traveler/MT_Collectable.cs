using System.Collections;
using System.Collections.Generic;
using Traveler;
using UnityEngine;

public class MT_Collectable : MonoBehaviour
{
    [SerializeField] int score;
    [SerializeField] float cooldownToBlink;
    [SerializeField] float blinkDuration;

    Animator animator;

    float t;

    public void Spawn (Vector2 position)
    {
        animator = GetComponent<Animator>();
        animator.SetTrigger("Reset");
        animator.SetBool("OnTransition", false);

        transform.localPosition = position;
        gameObject.SetActive(true);

        t = 0;
    }

    private void Update()
    {
        t += Time.deltaTime;

        if (t > cooldownToBlink)
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
        MT_CollectableSpawner.AddScore(score);
        gameObject.SetActive(false);
    }
}
