using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace RhythmSystem {
    // -- Classe criada usando como base o link a seguir:
    // -- https://www.gamedeveloper.com/audio/coding-to-the-beat---under-the-hood-of-a-rhythm-game-in-unity
    public class Conductor : MonoBehaviour
    {
        // Batidas por minuto
        // Isto � um atributo originado pela m�sica da qual voc� est� tentando se sincronizar
        public float songBpm;

            public float introSilence;

        [Header("Runtime Values")]
        // Var��vel que chamada por classes como BeatTrack e SyncedAnimation
        public bool isPlaying;

        // O atraso da primeira batida da m�sica em segundos 
        public float firstBeatOffset;

        // O valor de segundos para cara batida da m�sica
        public float secPerBeat;

        // Posi��o na linha do tempo atual da m�sica (em segundos)
        public float songPosition;

        // Posi��o na linha do tempo atual da m�sica (em batidas)
        public float songPositionInBeats;

        // Quantos segundos passaram desde que a m�sica come�ou
        public float dspSongTime;

        // AudioSource onde toca a m�sica
        public AudioSource musicSource;

        void Start()
        {
            // Carrega o AudioSource do GameObject
            musicSource = GetComponent<AudioSource>();

            // Calcula o tempo em segundos em cada batida
            secPerBeat = 60f / songBpm;
        }

        private void Setup()
        {
            // Guarda o tempo quando a m�sica come�a a tocar
            dspSongTime = (float) AudioSettings.dspTime;

            // Inicia a m�sica
            musicSource.Play();

            // Ativa a flag de in�cio do condutor
            isPlaying = true;
        }

        void Update()
        {
            // Aguarda pelo input do jogador para come�ar a rodar
            if (!isPlaying) 
            {
                if (Input.GetKeyDown(KeyCode.Return))
                    Setup();
                return;
            }

            // Atualiza quantos segundos passaram desde que a m�sica come�ou
            songPosition = (float)(AudioSettings.dspTime - dspSongTime - firstBeatOffset);

            // Atualiza quantas batidas passaram desde que a m�sica come�ou
            songPositionInBeats = songPosition / secPerBeat;
        }

        // Aguarda "isPlaying" se tornar "true" para chama a UnityAction 
        public static IEnumerator WaitForConductor(Conductor conductor, UnityAction action)
        {
            yield return new WaitWhile( () => conductor.isPlaying! );
            action();
        }
    }
}
