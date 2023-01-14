using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RhythmSystem 
{
    public class HitNote : MonoBehaviour
    {
        float targetBeat;
        float timeShownInAdvance;
        Conductor conductor;
        Transform spawnAnchor;
        Transform targetAnchor;

        public void Setup (float targetBeat, float timeShownInAdvance, Conductor conductor, Transform spawnAnchor, Transform targetAnchor)
        {
            this.targetBeat = targetBeat;
            this.timeShownInAdvance = timeShownInAdvance;

            this.conductor = conductor;
            this.spawnAnchor = spawnAnchor;
            this.targetAnchor = targetAnchor;

            transform.position = spawnAnchor.position;
        }

        void Update()
        {
            float t = (targetBeat - conductor.songPosition);
            transform.position = Vector2.Lerp(
                spawnAnchor.position,
                targetAnchor.position,
                (timeShownInAdvance - t) / timeShownInAdvance
            );    

            // Se estiver com AutoPlay, acerta a nota no primeiro frame válido
            if (GameManager.AutoPlay && conductor.songPosition >= targetBeat)
                OnHit();   
        }

        private void OnHit()
        {
            SFXController.Instance.PlaySound();
            gameObject.SetActive(false); 
        }
    }
}
