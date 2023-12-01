using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UI;

public class CustomTestButton : MonoBehaviour
{
    public Image image;
    public TextMeshProUGUI text;

    public Color hoverEnterColor = Color.green;
    public Color hoverExitColor = Color.blue;
    public float transitionDuration = 0.5f;
    public AnimationCurve transitionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    // déclaration rapide de fonction
    public void SetHoverEnterColor() => StartCoroutine(HandleColorTransition(hoverEnterColor));
    public void SetHoverExitColor() => StartCoroutine(HandleColorTransition(hoverExitColor));
    public Color GetButtonColor() => image.color;

    IEnumerator Start()
    {
        int i = 0;

        yield return new WaitForSeconds(0.5f);

        image.color = Color.yellow;
        text.color = Color.black;

        yield return new WaitForSeconds(1.5f);

        text.text = "It's been 2 seconds";

        yield return new WaitForSeconds(1f);
        
        while (true)
        {
            i++;
            text.text = i.ToString();
            yield return new WaitForSeconds(0.5f);
        }
    }

    // Méthode #1 : transition linéaire, utilisant l'update, sans coroutine
    // variables d'aide pour la transition sans coroutine
    // bool isPlayingTransition;
    // Color startColor, endColor;
    // float timeSinceTransitionStarted;
    /**
    public void UpdateTransition()
    {
        if (isPlayingTransition)
        {
            timeSinceTransitionStarted += Time.deltaTime;
            float ratio = timeSinceTransitionStarted / transitionDuration;
            if (ratio >= 1)
                isPlayingTransition = false;

            image.color = Color.Lerp(startColor, endColor, transitionCurve.Evaluate(ratio));
        }
    }

    public void SetColorTransition(Color targetColor)
    {
        startColor = GetButtonColor();
        endColor = targetColor;
        isPlayingTransition = true;
        timeSinceTransitionStarted = 0;
    }
    /**/

    // Méthode #2 : Coroutine
    public IEnumerator HandleColorTransition(Color targetColor)
    {
        // début de la transition
        Color initialColor = GetButtonColor();
        float transitionTimer = 0;

        // le milieu de la fonction s'exécute en plusieurs frames
        while (transitionTimer < transitionDuration)
        {
            transitionTimer += Time.deltaTime;
            image.color = Color.Lerp(initialColor, targetColor, transitionCurve.Evaluate(transitionTimer / transitionDuration));
            yield return new WaitForEndOfFrame();
        }
    }

    public void PrintSomething(string msg)
    {
        Debug.Log(msg);
    }

    public void SetText(string newText)
    {
        text.text = newText;
    }
}
