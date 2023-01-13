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
        public float beatsShownInAdvance;

        [Header("Anchors")]
        public Transform spawnAnchor;
        public Transform targetAnchor;

        int nextIndex;

        void Start()
        {
        
        }

        void Update()
        {
            if(!conductor.isPlaying)
                return;

            float targetBeat = (nextIndex + 1) * 2; // Valor temporário

            if (nextIndex < quantNotes && targetBeat < conductor.songPositionInBeats + beatsShownInAdvance)
            {
                GameObject note = notesPool.GetFromPool();
                note.transform.position = Vector2.right * nextIndex;
                note.SetActive(true);

                HitNote hitNote = note.GetComponent<HitNote>();
                if (hitNote) 
                    hitNote.Setup(targetBeat, beatsShownInAdvance, conductor, spawnAnchor, targetAnchor);

                nextIndex++;
            }
        }
    }
}
