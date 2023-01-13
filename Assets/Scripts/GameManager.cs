using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static bool AutoPlay;

    void Start()
    {
        Application.targetFrameRate = 60;
        AutoPlay = true;
    }
}
