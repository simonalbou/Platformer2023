using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScrollViewTester : MonoBehaviour
{
    public TextMeshProUGUI text;

    public void PrintScrollValues(Vector2 vec)
    {
        text.text = vec.ToString();
    }
}
