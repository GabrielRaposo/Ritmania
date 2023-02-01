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

    float heldTime = 0; // temp

    private void Update() 
    {
        if (Input.GetKeyDown(KeyCode.A))
            SetAutoPlay(!AutoPlay);

                    if (Input.touchCount > 1)
                    {
                        int count = 0;
                        foreach (Touch touch in Input.touches) 
                        {
                            if (touch.phase == TouchPhase.Stationary)
                            {
                                count++;
                            }
                        }

                        if (count > 1)
                        {
                            Debug.Log("heldTime: " + heldTime);
                            heldTime += Time.fixedDeltaTime;
                            if (heldTime > 2.0f)
                            {
                                SetAutoPlay(!AutoPlay);
                                heldTime = 0;
                                return;
                            }
                        }
                    }
                    else 
                    {
                        heldTime = 0;
                    }

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
