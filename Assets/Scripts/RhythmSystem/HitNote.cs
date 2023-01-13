using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RhythmSystem 
{
    public class HitNote : MonoBehaviour
    {
        float targetBeat;
        float beatsShownInAdvance;
        Conductor conductor;
        Transform spawnAnchor;
        Transform targetAnchor;

        public void Setup (float targetBeat, float beatsShownInAdvance, Conductor conductor, Transform spawnAnchor, Transform targetAnchor)
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
            transform.position = Vector2.Lerp(
                spawnAnchor.position,
                targetAnchor.position,
                (beatsShownInAdvance - (targetBeat - conductor.songPositionInBeats)) / beatsShownInAdvance
            );    

            if (transform.position == targetAnchor.position)
            {
                SFXController.Instance.PlaySound();
                gameObject.SetActive(false);    
            }
        }
    }
}
