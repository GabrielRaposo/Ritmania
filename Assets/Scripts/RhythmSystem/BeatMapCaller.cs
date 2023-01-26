using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RhythmSystem 
{
    // Responsible for initiating all the systems necessary for the BeatMap to run.
    public class BeatMapCaller : MonoBehaviour
    {
        public BeatMapData beatMapData;  
        [Space(10)]
        public Conductor conductor;
        public BeatTrack beatTrack;

        public bool BeatMapDataIsInvalid => !beatMapData || !beatMapData.Music || beatMapData.BPM <= 0;

        void Start()
        {
            if (BeatMapDataIsInvalid)
            {
                Debug.LogError("Invalid beatmap data.");
                enabled = false;
                return;
            }

            if (!conductor || !beatTrack) 
            {
                Debug.LogError("Some components are missing on the BeatMapCaller.");
                enabled = false;
                return;
            }

            conductor.Setup (beatMapData);
            beatTrack.Setup (conductor);
        }

        void Update()
        {
            // Temp: waits for player input to run. Later on this task should be executed externally.
            if (conductor.HasInitiated)
                return;
            
            if (Input.GetKeyDown(KeyCode.Return))
                    conductor.StartConduction();   
        }
    }
}
