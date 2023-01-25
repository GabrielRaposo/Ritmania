using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RhythmSystem 
{
    public class RhythmJudge : MonoBehaviour
    {
        bool isPlaying;

        Conductor conductor;
        BeatTrack beatTrack;

        public void Setup(Conductor conductor, BeatTrack beatTrack) 
        {
            this.conductor = conductor;
            this.beatTrack = beatTrack;
            isPlaying = true;
        }

        void Update()
        {
            if (!isPlaying)
                return;

            if (Input.GetKeyDown(KeyCode.Space)) 
            {
                Debug.Log("click");
            }
        }
    }
}