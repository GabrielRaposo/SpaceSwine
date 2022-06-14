using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [SerializeField] int startingValue;
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

        TakeDamage(hitbox.damage);
    }

    private void TakeDamage(int damage)
    {
        Debug.Log($"<color=#dd0000><b>Player Take damage</b></color>");
        value -= damage;

        if (value < 0)
            value = 0;
        
        if (value == 0)
            Die(deathFromDamage: true);
    }

    public void Die (bool deathFromDamage)
    {
        deathAKEvent?.Post(gameObject);
        //gameObject.SetActive(false);

        Vector2 pos = ((Vector2) transform.position)
            .Remap(new Vector2(-10f, -6f), new Vector2(10f, 6f),Vector2.zero, Vector2.one);
        
        //CameraShaderManager.Instance.StartDeathSentence(pos, OnDeathEvent);
        OnDeathEvent.Invoke(deathFromDamage);
    }
}
