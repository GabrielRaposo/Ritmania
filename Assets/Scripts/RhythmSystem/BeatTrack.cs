using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RhythmSystem 
{
    // Classe respons�vel por gerar as HitNotes
    public class BeatTrack : MonoBehaviour
    {       
        public ObjectPool notesPool;
        public int quantNotes;

        [Header("Anchors")]
        public Transform spawnAnchor;
        public Transform targetAnchor;

        int nextIndex;
        bool beatmapIsReady;
        Conductor conductor; 

        // -- Beatmap
        List<float> beatmapInBeats; // Lista com formato acess�vel para ser escrito
        List<float> beatmap; // Beatmap de fato, com precis�o de tempo em segundos

        private void Start() 
        {
            conductor = Conductor.Instance;
            if (!conductor)
            {
                Debug.LogError("Conductor couldn't be found.");
                enabled = false;
                return;
            }

            // -- Define beatmap em Beats
            beatmapInBeats = new List<float>() { 0, 1, 1.5f, 2, 4, 5, 6, 8, 9, 10 };

            beatmapIsReady = false;
            StartCoroutine( conductor.WaitUntilIsRunning(TranslateToBeatmap) );
        }

        private void TranslateToBeatmap()
        {
            // -- Transforma beatmap em divis�es de segundos
            beatmap = new List<float>();
            for (int i = 0; i < beatmapInBeats.Count; i++) 
            {
                beatmap.Add(beatmapInBeats[i] * conductor.secPerBeat);
            }

            beatmapIsReady = true;
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
                    hitNote.Setup(currentBeatTime, timeShownInAdvance, conductor, spawnAnchor, targetAnchor);

                nextIndex++;
            }
        }
    }
}
