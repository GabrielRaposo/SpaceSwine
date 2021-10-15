using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Jumper 
{
    public class MJ_ScoreLine : MonoBehaviour
    {
        [SerializeField] TextMeshPro display;    

        public void SetValue (int value, string text)
        {
            if (display)
                display.text = text;

            transform.position = new Vector2 (transform.position.x, value);
        }

    }
}
