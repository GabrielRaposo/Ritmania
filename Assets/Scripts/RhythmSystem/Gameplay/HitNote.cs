using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RhythmSystem 
{
    // Notes that need to be hit rhythmically
    public class HitNote : MonoBehaviour
    {
        const double AUTOPLAY_THRESHOLD = .001d;

        double beatTime;
        double timeShownInAdvance;

        BeatTrack beatTrack;
        Conductor conductor;

        Transform spawnAnchor;
        Transform targetAnchor;        

        public double BeatTime => beatTime;

        public void Setup 
        (
            double beatTime, double timeShownInAdvance, 
            BeatTrack beatTrack, Conductor conductor, 
            Transform spawnAnchor, Transform targetAnchor
        )
        {
            this.beatTime = beatTime;
            this.timeShownInAdvance = timeShownInAdvance;

            this.beatTrack = beatTrack;
            this.conductor = conductor;

            this.spawnAnchor = spawnAnchor;
            this.targetAnchor = targetAnchor;

            transform.position = spawnAnchor.position;
            gameObject.SetActive(true);
        }

        void LateUpdate ()
        {
            if (!conductor.HasInitiated)
                return;
                
            LerpPosition();

            if (GameManager.AutoPlay)
            {
                double hitOffset = conductor.songPosition - beatTime;
                if (Mathf.Abs((float)hitOffset) < AUTOPLAY_THRESHOLD || conductor.songPosition >= beatTime)
                {
                    transform.position = targetAnchor.position;
                    Debug.Log($"Offset: { hitOffset.ToString("0.0000") }");
                    OnHit(PrecisionScore.Perfect);
                }
                return;
            }

            // If AutoPlay is off, verify if the note has crossed the MissThreshold
            if (conductor.songPosition > beatTime + conductor.MissThreshold)
            {
                OnHit(PrecisionScore.Miss);
                return;
            }
        }

        private void LerpPosition()
        {
            double t = (beatTime - conductor.songPosition);
            double f = (timeShownInAdvance - t) / timeShownInAdvance; 

            // If the note is between the spawn point and the reaching point
            if (f <= 1.0d) 
            {
                // Lerps the note position between the two points 
                transform.position = Vector2.Lerp (
                    spawnAnchor.position,
                    targetAnchor.position,
                    (float) f
                );
            }
            // If the note has gone over the reaching point, overshoots the note
            else {
                // Lerps the note position between the 
                transform.position = Vector2.Lerp(
                    targetAnchor.position,
                    (targetAnchor.position * 2) - spawnAnchor.position,
                    (float)f - 1
                );
            }
        }

        public void OnHit (PrecisionScore score)
        {
            switch(score) 
            {
                default:
                case PrecisionScore.Perfect:
                    SFXController.Instance.PlaySound("Hit");
                    break;

                case PrecisionScore.Miss:
                    SFXController.Instance.PlaySound("Miss");
                    break;
            }

            FeedbackDisplayer.Instance.CallFeedback(score);

            if (beatTrack)
                beatTrack.OnNoteDeactivation(this);
            gameObject.SetActive(false); 
        }
    }
}
