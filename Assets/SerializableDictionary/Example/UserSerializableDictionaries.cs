using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public class LanguageToString : SerializableDictionary<GameLocalizationCode, string> { }

[Serializable]
public class CodeToDictionary : SerializableDictionary<string, LanguageToString> { }