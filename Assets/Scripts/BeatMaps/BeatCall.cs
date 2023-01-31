using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class BeatCall
{
    public string name;
    public string code;
    
    [FormerlySerializedAs("awnserDistance")] public int answerDistance;
    
    [FormerlySerializedAs("awnserCount")] public  int answerCount;
    [FormerlySerializedAs("awnsersSpacing")] public  int answersSpacing;

    public Color editorColor;

    public AudioClip callClip;
    public AudioClip answerClip;

    public BeatCall()
    {
        editorColor = Color.red;
        name = "Beat Call";

        code = Random.Range(0, 1728).ToString("X");
    }
    
}
