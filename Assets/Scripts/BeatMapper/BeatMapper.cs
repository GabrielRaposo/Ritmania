using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "BeatMapper", menuName = "ScriptableObjects/BeatMapper")]
public class BeatMapper : ScriptableObject
{
    public AudioClip clip;
    public List<BeatCall> callTypes;
    public List<BeatTiming> timedCalls;

    public List<Tuple<Tempo, int>> bpmChanges;

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
    
    public int GetBpm(int index)
    {
        if (bpmChanges == null)
            return -1;
        if (bpmChanges.Count <= index)
            return -1;

        return bpmChanges[index].Item2;
    }

    public float GetBeatLenght(int index)
    {
        if (bpmChanges == null)
            return -1;
        if (bpmChanges.Count <= index)
            return -1;

        return 60f / GetBpm(index);
    }

    public BeatCall GetCallFromCode(string code)
    {
        return callTypes.Find(a => a.Code == code);
    }

    public float GetTimeFromTempo(Tempo t)
    {
        if (bpmChanges == null)
            return -1;

        if (bpmChanges.Count == 0)
            return -1;

        if (bpmChanges.Count == 1)
            return t.compass * GetBeatLenght(0) + t.tempo * 4 * GetBeatLenght(0);

        if (t < bpmChanges[1].Item1)
            return t.compass * GetBeatLenght(0) + t.tempo * 4 * GetBeatLenght(0);
        
        float time = 0f;

        int index = 0;
        
        while (t<bpmChanges[index].Item1 && bpmChanges.Count < index)
        {
            index++;
        }
        
        

    }
    
}

[Serializable]
public class BeatTiming
{
    public Tempo tempo;
    
    public string code;

    public BeatTiming(int t, int c, string code)
    {
        tempo = new Tempo(t, c);
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
            int t = tempo.tempo;
            int c = tempo.compass + call.answerDistance + call.answersSpacing * i;
                    
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

