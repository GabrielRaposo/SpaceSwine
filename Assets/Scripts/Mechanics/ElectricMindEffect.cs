using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricMindEffect : MonoBehaviour
{
    const float SPRITE_LENGHT = 1f;    

    Transform start, end;

    float t;
    float cooldown;
    float animationDuration;

    bool active;

    Animator animator;

    private void Awake() 
    {
        animator = GetComponent<Animator>();
    }

    public void Setup (Transform start, Transform end)
    {
        this.start = start;
        this.end   = end;

        t = 0;
        RandomizeState();
        RandomizeCooldown();
    }

    private void RandomizeState()
    {
        int index = 0;
        //index = Random.Range(0, 3);
        animator.SetInteger("State", index);
        animationDuration = GetClipDuration(index);
    }

    private float GetClipDuration (int index)
    {
        AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
        string clipName = "Extra-Lightning-" + index;

        foreach (AnimationClip clip in clips)
        {
            if (clip.name == clipName)
                return clip.length;
        }

        return 0;
    }

    private void RandomizeCooldown()
    {
        cooldown = 1f;
    }

    void Update()
    {
        Vector3 direction = end.position - start.position;
        transform.position = start.position + (direction / 2f);
        transform.eulerAngles = Vector3.forward * Vector2.SignedAngle(Vector2.right, direction.normalized);

        t += Time.deltaTime;

        if (active)
        {
            float length = SPRITE_LENGHT / 2f;
            transform.position = Vector3.Lerp
            (
                start.position + direction.normalized * length, 
                end.position   - direction.normalized * length, 
                t / animationDuration
            );

            if (t < animationDuration)
                return;

            active = false; 
            Debug.Log("Switch off");    
        }
        else
        {
            if (t < cooldown)
                return;
            
            t = 0;
            RandomizeState();
            RandomizeCooldown();

            animator.SetTrigger("Activate");

            active = true;
            Debug.Log("Switch on");
        }
    }
}
