using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentGroup : MonoBehaviour
{
    [SerializeField] AK.Wwise.Event bgmAKEvent;

    static PersistentGroup Instance;

    private void Awake() 
    {
        if (Instance)
        {
            Destroy( gameObject );
            return;
        }

        DontDestroyOnLoad(gameObject);
        Instance = this;
    }

    private void Start() 
    {
        //bgmAKEvent?.Post(gameObject);    
    }
}
