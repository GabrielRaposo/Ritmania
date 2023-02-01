using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace RhythmSystem 
{
    public enum PrecisionScore { Perfect, Good, Bad, Miss }

    public class RhythmJudge : MonoBehaviour
    {
        // These values are tested for both early and late hits, 
        // so a 0.05 sec threshold is effectively a 0.10 sec leniency range
        const float PERFECT_THRESHOLD = .05f;
        const float GOOD_THRESHOLD    = .10f;
        const float BAD_THRESHOLD     = .15f;

        // Minimum distance for a note to be interactable
        const float RANGE_RADIUS = .3f; 

        public Transform cursor;

        Conductor conductor;
        BeatTrack beatTrack;

        bool isPlaying;
        HitNote focusedNote;

        public void Setup (Conductor conductor, BeatTrack beatTrack) 
        {
            this.conductor = conductor;
            this.beatTrack = beatTrack;
            isPlaying = true;
        }

        void LateUpdate()
        {
            if (!isPlaying)
                return;

            focusedNote = beatTrack.GetActiveHitNote();

            // If there's no note to focus on, hides the cursor off-screen
            if (focusedNote == null)
            {
                if (cursor)
                    cursor.position = Vector2.up * 100;
                return;
            }

            SetCursorToFocusedNote();

            if (Input.GetKeyDown(KeyCode.Space)) 
                TryToHitNote(conductor.songPosition);

            foreach (Touch touch in Input.touches) 
            {
                if (touch.phase == TouchPhase.Began)
                {
                    TryToHitNote(conductor.songPosition);
                }
            }
        }

        private void SetCursorToFocusedNote()
        {
            if (!cursor)
                return;
            
            cursor.position = focusedNote.transform.position;
            cursor.localScale = Vector2.one * (Mathf.Abs((float)(focusedNote.BeatTime - conductor.songPosition)) < RANGE_RADIUS ? 1 : .8f);
        }

        private void TryToHitNote(double hitTime)
        {
            double difference = hitTime - focusedNote.BeatTime;
            float absDifference = Mathf.Abs((float)difference);

            // If the note is way too far, don't even try to hit it
            if (absDifference > RANGE_RADIUS) 
                return;

            if (absDifference < PERFECT_THRESHOLD)
            {
                focusedNote.OnHit(PrecisionScore.Perfect, difference);
                return;
            }

            if (absDifference < GOOD_THRESHOLD)
            {
                focusedNote.OnHit(PrecisionScore.Good, difference);
                return;
            }

            if (absDifference < BAD_THRESHOLD)
            {
                focusedNote.OnHit(PrecisionScore.Bad, difference);
                return;
            }
            
            focusedNote.OnHit(PrecisionScore.Miss, difference);
        }
    }
}