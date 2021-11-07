using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [SerializeField] int startingValue;
    [SerializeField] AK.Wwise.Event deathAKEvent;

    int value;

    public UnityAction OnDeathEvent;

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

        TakeDamage(hitbox.damage);
    }

    private void TakeDamage(int damage)
    {
        value -= damage;

        if (value < 0)
            value = 0;
        
        if (value == 0)
            Die();
    }

    public void Die()
    {
        deathAKEvent?.Post(gameObject);
        gameObject.SetActive(false);
        OnDeathEvent?.Invoke();
    }
}
