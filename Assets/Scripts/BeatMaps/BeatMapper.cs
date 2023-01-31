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

        var toRemove = callTypes.Find(a => a.Code == code);
        
        if(toRemove!= null)
            callTypes.Remove(toRemove);
    }

    public BeatCall GetCallFromCode(string code)
    {
        return callTypes.Find(a => a.Code == code);
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

    public List<Tuple<int, int>> GetAnswersTiming(BeatMapper map)
    {
        var call = map.GetCallFromCode(code);
        if (call == null)
            return null;

        var list = new List<Tuple<int, int>>();
        
        for (int i = 0; i < call.answerCount; i++)
        {
            int t = tempo;
            int c = compass + call.answerDistance + call.answersSpacing * i;
                    
            if (c >= 4)
            {
                t += Mathf.FloorToInt(c / 4f);
                c = c % 4;
            }
            
            list.Add(new Tuple<int, int>(t,c));
        }

        return list;

    }

}

