using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BeatMapData", menuName = "ScriptableObjects/BeatMapData")]
public class BeatMapData : ScriptableObject
{
    public AudioClip Music;
    public float BPM;
    public float BeatsShownInAdvance;
    public float FirstBeatOffset;
    public float MissThreshold;

    [Space(10)]

    public List<BeatCall> beatCalls;
}
