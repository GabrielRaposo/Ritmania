using System.Collections;
using System.Collections.Generic;using System.Security.Claims;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "_BeatCall", menuName = "ScriptableObjects/BeatCall")]
[System.Serializable]
public class BeatCall : ScriptableObject
{
    public string tag;
    [FormerlySerializedAs("code")] [SerializeField] private string id;
    public string ID
    {
        get
        {
            if (id == "")
                Init();
            return id;
        }
        set => id = value;
    }

    [Header("Call Info")]
    public Color editorColor;
    public AudioClip callClip;
    public float callClipVolume = 1.0f;

    [Space(10)]
    public List<BeatAnswerInformation> answerInfo;

    // -- TO-DO: remover pois agora está informação esta presente no BeatAnswerInformation
    [Header("Remove this later:")]
    public AudioClip answerClip; 
    // -- 

    public BeatCall()
    {
        editorColor = Color.red;
        tag = "Beat Call";

        //id = Random.Range(0, 1728).ToString("X");
    }

    public void Init()
    {
        id = Random.Range(0, 1728).ToString("X");
    }
}

public enum BeatType {Normal, Dependent, HoldStart, HoldEnd}

[System.Serializable]
public class BeatAnswerInformation
{
    public int numerator;
    public int denominator;

    public Color editorColor;
    public BeatType beatType;
    public AudioClip audioClip;
    [Range(0f, 1.0f)] public float audioVolume = 1.0f;

    public BeatAnswerInformation()
    {
        numerator = 1;
        denominator = 4;
    }

    public float Fraction() => (float)numerator / denominator;
} 
