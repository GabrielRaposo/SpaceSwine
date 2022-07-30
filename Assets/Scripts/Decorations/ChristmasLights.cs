using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChristmasLights : MonoBehaviour
{
    [Header("Values")]
    [SerializeField] float startDelay;
    [SerializeField] float delayBetweenSteps;
    
    [Header("References")]
    [SerializeField] TitleMenuNavigation titleMenuNavigation;
    [SerializeField] List<Animator> lights;

    void Start()
    {
        if (!titleMenuNavigation)
            return;

        if (lights.Count < 1)
            return;

        for (int i = 0; i < lights.Count; i++)
        {
            lights[i].Play("ColoredLght-Idle", layer: 0, normalizedTime: Random.Range(0.0f, 1.0f) );
        }

        titleMenuNavigation.OnEnterMenuEvent += () => 
        {
            StopAllCoroutines();
            StartCoroutine(FlailLights());
        };
    }

    private IEnumerator FlailLights()
    {
        yield return new WaitForSeconds(startDelay);

        for (int i = 0; i < lights.Count; i++)
        {
            float modifier = Random.Range(-50f, 50f) / 1000f;
            yield return new WaitForSeconds(modifier);

            int local = i;
            lights[local].SetTrigger("Flail");

            yield return new WaitForSeconds(delayBetweenSteps);
        }
    }
}
