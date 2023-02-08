using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class CallSoundController : MonoBehaviour
{
    public AudioMixerGroup audioMixerGroup;

    public class CallData 
    {
        public CallData (GameObject gameObject, AudioMixerGroup audioMixerGroup, string tag, AudioClip clip, float volume)
        {
            this.tag = tag;
            
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = clip;
            audioSource.volume = volume;
            audioSource.outputAudioMixerGroup = audioMixerGroup;
            audioSource.playOnAwake = false;
            audioSource.loop = false;
        }

        public void PlayAudio()
        {
            if (!audioSource)
                return;
            audioSource.Play();
        }

        public string tag;
        public AudioSource audioSource;
    }

    List<CallData> callDatas;

    public void AddCallData (string tag, AudioClip clip, float volume)
    {
        if (callDatas == null)
            callDatas = new List<CallData>();

        callDatas.Add( new CallData(gameObject, audioMixerGroup, tag, clip, volume) );
    }

    public void PlayCallSound (string tag)
    {
        if (callDatas == null)
            return;

        CallData callData = callDatas.Find( (data) => data.tag == tag );
        if (callData == null)
            return;

        callData.PlayAudio();
    }
}
