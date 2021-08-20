using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderTab : MonoBehaviour
{
    Slider slider;

    void Start()
    {
        slider = GetComponentInChildren<Slider>();
    }

    public void ChangeValue (float value)
    {
        int direction = value > 0 ? 1 : -1;
        slider.value += direction * .1f;
    }
}
