using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace RhythmSystem 
{
    // Classe auxiliar com função de centralizar chamadas de sons para facilitar testes
    public class SFXController : MonoBehaviour
    {
        [System.Serializable]
        public class SFXPlayer 
        {
            public string tag;
            public AudioClip audioClip;
            public AudioMixerGroup mixerGroup;
            public float volume;
            public float pitch;

            [HideInInspector] public AudioSource source;
        }

        public List<SFXPlayer> sfxPlayers;

        public static SFXController Instance;


        private void Awake() 
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        void Start()
        {
            if (sfxPlayers == null || sfxPlayers.Count < 1)
            {
                enabled = false;
                return;
            }

            for (int i = 0; i < sfxPlayers.Count; i++) 
            {
                SFXPlayer sfxPlayer = sfxPlayers[i];

                AudioSource source = gameObject.AddComponent<AudioSource>();
                source.clip = sfxPlayer.audioClip;
                source.outputAudioMixerGroup = sfxPlayer.mixerGroup;
                source.volume = sfxPlayer.volume;
                source.pitch = sfxPlayer.pitch;
                source.playOnAwake = false;

                sfxPlayers[i].source = source;
            }
        }

        public void PlaySound(string tag)
        {
            if (sfxPlayers == null || sfxPlayers.Count < 1)
                return;

            AudioSource audioSource = null;
            foreach (SFXPlayer sfxPlayer in sfxPlayers) 
            {
                if ( sfxPlayer.tag.Trim().CompareTo( tag.Trim() ) == 0 )
                {
                    Debug.Log("in! sfxPlayer.source: " + sfxPlayer.source);
                    audioSource = sfxPlayer.source;
                    break;
                }
            }

            if (audioSource)
            {
                audioSource.Play();
            }
        }
    }
}