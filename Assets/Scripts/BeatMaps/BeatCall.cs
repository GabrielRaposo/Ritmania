using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class BeatCall
{
    public string name;
    [SerializeField]private string code;
    public string Code
    {
        get
        {
            if(code == "")
                Init();
            return code;
        }
        set => code = value;
    }
    public int answerDistance;
    public  int answerCount;
    public  int answersSpacing;

    public Color editorColor;

    public AudioClip callClip;
    public AudioClip answerClip;

    public BeatCall()
    {
        editorColor = Color.red;
        name = "Beat Call";

        //code = Random.Range(0, 1728).ToString("X");
    }

    public void Init()
    {
        code = Random.Range(0, 1728).ToString("X");
    }
    
    
}
