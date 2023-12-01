using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spikes : MonoBehaviour
{
    public float damage = 3;

    void OnTriggerEnter2D(Collider2D coll)
    {
        CharacterController2D cc = coll.GetComponent<CharacterController2D>();

        if (cc == null) return;

        cc.onGotHit?.Invoke(damage);
    }
}
