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
        // Isto é um atributo originado pela música da qual você está tentando se sincronizar
        public float songBpm;

            public float introSilence;

        [Header("Runtime Values")]
        // Varíável que chamada por classes como BeatTrack e SyncedAnimation
        public bool isPlaying;

        // O atraso da primeira batida da música em segundos 
        public float firstBeatOffset;

        // O valor de segundos para cara batida da música
        public float secPerBeat;

        // Posição na linha do tempo atual da música (em segundos)
        public float songPosition;

        // Posição na linha do tempo atual da música (em batidas)
        public float songPositionInBeats;

        // Quantos segundos passaram desde que a música começou
        public float dspSongTime;

        // AudioSource onde toca a música
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
            // Guarda o tempo quando a música começa a tocar
            dspSongTime = (float) AudioSettings.dspTime;

            // Inicia a música
            musicSource.Play();

            // Ativa a flag de início do condutor
            isPlaying = true;
        }

        void Update()
        {
            // Aguarda pelo input do jogador para começar a rodar
            if (!isPlaying) 
            {
                if (Input.GetKeyDown(KeyCode.Return))
                    Setup();
                return;
            }

            // Atualiza quantos segundos passaram desde que a música começou
            songPosition = (float)(AudioSettings.dspTime - dspSongTime - firstBeatOffset);

            // Atualiza quantas batidas passaram desde que a música começou
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
