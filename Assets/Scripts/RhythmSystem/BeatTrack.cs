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
        public int quantNotes;

        // Transforms that define the spatial origin and end of the timeline space on the screen
        [Header("Anchors")]
        public Transform spawnAnchor;
        public Transform targetAnchor;

        int nextIndex;        // Last HitNote that's been spawned. 
        bool beatmapIsReady;  // 
        Conductor conductor; 

        // -- Beatmap
        List<float> beatmapInBeats; // Lista com formato acessível para ser escrito
        List<float> beatmap;        // Beatmap de fato, com precisão de tempo em segundos
        List<HitNote> activeNotes;  // Enfilera as notas em tela em ordem de chegada

        // This class should be always set-up by the BeatMapCaller.
        public void Setup (Conductor conductor) 
        {
            this.conductor = conductor;

            // -- Temp: BeatMap as Beats
            beatmapInBeats = new List<float>() { 0, 1, 1.5f, 2, 4, 5, 6, 8, 9, 10 };

            beatmapIsReady = false;
            StartCoroutine( conductor.WaitUntilHasInitiated(TranslateToBeatmap) );
        }

        private void TranslateToBeatmap()
        {
            // -- Transforma beatmap em divisões de segundos
            beatmap = new List<float>();
            for (int i = 0; i < beatmapInBeats.Count; i++) 
            {
                beatmap.Add(beatmapInBeats[i] * conductor.secPerBeat);
            }

            activeNotes = new List<HitNote>();

            beatmapIsReady = true;
            
            // -- Ativa a leitura de inputs
            if (rhythmJudge)
                rhythmJudge.Setup(conductor, this);
        }

        void Update()
        {
            if (!beatmapIsReady)
                return;

            if (nextIndex > beatmap.Count - 1)
            {
                // TO-DO: flag de termino de beatmap
                return;
            }

            float currentBeatTime = beatmap[nextIndex];
            float timeShownInAdvance = conductor.TimeShownInAdvance;
            if (currentBeatTime < conductor.songPosition + timeShownInAdvance)
            {
                GameObject note = notesPool.GetFromPool();
                note.transform.position = Vector2.right * nextIndex;
                note.SetActive(true);

                HitNote hitNote = note.GetComponent<HitNote>();
                if (hitNote)
                {
                    hitNote.Setup(currentBeatTime, timeShownInAdvance, this, conductor, spawnAnchor, targetAnchor);
                    activeNotes.Add(hitNote); // Enfilera a nota para que seja visível para o Rhythm Judge
                }

                nextIndex++;
            }
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
