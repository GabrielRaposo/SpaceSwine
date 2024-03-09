using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MT_Collectable : MonoBehaviour
{
    [SerializeField] int score;

    public void Spawn (Vector2 position)
    {
        transform.localPosition = position;
        gameObject.SetActive(true);
    }

    public void OnCollect()
    {
        MT_CollectableSpawner.AddScore(score);
        gameObject.SetActive(false);
    }
}
