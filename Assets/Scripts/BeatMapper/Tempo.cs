using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Tempo
{
    public int tempo;
    public int compass;

    public Tempo(int t, int c)
    {
        tempo = t;
        compass = c;
    }

    public void test(Tempo a, Tempo b)
    {
        if(a>b)
            return;
    }

    private sealed class TempoCompassRelationalComparer : IComparer<Tempo>
    {
        public int Compare(Tempo x, Tempo y)
        {
            if (x.compass == y.compass && x.tempo == y.tempo)
                return 0;

            int valueX = x.compass + x.tempo * 4;
            int valueY = y.compass + y.tempo * 4;

            if (valueX > valueY)
                return -1;
            return 1;
        }
    }

    public static bool operator >(Tempo x, Tempo y)
    {
        return TempoCompassComparer.Compare(x, y) < 0;
    }

    public static bool operator <(Tempo x, Tempo y)
    {
        return TempoCompassComparer.Compare(x, y) > 0;
    }

    public static IComparer<Tempo> TempoCompassComparer { get; } = new TempoCompassRelationalComparer();
}
