using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FPSDisplay : MonoBehaviour
{
    public bool ceilValue;

    TextMeshProUGUI display;
    float deltaTime;

    void Start()
    {
        display = GetComponent<TextMeshProUGUI>();
        if (!display)
        {
            this.ShowErrorAndDisable("Unable to find display.");
            return;
        }
    }
    void Update () 
    {
         deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
         float fps = 1.0f / deltaTime;
         display.text = "FPS: ";
         display.text += ceilValue ? Mathf.Ceil(fps).ToString() : fps.ToString ("0.0");
    }
}
