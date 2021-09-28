using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Classe responsável por conversar com interações externas com controladores e managers
public class PlayerCharacter : MonoBehaviour
{
    public static PlayerCharacter Instance;

    private void Awake() 
    {
        if (Instance) 
        {
            gameObject.SetActive(false);
            Debug.Log("Duplicate player instance found.");
            return;
        }

        Instance = this;
    }

    public void ResetStates()
    {

    }

    public void SpawnAt (Vector2 position, float rotation = 0)
    {
        ResetStates();

        //Spawn state

        transform.position = position;
        transform.eulerAngles = rotation * Vector3.forward;
    }
}
