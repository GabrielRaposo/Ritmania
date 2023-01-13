using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RhythmSystem 
{
    public class HitNote : MonoBehaviour
    {
        double targetBeat;
        float beatsShownInAdvance;
        Conductor conductor;
        Transform spawnAnchor;
        Transform targetAnchor;

        public void Setup (double targetBeat, float beatsShownInAdvance, Conductor conductor, Transform spawnAnchor, Transform targetAnchor)
        {
            this.targetBeat = targetBeat;
            this.beatsShownInAdvance = beatsShownInAdvance;

            this.conductor = conductor;
            this.spawnAnchor = spawnAnchor;
            this.targetAnchor = targetAnchor;

            transform.position = spawnAnchor.position;
        }

        void Update()
        {
            double t = (targetBeat - conductor.songPositionInBeats);
            transform.position = Vector2.Lerp(
                spawnAnchor.position,
                targetAnchor.position,
                (beatsShownInAdvance - (float)t) / beatsShownInAdvance
            );    

            // Se estiver com AutoPlay, acerta a nota no primeiro frame válido
            if (GameManager.AutoPlay && conductor.songPositionInBeats >= targetBeat)
                OnHit();   
        }

        private void OnHit()
        {
            SFXController.Instance.PlaySound();
            gameObject.SetActive(false); 
        }
    }
}
