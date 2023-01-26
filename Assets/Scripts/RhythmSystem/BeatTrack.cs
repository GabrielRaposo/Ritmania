using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RhythmSystem 
{
    // Responsible for generating the Hit Notes according to the beatmap data
    public class BeatTrack : MonoBehaviour
    {       
        public RhythmJudge rhythmJudge;
        public ObjectPool notesPool;

        // Anchors that define the start and end positions of the timeline on the screen
        [Header("Anchors")]
        public Transform spawnAnchor;
        public Transform targetAnchor;

        int nextIndex;        // Last HitNote that's been spawned. 
        bool beatmapIsReady;  // Is set to true when the list of hitnotes is prepared

        // -- Beatmap
        List<float> beatmapInBeats; // Lista com formato acess�vel para ser escrito
        List<float> beatmap;        // Beatmap de fato, com precis�o de tempo em segundos
        List<HitNote> activeNotes;  // Enfilera as notas em tela em ordem de chegada

        Conductor conductor; 

        // This class should be always set-up by the BeatMapCaller.
        public void Setup (Conductor conductor) // -- TO-DO: receive beatmap data as well
        {
            this.conductor = conductor;

            activeNotes = new List<HitNote>();

            // -- Temp: BeatMap as Beats
            beatmapInBeats = new List<float>() { 0, 1, 1.5f, 2, 4, 5, 6, 8, 9, 10 };
            TranslateToBeatmap();

            beatmapIsReady = false;
        }

        // Translates the beatmap from beat's to second's timestamps
        public void TranslateToBeatmap()
        {
            beatmap = new List<float>();
            for (int i = 0; i < beatmapInBeats.Count; i++) 
            {
                beatmap.Add(beatmapInBeats[i] * conductor.secPerBeat);
            }
        }

        public void StartBeatMap()
        {
            // Activates the gameplay controls
            if (rhythmJudge)
                rhythmJudge.Setup(conductor, this);

            beatmapIsReady = true;
        }

        private bool SpawnedAllNotes => nextIndex > beatmap.Count - 1;

        void Update()
        {
            if (!beatmapIsReady)
                return;

            if (SpawnedAllNotes)
                return; // TO-DO: flag de termino de beatmap

            float currentNoteTime = beatmap[nextIndex];
            float timeShownInAdvance = conductor.TimeShownInAdvance;
            if (conductor.songPosition + timeShownInAdvance > currentNoteTime)
                SpawnHitNote (currentNoteTime, timeShownInAdvance);
        }

        private void SpawnHitNote(float currentNoteTime, float timeShownInAdvance)
        {
            GameObject note = notesPool.GetFromPool();
            note.SetActive(true);

            HitNote hitNote = note.GetComponent<HitNote>();
            if (!hitNote)
                return;

            hitNote.Setup(currentNoteTime, timeShownInAdvance, this, conductor, spawnAnchor, targetAnchor);
            activeNotes.Add(hitNote); // Enqueue the Hit Notes so they are visible by the Rhythm Judge in order
            
            nextIndex++;
        }

        public void OnNoteDeactivation (HitNote note)
        {
            if (activeNotes == null || activeNotes.Count < 1)
                return;
            activeNotes.Remove(note);
        }

        public HitNote GetActiveHitNote()
        {
            if (activeNotes == null || activeNotes.Count < 1)
                return null;
            return activeNotes[0];
        }
    }
}
