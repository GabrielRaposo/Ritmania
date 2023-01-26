using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RhythmSystem;
using TMPro;

public class TimerDisplay : MonoBehaviour
{
    Conductor conductor;
    TextMeshProUGUI display;

    void Start()
    {
        conductor = GetComponentInParent<Conductor>();
        display = GetComponentInChildren<TextMeshProUGUI>();

        if (!conductor || !display)
            gameObject.SetActive(false);
    }

    void Update()
    {
        display.text = "Time: " + conductor.songPosition.ToString("00.0000");
    }
}
