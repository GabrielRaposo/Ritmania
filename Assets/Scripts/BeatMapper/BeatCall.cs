using System.Collections;
using System.Collections.Generic;using System.Security.Claims;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class BeatCall
{
    public string name;
    [FormerlySerializedAs("code")] [SerializeField]private string id;
    public string ID
    {
        get
        {
            if(id == "")
                Init();
            return id;
        }
        set => id = value;
    }

    public List<BeatAnswerInformation> answerInfo;

    public Color editorColor;

    public AudioClip callClip;
    public AudioClip answerClip;

    public BeatCall()
    {
        editorColor = Color.red;
        name = "Beat Call";

        //id = Random.Range(0, 1728).ToString("X");
    }

    public void Init()
    {
        id = Random.Range(0, 1728).ToString("X");
    }
}

public enum BeatType {Normal, Dependent, HoldStart, HoldEnd}
public class BeatAnswerInformation
{
    public int numerator;
    public int denominator;

    public BeatType beatType;

    public BeatAnswerInformation()
    {
        numerator = 1;
        denominator = 4;
    }

    public float Fraction() => (float)numerator / denominator;
} 
