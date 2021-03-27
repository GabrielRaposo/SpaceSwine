using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SidewaysCharacter : MonoBehaviour
{
    protected bool facingRight;

    protected virtual void SetFacingRight(bool value)
    {
        facingRight = value;
    }
}
