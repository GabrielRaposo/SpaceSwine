using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "AstroPigLocalizationFile", menuName = "Localization/AstroPigLocalizationFile")]
[System.Serializable]
public class AstroPigLocalizationFile : ScriptableObject
{
    public CodeToDictionary dic;
}
