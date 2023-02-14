using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParsedLocalizedText : LocalizedText
{
    public string tagSeparator;

    protected override string ParsedText(string text) 
    {
        return ParseInputTag.ParsedOutput (text, tagSeparator);
    }
}
