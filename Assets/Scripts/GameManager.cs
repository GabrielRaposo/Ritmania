using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static bool AutoPlay;

    void Start()
    {
        Application.targetFrameRate = 60;
        AutoPlay = false;
    }

    private void Update() 
    {
        // Toggle de Auto-play
        if (Input.GetKeyDown(KeyCode.A))
        {
            AutoPlay = !AutoPlay;
        }

        // Reset da cena
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
