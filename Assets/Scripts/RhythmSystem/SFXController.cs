using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RhythmSystem 
{
    public class SFXController : MonoBehaviour
    {
        public static SFXController Instance;
        AudioSource audioSource;

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
            audioSource = GetComponent<AudioSource>();
        }

        public void PlaySound()
        {
            if (audioSource)
                audioSource.Play();
        }
    }
}