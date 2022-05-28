using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecorationSpriteRandomizer : MonoBehaviour
{
    [SerializeField] bool randomized;
    [SerializeField] List<Sprite> options;
    [SerializeField] SpriteRenderer sr;

    void Start()
    {
        Randomize();
    }

    private void OnValidate() 
    {
        Randomize(); 
    }

    private void Randomize() 
    {
        if (randomized)
            return;

        if (options.Count < 1 || !sr)
            return;

        if (Random.Range(0,2) == 0)
            sr.flipX = !sr.flipX;
        sr.sprite = options[ Random.Range(0, options.Count - 1) ];
            
        randomized = true;
    }

}
