using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace RhythmSystem 
{
    // -- Class created following the code at:
    // -- https://www.gamedeveloper.com/audio/coding-to-the-beat---under-the-hood-of-a-rhythm-game-in-unity
    public class Conductor : MonoBehaviour
    {
        #region Variables

        // Base silence duration that should happen at the start of every beatmap. 
        public float introSilenceBaseFiller; 

        [Header("-- Runtime Values")]
        // Flag for the state of the conductor.
        public SongState songState;

        // Current BPM in use. This should be able to change dynamicaly during gameplay.
        public float songBpm;

        // Some songs don't start on the first second of the file, 
        // so we need to be able to cut-off the silence to start the beat at the right time.
        public float firstBeatOffset;

        // This variable defines how much earlier you need to spawn a hit-note before it reaches it's hit-time.
        public float beatsShownInAdvance;

        // Time between beats. This is necessary to translate beat-based timestamps to time-based ones.
        public float secPerBeat;

        // The timestamp of when the music started playing according to the Audio System timer.
        public float dspSongTime;

        // Song position on the timeline
        public float songPosition;
        public float songPositionInBeats;

        public AudioSource musicSource;
        public static Conductor Instance;

        // Source of the song data
        BeatMapData beatMapData;

        // How "far" the note can move away from it's hit-spot before it's considered a "miss".
        public float MissThreshold => beatMapData ? beatMapData.MissThreshold : 0;
        
        // The "beatsShownInAdvance" as seconds-based value.
        public float TimeShownInAdvance => beatsShownInAdvance * secPerBeat;

        // The beatmap timer has negative value during the intro silence, 
        // so the music should start playing at 0:00:000.
        // "firstBeatOffset" is used to "shift" the start of the song.
        public bool HasExitedTheIntro => songPosition + firstBeatOffset >= 0;

        // Retorna um tempo de base pré-definido + o tempo mínimo para que uma nota seja gerada e faça o caminho até o ponto de chegada
        private float IntroDuration 
        {
            get 
            {
                if (songBpm <= 0)
                    return introSilenceBaseFiller;
                else 
                    return introSilenceBaseFiller + (beatsShownInAdvance * secPerBeat);
            }
        }

        #endregion

        #region SongState

        public enum SongState 
        { 
            Stopped, // Is playing anymore
            Intro,   // Initial silence, "songPosition" is at a negative value
            Playing, // The audioSource is playing, "songPosition" is >= 0
            Outro    // The conductor received the flag that the beatmap ended
        }

        public bool HasInitiated => songState != SongState.Stopped;

        public bool SongIsPlaying => songState == SongState.Playing || songState == SongState.Outro;

        public IEnumerator WaitUntilHasInitiated (UnityAction action)
        {
            yield return new WaitUntil( () => HasInitiated );
            action();
        }

        #endregion

        // Singleton initiation
        private void Awake() 
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;  
        }

        // This class should be always set-up by the BeatMapCaller.
        public void SetupConductor(BeatMapData beatMapData)
        {
            musicSource = GetComponent<AudioSource>();
            if (!musicSource) 
            {
                Debug.LogError("The Music audioSource couldn't be found.");
                return;
            }

            this.beatMapData = beatMapData;
            
            // Local values initialization
            // TO-DO: songBPM and firstBeatOffset should be able to change dynamicaly.
            { 
                songBpm = beatMapData.BPM; 
                secPerBeat = 60f / songBpm;
            
                firstBeatOffset = beatMapData.FirstBeatOffset;
                beatsShownInAdvance = beatMapData.BeatsShownInAdvance;
            }
        }

        // The beatmap should always have a silenced intro so the notes have time to spawn,
        // so we start the conduction before the song itself.
        public void StartConduction()
        {
            // Saves the current timestamp so it can be used as an offset to the song time calculation.
            dspSongTime = (float) AudioSettings.dspTime;

            // Notify that the conductor is running, but the song is not playing yet.
            songState = SongState.Intro;
        }

        private void StartMusic() 
        {
            musicSource?.Play();
            songState = SongState.Playing;
        }

        void Update()
        {
            if (!HasInitiated) 
                return;

            // AudioSettings.dspTime is constantly running during playtime, 
            // so it's necessary to offset it with the "StartConduction()" timestamp
            songPosition = (float)(AudioSettings.dspTime - dspSongTime - IntroDuration - firstBeatOffset);
            songPositionInBeats = songPosition / secPerBeat;

            // Starts playing the song at 0:00:000
            if (!SongIsPlaying && HasExitedTheIntro) 
                StartMusic();
        }
    }
}
