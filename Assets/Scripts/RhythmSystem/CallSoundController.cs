using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

// Chamadas de som da Cue vem com a tag da Call, como por exemplo: "MyCall"
// Chamadas de som do Hit vem com a tag, mais a posição na lista, como: "MyCall0", "MyCall1", etc.
// Chamadas de som de Miss vem com a tag do hit, mais o texto "Miss", como: "MyCall0Miss", MyCall1Miss", etc.
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

        Debug.Log("Play " + tag);

        callData.PlayAudio();
    }
}
