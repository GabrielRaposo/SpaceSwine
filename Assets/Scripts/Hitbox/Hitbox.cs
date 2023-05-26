using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DamageType { Spike, Electricity };

public class Hitbox : MonoBehaviour
{
    public int damage = 1;
    public DamageType damageType;
}
