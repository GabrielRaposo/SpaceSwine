using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawnPortal : MonoBehaviour
{
    [SerializeField] ParticleSystem spawnParticle;
    
    Transform player;
    Animator animator;

    private void Awake() 
    {
        animator = GetComponent<Animator>();

        if (transform.parent == null)
            return;

        player = transform.parent;
        transform.SetParent(null);
    }

    public void Call()
    {
        transform.position = player.position;
        animator.SetTrigger("Activate");

        if (spawnParticle)
        {
            if (spawnParticle.isPlaying)
                spawnParticle.Clear();

            spawnParticle.Play();
        }
    }
}
