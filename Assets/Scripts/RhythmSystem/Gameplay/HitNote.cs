using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RhythmSystem 
{
    // Classe para as notas que devem ser acertadas dentro do ritmo
    public class HitNote : MonoBehaviour
    {
        float beatTime;
        float timeShownInAdvance;

        BeatTrack beatTrack;
        Conductor conductor;

        Transform spawnAnchor;
        Transform targetAnchor;        

        public float BeatTime { get { return beatTime; } }

        public void Setup 
        (
            float beatTime, float timeShownInAdvance, 
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
        }

        void Update()
        {
            float t = (beatTime - conductor.songPosition);
            float f = (timeShownInAdvance - t) / timeShownInAdvance; 

            // Se está entre o ponto de spawn e o ponto de chegada 
            if (f <= 1.0f) 
            {
                // Faz lerp na posição da nota de acordo com o tempo t 
                transform.position = Vector2.Lerp (
                    spawnAnchor.position,
                    targetAnchor.position,
                    f
                );
            }
            // Se já passou do ponto de chegada, faz overshoot da nota
            else {
                // Faz lerp na posição da nota de acordo com o tempo t 
                transform.position = Vector2.Lerp(
                    targetAnchor.position,
                    (targetAnchor.position * 2) - spawnAnchor.position,
                    f - 1
                );
            }

            if (GameManager.AutoPlay)
            {
                if (conductor.songPosition >= beatTime) 
                {
                    transform.position = targetAnchor.position;
                    OnHit(); // Se estiver com AutoPlay, acerta a nota no primeiro frame válido
                }
                return;
            }

            // Se não estiver com AutoPlay, verifica se atravessou o MissThreshold
            if (conductor.songPosition > beatTime + conductor.missThreshold)
            {
                OnMiss();
                return;
            }
        }

        public void OnHit()
        {
            SFXController.Instance.PlaySound("Hit");
            FeedbackDisplayer.Instance.CallFeedback(PrecisionScore.Perfect);

            if (beatTrack)
                beatTrack.OnNoteDeactivation(this);
            gameObject.SetActive(false); 
        }

        public void OnMiss()
        {
            SFXController.Instance.PlaySound("Miss");
            FeedbackDisplayer.Instance.CallFeedback(PrecisionScore.Miss);
            
            if (beatTrack)
                beatTrack.OnNoteDeactivation(this);
            gameObject.SetActive(false); 
        }

        public float GetBeatTime ()
        {
            return beatTime;
        }
    }
}
