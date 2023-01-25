using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RhythmSystem 
{
    public class RhythmJudge : MonoBehaviour
    {
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
                cursor.localScale = Vector2.one * (Mathf.Abs(focusedNote.BeatTime - conductor.songPosition) < RANGE_RADIUS ? 1 : .8f);
            }

            // Lê input e tenta acertar nota
            if (Input.GetKeyDown(KeyCode.Space)) 
                HitNote(conductor.songPosition);
        }

        private void HitNote(float hitTime)
        {
            float difference = focusedNote.BeatTime - hitTime;
            //Debug.Log($"hit time: {hitTime}, difference: {difference}" );

            // Se a nota estiver muito longe ainda, nem tenta apertar
            if (Mathf.Abs(difference) > RANGE_RADIUS) 
                return;

            if (Mathf.Abs(difference) < .2f)
            {
                focusedNote.OnHit();
            }
            else 
            {
                focusedNote.OnMiss();
            }
        }
    }
}