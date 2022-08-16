using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RedBlueGames.Tools.TextTyper;

public class PlaySoundOnType : MonoBehaviour
{
    [SerializeField] AK.Wwise.Event voiceAkEvent;
    [SerializeField] TextTyper typer;

    bool mute;

    List<string> silentCharacters = new List<string>() { string.Empty, " ", ",", ".", "!", "?", "\"", "'"}; 

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

    public void SetVoiceEvent(AK.Wwise.Event newEvent)
    {
        voiceAkEvent = newEvent;
    }

    public void TypeSound(string s)
    {
        if (mute)
            return;

        if (voiceAkEvent.IsPlaying(gameObject))
            return;

        if (silentCharacters.Contains(s))
            return;

        voiceAkEvent.Post(gameObject);
    }

    public void SetMute (bool value)
    {
        mute = value;
    }
}
