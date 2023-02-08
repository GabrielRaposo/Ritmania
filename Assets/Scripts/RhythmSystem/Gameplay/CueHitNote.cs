using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RhythmSystem 
{
    // Notes that make the sound of the cue rhythmically
    public class CueHitNote : HitNote
    {
        public override bool IsCue => true;

        //public override void OnHit(PrecisionScore score, double offset) 
        //{
        //    base.OnHit(score, offset);
        //}
    }
}
