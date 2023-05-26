using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [SerializeField] int startingValue;
    [SerializeField] ParticleSystem damageFX;
    [SerializeField] AK.Wwise.Event deathAKEvent;

    int value;

    public UnityAction<bool> OnDeathEvent;

    void Start()
    {
        value = startingValue;
    }

    private void OnTriggerEnter2D (Collider2D collision) 
    {
        if (!collision.CompareTag("Hitbox"))
            return;

        Hitbox hitbox = collision.GetComponent<Hitbox>();
        if (!hitbox)
            return;

        Vector2 point = transform.position;

        RaycastHit2D[] hit = Physics2D.CircleCastAll(transform.position, .5f, Vector2.zero);
        if (hit.Length > 0 )
        {
            foreach(RaycastHit2D h in hit)
            {
                Hitbox testBox = h.collider.GetComponent<Hitbox>();
                if (testBox)
                {
                    //Debug.Log (h.point);
                    point = h.point;
                    break;
                }
            }
        }

        TakeDamage(hitbox.damage, point);
    }

    private void TakeDamage(int damage, Vector2 damagePosition)
    {
        Debug.Log($"<color=#dd0000><b>Player Take damage</b></color>");
        if (damageFX)
        {
            damageFX.transform.position = damagePosition;
            damageFX.Play();
        }
        value -= damage;

        if (value < 0)
            value = 0;
        
        if (value == 0)
            Die (deathFromDamage: true);
    }

    public void Die (bool deathFromDamage)
    {
        deathAKEvent?.Post(gameObject);
        //gameObject.SetActive(false);

        //Vector2 pos = ((Vector2) transform.position)
        //    .Remap(new Vector2(-10f, -6f), new Vector2(10f, 6f),Vector2.zero, Vector2.one);
        //CameraShaderManager.Instance.StartDeathSentence(pos, OnDeathEvent);

        OnDeathEvent?.Invoke(deathFromDamage);
    }
}
