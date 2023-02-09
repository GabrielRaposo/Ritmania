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
    public Color editorColor;

    public int numerator;
    public int denominator;

    public BeatType beatType;
    public InputRegion inputRegion;

    public AudioClip hitAudioClip;
    [Range(0f, 1.0f)] public float hitAudioVolume = .2f;

    public AudioClip missAudioClip;
    [Range(0f, 1.0f)] public float missAudioVolume = .2f;

    public BeatAnswerInformation()
    {
        numerator = 1;
        denominator = 4;
    }

    public float Fraction() => (float) numerator / denominator;
} 
