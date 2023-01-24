using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "BeatMap", menuName = "ScriptableObjects/BeatMap")]
public class BeatMap : ScriptableObject
{
    public AudioClip clip;
    public int bpm;

    public List<BeatCall> callTypes;
    public List<Tuple<float, BeatCall>> calls;


}
