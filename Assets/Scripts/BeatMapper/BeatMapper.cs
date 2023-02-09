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
    public List<BeatTiming> timedCalls;

    public void RemoveCall(string code)
    {
        for (int i = timedCalls.Count - 1; i >= 0; i--)
        {
            if(timedCalls[i].code == code)
                timedCalls.RemoveAt(i);
        }

        var toRemove = callTypes.Find(a => a.ID == code);
        
        if(toRemove!= null)
            callTypes.Remove(toRemove);
    }

    public BeatCall GetCallFromCode(string code)
    {
        return callTypes.Find(a => a.ID == code);
    }

    public float GetTimeFromTempo(int objTempo, int objCompass)
    {
        return objTempo * BeatLenght + objCompass * BeatLenght * 0.25f;
    }
}

[System.Serializable]
public class BeatTiming
{
    public int tempo;
    public int compass;

    public string code;

    public BeatTiming(int t, int c, string code)
    {
        tempo = t;
        compass = c;
        this.code = code;
    }

    public List<double> GetAnswersTiming(BeatMapper map)
    {
        var call = map.GetCallFromCode(code);
        if (call == null)
            return null;

        var list = new List<double>();
        
        for (int i = 0; i < call.answerInfo.Count; i++)
        {
            var info = call.answerInfo[i];
            list.Add(info.Fraction()*map.BeatLenght);
        }

        return list;

    }

}

