using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextCounter : MonoBehaviour
{
    public TextMeshProUGUI textComponent;

    public Transform frogPosition;

    // Update is called once per frame
    void Update()
    {
        textComponent.text = "La <b>position du personnage est: \n<#ff0>"+frogPosition.position.x.ToString()+"</color>";
    }
}
