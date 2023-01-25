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
        AutoPlay = true;
    }

    private void Update() 
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            AutoPlay = !AutoPlay;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
