using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RhythmSystem 
{
    // Responsible for generating the Hit Notes according to the beatmap data
    public class BeatTrack : MonoBehaviour
    {   
        public class NoteData 
        {
            public NoteData (string tag, double time) 
            {
                this.tag = tag;
                this.time = time;
            }

            public string tag;
            public double time;
        }

        public RhythmJudge rhythmJudge;
        public ObjectPool cuesPool; // <<< COMMENT HERE
        public ObjectPool notesPool;
        public CallSoundController callSoundController;

        // Anchors that define the start and end positions of the timeline on the screen
        [Header("Anchors")]
        public Transform spawnAnchor;
        public Transform targetAnchor;

        int nextCueIndex;     // the index of the last CueNote that's been spawned 
        int nextHitIndex;     // the index of the last HitNote that's been spawned 
        bool beatmapIsReady;  // Is set to true when the list of hitnotes is prepared

        // -- Beatmap
        List<float> beatmapInBeats;  
        List<NoteData> beatmapCues;
        List<double> beatmapHits;
        List<CueHitNote> cueHitNotes; // <<< COMMENT HERE
        List<HitNote> activeHitNotes;   // Queue of the active hit notes that are on screen

        Conductor conductor; 
        BeatMapData beatMapData;

        // This class should be always set-up by the BeatMapCaller.
        public void Setup (Conductor conductor, BeatMapData beatMapData)
        {
            this.conductor = conductor;
            this.beatMapData = beatMapData;

            cueHitNotes = new List<CueHitNote>();
            activeHitNotes = new List<HitNote>();

            SetupCallSounds();
            TranslateToBeatmap();  // Make the CueNotes list
            MakeHitNotesList();    // Make the HitNotes list based on the CueNotes list above

            beatmapIsReady = false;
        }

        public void SetupCallSounds()
        {
            if (!callSoundController || !beatMapData)
                return;
            
            if (beatMapData.beatCalls.Count < 1)
                return;

            for (int i = 0; i < beatMapData.beatCalls.Count; i++)
            {
                BeatCall beatCall = beatMapData.beatCalls[i];
                callSoundController.AddCallData(beatCall.tag, beatCall.callClip, beatCall.callClipVolume);
            }
        }

        // Translates the beatmap from beat's to second's timestamps
        public void TranslateToBeatmap()
        {
            // -- Temp: BeatMap as Beats
            beatmapInBeats = new List<float>();
            for (int i = 0; i < 100; i++) beatmapInBeats.Add(i * 2);

            string cueTag = beatMapData.beatCalls[0].tag;
            // --

            beatmapCues = new List<NoteData>();
            for (int i = 0; i < beatmapInBeats.Count; i++) 
            {
                beatmapCues.Add( new NoteData(cueTag, beatmapInBeats[i] * conductor.secPerBeat) );
            }

        }

        public void MakeHitNotesList()
        {
            if (!beatMapData)
                return;

            beatmapHits = new List<double>();

            foreach (NoteData cueData in beatmapCues)
            {
                BeatCall beatCall = beatMapData.beatCalls.Find( (call) => call.tag == cueData.tag );
                if (beatCall == null)
                    continue;

                foreach (BeatAnswerInformation answerInfo in beatCall.answerInfo)
                {
                    double d = cueData.time + (answerInfo.Fraction() * conductor.secPerBeat);
                    // TO-DO: fazer esse tempo não depender de cueData.time para evitar proliferação de erros de arredondamento
                    beatmapHits.Add(d);
                    Debug.Log("d: " + d);
                }
            }

        }

        public void StartBeatMap()
        {
            // Activates the gameplay interaction
            if (rhythmJudge)
                rhythmJudge.Setup(conductor, this);

            beatmapIsReady = true;
        }

        private bool SpawnedAllNotes => nextHitIndex > beatmapHits.Count - 1;

        void Update()
        {
            if (!beatmapIsReady)
                return;

            if (SpawnedAllNotes)
                return;

            double timeShownInAdvance = conductor.TimeShownInAdvance;

            // If it's X-seconds before the time for the cue to play, spawn it
            if (conductor.songPosition + timeShownInAdvance > beatmapCues[nextCueIndex].time)
            {
                SpawnCueNote (beatmapCues[nextCueIndex], timeShownInAdvance);
                nextCueIndex++;
                return;
            }

            // If it's X-seconds before the time for the note to be hit, spawn it
            if (conductor.songPosition + timeShownInAdvance > beatmapHits[nextHitIndex])
            {
                SpawnHitNote (beatmapHits[nextHitIndex], timeShownInAdvance);
                nextHitIndex++;
            }
        }
        private void SpawnCueNote(NoteData noteData, double timeShownInAdvance)
        { 
            GameObject note = cuesPool.GetFromPool();
            CueHitNote cueNote = note.GetComponent<CueHitNote>();
            if (!cueNote)
                return;

            cueNote.Setup(noteData.tag, noteData.time, timeShownInAdvance, this, conductor, spawnAnchor, targetAnchor);
            cueHitNotes.Add(cueNote); // Enqueue the Hit Notes so they are visible by the Rhythm Judge in order
        }

        private void SpawnHitNote(double currentNoteTime, double timeShownInAdvance)
        { 
            GameObject note = notesPool.GetFromPool();
            HitNote hitNote = note.GetComponent<HitNote>();
            if (!hitNote)
                return;

            hitNote.Setup("", currentNoteTime, timeShownInAdvance, this, conductor, spawnAnchor, targetAnchor);
            activeHitNotes.Add(hitNote); // Enqueue the Hit Notes so they are visible by the Rhythm Judge in order
        }


        public void OnNoteDeactivation (HitNote note)
        {
            if (activeHitNotes == null || activeHitNotes.Count < 1)
                return;
            activeHitNotes.Remove(note);
        }

        public HitNote GetActiveHitNote()
        {
            if (activeHitNotes == null || activeHitNotes.Count < 1)
                return null;
            return activeHitNotes[0];
        }
    }
}
