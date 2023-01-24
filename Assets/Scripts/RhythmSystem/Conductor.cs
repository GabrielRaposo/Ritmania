using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace RhythmSystem 
{
    // -- Classe criada usando como base o link a seguir:
    // -- https://www.gamedeveloper.com/audio/coding-to-the-beat---under-the-hood-of-a-rhythm-game-in-unity
    public class Conductor : MonoBehaviour
    {
        #region Variables

        // TO-DO: por enquanto recebe só um valor fixo, depois deve fazer um cálculo de acordo com o beatsShownInAdvance
        public float introSilenceBaseFiller; 

        [Header("-- Runtime Values")]
        // Batidas por minuto
        // Isto é um atributo originado pela música da qual você está tentando se sincronizar
        public float songBpm;

        // O atraso da primeira batida da música em segundos 
        public float firstBeatOffset;

        // TO-DO: comentar ----------------------
        public float beatsShownInAdvance;

        // Varíável que chamada por classes como BeatTrack e SyncedAnimation
        public SongState songState;

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

        // Singleton do Conductor
        public static Conductor Instance;

        #endregion

        #region SongState

        public enum SongState 
        { 
            Stopped, // Não começou a tocar ainda 
            Intro,   // Está no silêncio inicial, antes do AudioSource ser chamado
            Playing, // AudioSource está tocando a música
            Outro    // AudioSource já terminou de tocar a música
        }

        public bool isRunning 
        {
            get { return songState != SongState.Stopped; }
        }

        public bool isPlaying 
        {
            get { return songState == SongState.Playing || songState == SongState.Outro; }
        }

        #endregion

        private void Awake() 
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            songState = SongState.Stopped;    
        }

        //// TO-DO: tercerizar essa inicialização para outra classe
        //void Start()
        //{
        //    SetupConductor(songBpm);
        //}

        public void SetupConductor(BeatMapData beatMapData)
        {
            // Carrega o AudioSource do GameObject
            musicSource = GetComponent<AudioSource>();

            if (!musicSource) 
            {
                Debug.LogError("The Music audioSource couldn't be found.");
                return;
            }

            songBpm = beatMapData.BPM;
            firstBeatOffset = beatMapData.FirstBeatOffset;
            beatsShownInAdvance = beatMapData.BeatsShownInAdvance;

            // Calcula o tempo em segundos em cada batida
            secPerBeat = 60f / songBpm;
        }

        public void StartConduction()
        {
            // Guarda o tempo quando a música começa a tocar
            dspSongTime = (float) AudioSettings.dspTime;

            // Ativa a flag de início do condutor
            songState = SongState.Intro;
        }

        private void StartMusic() 
        {
            // Inicia a música
            musicSource.Play();

            // Ativa a flag de início da música
            songState = SongState.Playing;
        }

        // Retorna um tempo de base pré-definido + o tempo mínimo para que uma nota seja gerada e faça o caminho até o ponto de chegada
        private float IntroDuration 
        {
            get {
                if (songBpm <= 0)
                    return introSilenceBaseFiller;
                else 
                    return introSilenceBaseFiller + (beatsShownInAdvance * secPerBeat);
            }
        }

        void Update()
        {
            if (!isRunning) 
                return;

            // Atualiza quantos segundos passaram desde que a música começou
            songPosition = (float)(AudioSettings.dspTime - dspSongTime - firstBeatOffset - IntroDuration);

            // Atualiza quantas batidas passaram desde que a música começou
            songPositionInBeats = songPosition / secPerBeat;

            // Verifica se já passou do tempo de silêncio da Intro
            if (!isPlaying) 
            {
                if (songPosition + firstBeatOffset >= 0)
                    StartMusic();
                return;
            }
        }

        // Aguarda "isRunning" se tornar "true" para chama a UnityAction 
        public IEnumerator WaitUntilIsRunning(UnityAction action)
        {
            yield return new WaitUntil( () => isRunning );
            action();
        }
    }
}
