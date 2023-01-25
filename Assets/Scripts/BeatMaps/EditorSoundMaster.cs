using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

[ExecuteInEditMode]
public class EditorSoundMaster : MonoBehaviour
{
    private Dictionary<AudioClip, SoundInEditor> soundControllers;
    [SerializeField] private AudioClip beepClip;

    public SoundInEditor beepController;
    public SoundInEditor mainAudioController;
    private void OnEnable()
    {
        if(soundControllers != null)
            return;

        soundControllers = new Dictionary<AudioClip, SoundInEditor>();

        beepController = NewSoundInEditor(null);
        beepController._AudioSource.clip = beepClip;

        mainAudioController = NewSoundInEditor(null);
    }

    public void AddController(AudioClip clip)
    {
        var sie = NewSoundInEditor(clip);

        soundControllers.Add(clip, sie);
    }

    private SoundInEditor NewSoundInEditor(AudioClip clip)
    {
        var go = new GameObject();
        go.transform.SetParent(transform);
        var audioSource = go.AddComponent<AudioSource>();
        var sie = go.AddComponent<SoundInEditor>();

        sie._AudioSource = audioSource;
        sie._AudioSource.clip = clip;
        return sie;
    }

    public AudioSource GetAudioSource(AudioClip clip)
    {
        if (soundControllers == null)
            return null;

        soundControllers.TryGetValue(clip, out SoundInEditor a);

        if (a == null)
            return null;

        return a._AudioSource;
    }

    public AudioSource GetBeepSource()
    {
        if (beepController == null)
            return null;

        return beepController._AudioSource;
    }
    
    public void OnValidate()
    {
        beepController._AudioSource.clip = beepClip;
    }
}
