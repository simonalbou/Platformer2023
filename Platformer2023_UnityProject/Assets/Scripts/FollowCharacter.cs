using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCharacter : MonoBehaviour
{
    public Transform selfTransform, characterTransform;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Update()
    {
        selfTransform.position = new Vector3(
            characterTransform.position.x,
            selfTransform.position.y,
            selfTransform.position.z
        );
    }
}
