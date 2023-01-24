using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RhythmSystem 
{
    public class BeatTrack : MonoBehaviour
    {       
        public Conductor conductor; 
        public ObjectPool notesPool;
        public int quantNotes;
        public float beatsShownInAdvance; // TO-DO: passar pra outro lugar, conseguir essa info através de inicialização

        [Header("Anchors")]
        public Transform spawnAnchor;
        public Transform targetAnchor;

        int nextIndex;
        bool beatmapWasSetup;

        //beatmap
        List<float> beatmapInBeats; // Lista com formato acessível
        List<float> beatmap; // Beatmap de fato, com precisão de tempo em segundos

        private void Start() 
        {
            // -- Define beatmap em Beats
            beatmapInBeats = new List<float>() { 0, 1, 1.5f, 2, 4, 5, 6, 8, 9, 10 };

            beatmapWasSetup = false;
            StartCoroutine( conductor.WaitUntilIsRunning(TranslateToBeatmap) );
        }

        private void TranslateToBeatmap()
        {
            // -- Transforma beatmap em divisões de segundos
            beatmap = new List<float>();
            for (int i = 0; i < beatmapInBeats.Count; i++) 
            {
                beatmap.Add(beatmapInBeats[i] * conductor.secPerBeat);
            }

            beatmapWasSetup = true;
        }


        void Update()
        {
            if (!beatmapWasSetup)
                return;

            if (nextIndex > beatmap.Count - 1)
            {
                // TO-DO: flag de termino de beatmap
                return;
            }

            float currentBeatTime = beatmap[nextIndex];
            float timeShownInAdvance = beatsShownInAdvance * conductor.secPerBeat;
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
