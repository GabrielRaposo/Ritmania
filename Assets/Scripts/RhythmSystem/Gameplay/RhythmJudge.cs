using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace RhythmSystem 
{
    public enum PrecisionScore { Perfect, Good, Bad, Miss }

    public class RhythmJudge : MonoBehaviour
    {
        const float PERFECT_THRESHOLD = .05f;
        const float GOOD_THRESHOLD    = .10f;
        const float BAD_THRESHOLD     = .15f;

        const float RANGE_RADIUS = .5f; // Distância mínima para que a nota seja interagível

        public Transform cursor;

        Conductor conductor;
        BeatTrack beatTrack;

        bool isPlaying;
        HitNote focusedNote;

        // Recebe valores de inicialização do BeatTrack
        public void Setup (Conductor conductor, BeatTrack beatTrack) 
        {
            this.conductor = conductor;
            this.beatTrack = beatTrack;
            isPlaying = true;
        }

        void Update()
        {
            if (!isPlaying)
                return;

            // Atualiza a nota de foco
            focusedNote = beatTrack.GetActiveHitNote();

            // Se não existe nota para se focar, some com o cursor
            if (focusedNote == null)
            {
                if (cursor)
                    cursor.position = Vector2.up * 100;
                return;
            }

            // Faz cursor acompanhar posição da nota em foco
            if (cursor)
            {
                cursor.position = focusedNote.transform.position;
                cursor.localScale = Vector2.one * (Mathf.Abs((float)(focusedNote.BeatTime - conductor.songPosition)) < RANGE_RADIUS ? 1 : .8f);
            }

            // Lê input e tenta acertar nota
            if (Input.GetKeyDown(KeyCode.Space)) 
                TryToHitNote(conductor.songPosition);
        }

        private void TryToHitNote(double hitTime)
        {
            double difference = focusedNote.BeatTime - hitTime;
            float absDifference = Mathf.Abs((float)difference);

            // If the note is way too far, don't even try to hit it
            if (absDifference > RANGE_RADIUS) 
                return;

            if (absDifference < PERFECT_THRESHOLD)
            {
                focusedNote.OnHit(PrecisionScore.Perfect);
                return;
            }

            if (absDifference < GOOD_THRESHOLD)
            {
                focusedNote.OnHit(PrecisionScore.Good);
                return;
            }

            if (absDifference < BAD_THRESHOLD)
            {
                focusedNote.OnHit(PrecisionScore.Bad);
                return;
            }
            
            focusedNote.OnHit(PrecisionScore.Miss);
        }
    }
}