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
    public List<Tuple<float, BeatCall>> calls;


}
