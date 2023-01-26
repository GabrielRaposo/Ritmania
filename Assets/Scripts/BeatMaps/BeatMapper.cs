using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "BeatMapper", menuName = "ScriptableObjects/BeatMapper")]
public class BeatMapper : ScriptableObject
{
    public AudioClip clip;
    public int bpm;

    public float BeatLenght => 60f / bpm;

    public List<BeatCall> callTypes;
    public List<BeatTiming> calls;
}

[System.Serializable]
public class BeatTiming
{
    public int tempo;
    public int compass;

    public BeatCall call;

    public BeatTiming(int t, int c, BeatCall call)
    {
        tempo = t;
        compass = c;
        this.call = call;
    }
    
}

