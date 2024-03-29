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
        public double introSilenceBaseFiller; 

        public bool playBPMTest;

        [Header("-- Runtime Values")]
        // Flag for the state of the conductor.
        public SongState songState;

        public float songBpm;
        public double secPerBeat;

        // The timestamp of when the music started playing according to the Audio System timer.
        public double dspSongTime;

        // Song position on the timeline
        public double songPosition;
        public double songPositionInBeats;

        public AudioSource musicSource;

        // Source of the song data
        BeatMapData beatMapData;

        // Some songs don't start at 0:00:000 so we need to offset the start to set the beat to the right time.
        public double FirstBeatOffset => beatMapData ? beatMapData.FirstBeatOffset : 0;

        // How much earlier you need to spawn a hit-note before it reaches it's hit-time.
        public float BeatsShownInAdvance => beatMapData ? beatMapData.BeatsShownInAdvance : 0;

        // How "far" the note can move away from it's hit-spot before it's considered a "miss".
        public float MissThreshold => beatMapData ? beatMapData.MissThreshold : 0;
        
        // The "beatsShownInAdvance" as seconds-based value.
        public double TimeShownInAdvance => BeatsShownInAdvance * secPerBeat;

        // The beatmap timer has negative value during the intro silence, 
        // so the music should start playing at 0:00:000.
        // "firstBeatOffset" is used to "shift" the start of the song.
        public bool HasExitedTheIntro => songPosition + FirstBeatOffset >= 0;

        // Retorna um tempo de base pr�-definido + o tempo m�nimo para que uma nota seja gerada e fa�a o caminho at� o ponto de chegada
        private double IntroDuration 
        {
            get 
            {
                if (songBpm <= 0)
                    return introSilenceBaseFiller;
                else 
                    return introSilenceBaseFiller + (BeatsShownInAdvance * secPerBeat);
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

        #endregion

        // This class should be always set-up by the BeatMapCaller.
        public void Setup(BeatMapData beatMapData)
        {
            musicSource = GetComponent<AudioSource>();
            if (!musicSource) 
            {
                this.ShowErrorAndDisable("The music AudioSource couldn't be found.");
                return;
            }

            this.beatMapData = beatMapData;
            
            // Local values initialization
            // TO-DO: songBPM and firstBeatOffset should be able to change dynamicaly.
            { 
                musicSource.clip = beatMapData.Music;

                songBpm = beatMapData.BPM; 
                secPerBeat = 60d / songBpm;
            }
        }

        // The beatmap should always have a silenced intro so the notes have time to spawn,
        // so we start the conduction before the song itself.
        public void StartConduction()
        {
            // Saves the current timestamp so it can be used as an offset to the song time calculation.
            dspSongTime = AudioSettings.dspTime;
            songPosition = -999f;

            // Notify that the conductor is running, but the song is not playing yet.
            songState = SongState.Intro;
        }

        public void StartMusic() 
        {
            musicSource.Play();
            dspSongTime = AudioSettings.dspTime;

            songState = SongState.Playing;
        }

        void Update()
        {
            if (!HasInitiated) 
                return;

            switch (songState)
            {
                case SongState.Intro:
                    // -- AudioSettings.dspTime is constantly running during playtime, 
                    // -- so it's necessary to offset it with the timestamp of the beginning of the beatmap
                    songPosition = AudioSettings.dspTime - dspSongTime - IntroDuration;
                    break;

                case SongState.Playing:
                    //songPosition = AudioSettings.dspTime - dspSongTime; // -- Older method: may be necessary if audioclip changes dynamicaly 
                    songPosition = musicSource.time - FirstBeatOffset;    // -- Newer method: seems more consistent
                    break;

                case SongState.Outro:
                    break;
            }

            songPositionInBeats = songPosition / secPerBeat;
            
            if (playBPMTest)
                TestSoundBPM();

            // Starts playing the song at 0:00:000
            if (!SongIsPlaying && HasExitedTheIntro) 
                StartMusic();
        }

        int currentBeat = 0;
        private void TestSoundBPM()
        {
            if (songPositionInBeats < currentBeat)
                return;

            SFXController.Instance.PlaySound("Hit");
            currentBeat++;
        }
    }
}
