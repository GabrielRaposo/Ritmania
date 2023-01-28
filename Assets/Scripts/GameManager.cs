using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI autoPlayDisplay;

    public static bool AutoPlay;

    void Start()
    {
        Application.targetFrameRate = 60;
        SetAutoPlay(false);
    }

    private void Update() 
    {
        if (Input.GetKeyDown(KeyCode.A))
            SetAutoPlay(!AutoPlay);

        if (Input.GetKeyDown(KeyCode.R))
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void SetAutoPlay(bool value)
    {
        AutoPlay = value;
        if (autoPlayDisplay)
        {
            autoPlayDisplay.text = "[A] - Autoplay: ";
            autoPlayDisplay.text += AutoPlay ?
                "<color=green>ON" :
                "<color=red>OFF";
        }
    }
}
