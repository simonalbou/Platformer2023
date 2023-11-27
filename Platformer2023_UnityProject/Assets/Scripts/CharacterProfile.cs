using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterProfile : ScriptableObject
{
    // valeurs à équilibrer
    [Header("Parameters")]
    public float speed = 5;
    public float gravity = -9.81f;
    public float jumpStrength = 10;
    [Tooltip("Used for double-jumps, triple-jumps, or more.")]
    public int numberOfJumps = 2;
    [Tooltip("Acts as a gravity multiplier during jump.")]
    public AnimationCurve jumpCurve;
}
