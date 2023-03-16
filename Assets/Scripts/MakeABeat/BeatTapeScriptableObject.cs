using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BeatTape_", menuName = "ScriptableObjects/BeatTape")]
public class BeatTapeScriptableObject : ScriptableObject
{
    public string title;
    public bool silent;
    public AK.Wwise.Event sampleAKEvent;
    public int signatureDuration;
    public Sprite frontalSprite;
    public Sprite sidewaysSprite;
}
