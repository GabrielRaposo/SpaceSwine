using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RedBlueGames.Tools.TextTyper;

public class PlaySoundOnType : MonoBehaviour
{
    [SerializeField] AK.Wwise.Event voiceAkEvent;
    [SerializeField] TextTyper typer;

    void Start()
    {
        if (!typer)
            typer = GetComponentInChildren<TextTyper>();

        if (!typer || voiceAkEvent == null) 
        {
            enabled = false;
            return;
        }

    }

    private void Update() 
    {
        if (!typer.IsTyping || voiceAkEvent.IsPlaying(gameObject))
            return;

        voiceAkEvent.Post(gameObject);
    }

}
