using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace RhythmSystem {
    // -- Classe criada usando como base o link a seguir:
    // -- https://www.gamedeveloper.com/audio/coding-to-the-beat---under-the-hood-of-a-rhythm-game-in-unity
    public class Conductor : MonoBehaviour
    {
        [Header("-- Song Values")]
        // Batidas por minuto
        // Isto � um atributo originado pela m�sica da qual voc� est� tentando se sincronizar
        public float songBpm;

        // O atraso da primeira batida da m�sica em segundos 
        public float firstBeatOffset;

            // TO-DO: por enquanto recebe s� um valor fixo, depois deve fazer um c�lculo de acordo com o beatsShownInAdvance
            public float introSilenceBaseFiller; 

        [Header("-- Runtime Values")]
        // Var��vel que chamada por classes como BeatTrack e SyncedAnimation
        public SongState songState;

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

        public enum SongState 
        { 
            Stopped, // N�o come�ou a tocar ainda 
            Intro,   // Est� no sil�ncio inicial, antes do AudioSource ser chamado
            Playing, // AudioSource est� tocando a m�sica
            Outro    // AudioSource j� terminou de tocar a m�sica
        }

        public bool isRunning 
        {
            get { return songState != SongState.Stopped; }
        }

        public bool isPlaying 
        {
            get { return songState == SongState.Playing || songState == SongState.Outro; }
        }

        private void Awake() 
        {
            songState = SongState.Stopped;    
        }

        // TO-DO: tercerizar essa inicializa��o para outra classe
        void Start()
        {
            // Carrega o AudioSource do GameObject
            musicSource = GetComponent<AudioSource>();

            if (!musicSource) 
            {
                Debug.LogError("The Music audioSource couldn't be found.");
                return;
            }

            // Calcula o tempo em segundos em cada batida
            secPerBeat = 60f / songBpm;
        }

        private void StartConduction()
        {
            // Guarda o tempo quando a m�sica come�a a tocar
            dspSongTime = (float) AudioSettings.dspTime;

            if (introSilenceBaseFiller <= 0) 
            {
                StartMusic();
            }
            else 
            {
                // Ativa a flag de in�cio do condutor
                songState = SongState.Intro;
            }
        }

        private void StartMusic() 
        {
            // Inicia a m�sica
            musicSource.Play();

            // Ativa a flag de in�cio do condutor
            songState = SongState.Playing;
        }

        void Update()
        {
            // Aguarda pelo input do jogador para come�ar a rodar
            if (!isRunning) 
            {
                if (Input.GetKeyDown(KeyCode.Return))
                    StartConduction();
                return;
            }

            // Atualiza quantos segundos passaram desde que a m�sica come�ou
            songPosition = (float)(AudioSettings.dspTime - dspSongTime - firstBeatOffset - introSilenceBaseFiller);

            // Atualiza quantas batidas passaram desde que a m�sica come�ou
            songPositionInBeats = songPosition / secPerBeat;

            // Verifica se j� passou do tempo de sil�ncio da Intro
            if (!isPlaying) 
            {
                if (songPosition >= 0)
                    StartMusic();
                return;
            }
        }

        // Aguarda "isPlaying" se tornar "true" para chama a UnityAction 
        public static IEnumerator WaitForConductor(Conductor conductor, UnityAction action)
        {
            yield return new WaitUntil( () => conductor.isRunning );
            action();
        }
    }
}
