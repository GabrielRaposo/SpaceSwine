﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PagerInteractable : MonoBehaviour
{
    public virtual void Select() { }

    public virtual void Deselect() { }

    public virtual void OnClick() { }

    public virtual bool OnHorizontalInput(float direction) { return false; }
}
