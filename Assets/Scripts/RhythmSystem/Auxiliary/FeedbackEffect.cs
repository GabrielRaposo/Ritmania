using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using RhythmSystem;

public class FeedbackEffect : MonoBehaviour
{
    public float duration;
    public Vector2 movement;

    float t;
    Vector2 spawnPoint;
    TextMeshPro display;

    public void Setup (PrecisionScore precisionScore, Vector2 spawnPoint)
    {
        display = GetComponent<TextMeshPro>();
        if (!display)
        {
            gameObject.SetActive(false);
            return;
        }

        switch (precisionScore) 
        {
            default:
            case PrecisionScore.Perfect:
                display.text = "<color=yellow>Perfect";
                break;

            case PrecisionScore.Good:
                display.text = "<color=blue>Good";
                break;

            case PrecisionScore.Bad:
                display.text = "<color=grey>Bad";
                break;

            case PrecisionScore.Miss:
                display.text = "<color=red>Miss";
                break;

        }

        this.spawnPoint = spawnPoint;
        transform.position = spawnPoint;
        display.alpha = 1;
        t = 0;

        gameObject.SetActive(true);
    }

    private void Update() 
    {
        // Faz Lerp de posição
        transform.position = Vector2.Lerp ( 
            spawnPoint,
            spawnPoint + movement,
            t / duration
        );

        // Faz Lerp de transparência
        if (display)
            display.alpha = Mathf.Lerp(1, 0, t / duration);

        t += Time.deltaTime;

        if (t >= duration)
            gameObject.SetActive(false);
    }
}
