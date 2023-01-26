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

        private bool BeatMapDataIsInvalid => !beatMapData || !beatMapData.Music || beatMapData.BPM <= 0;
        private bool ComponentsAreMissing => !conductor || !beatTrack;

        void Start()
        {
            if (BeatMapDataIsInvalid)
            {
                this.ShowErrorAndDisable("Invalid beatmap data.");
                return;
            }

            if (ComponentsAreMissing) 
            {
                this.ShowErrorAndDisable("Some components are missing on the BeatMapCaller.");
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
                StartBeatMap();
        }

        private void StartBeatMap()
        {
            conductor.StartConduction();   
            beatTrack.StartBeatMap();
        }
    }
}
