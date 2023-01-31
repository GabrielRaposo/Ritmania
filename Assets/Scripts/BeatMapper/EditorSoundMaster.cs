using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

[ExecuteInEditMode]
public class EditorSoundMaster : MonoBehaviour
{
    private Dictionary<AudioClip, SoundInEditor> soundControllers;
    [SerializeField] private AudioClip beepClip;

    private SoundInEditor beepController;
    private SoundInEditor mainAudioController;

    private BeatMapper currentMapper;

    private bool initialized  => beepController != null && mainAudioController != null;
    private void OnEnable()
    {
        if(initialized)
            return;

        soundControllers = new Dictionary<AudioClip, SoundInEditor>();

        beepController = NewSoundInEditor(null, false);
        beepController._AudioSource.clip = beepClip;

        mainAudioController = NewSoundInEditor(null, false);
        
    }

    public void SetMapper(BeatMapper mapper)
    {
        OnEnable();
        
        if(currentMapper == mapper)
            return;

        currentMapper = mapper;
        
        ClearControllers();

        foreach (BeatCall call in mapper.callTypes)
        {
            if (call.answerClip != null)
            {
                if(!soundControllers.ContainsKey(call.answerClip));
                    NewSoundInEditor(call.answerClip, true);
            }

            if (call.callClip != null)
            {
                if(!soundControllers.ContainsKey(call.callClip))
                    NewSoundInEditor(call.callClip, true);
            }
            
        }
        
    }

    public void UpdateMapper(BeatMapper mapper)
    {
        if(soundControllers == null)
            return;
        
        var allClips = soundControllers.Keys.ToList();

        for (int i = allClips.Count - 1; i >= 0; i--)
        {
            var clip = allClips[i];
            var match = mapper.callTypes.Find(a => a.callClip == clip || a.answerClip == clip);
            if(match == null)
                RemoveController(clip);
        }

        for (int i = 0; i < mapper.callTypes.Count; i++)
        {
            var call = mapper.callTypes[i];
            
            if(call.answerClip!=null)
                if (!soundControllers.ContainsKey(call.answerClip))
                    NewSoundInEditor(call.answerClip, true);
            
            if(call.callClip!=null)
                if (!soundControllers.ContainsKey(call.callClip))
                    NewSoundInEditor(call.callClip, true);
        }
        
    }

    
    
    private void ClearControllers()
    {
        if(soundControllers == null)
            return;

        var list = soundControllers.Keys.ToList();

        for (int i = list.Count - 1; i >= 0; i--)
        {
            RemoveController(list[i]);    
        }
    }

    private SoundInEditor NewSoundInEditor(AudioClip clip, bool addToDictionary)
    {
        var go = new GameObject();
        go.transform.SetParent(transform);
        var audioSource = go.AddComponent<AudioSource>();
        var sie = go.AddComponent<SoundInEditor>();

        sie._AudioSource = audioSource;
        sie._AudioSource.clip = clip;
        
        if(addToDictionary)
            if(clip!= null)
                soundControllers.Add(clip, sie);
        
        return sie;
    }

    private void RemoveController(AudioClip clip)
    {
        if(!soundControllers.ContainsKey(clip))
            return;

        soundControllers.TryGetValue(clip, out SoundInEditor toRemove);

        if(toRemove!= null)
            DestroyImmediate(toRemove.gameObject);
        
        soundControllers.Remove(clip);
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

    public AudioSource GetMainSoundSource()
    {
        if (mainAudioController == null)
            return null;

        return mainAudioController._AudioSource;
    }
    
    public AudioSource GetBeepSource()
    {
        if (beepController == null)
            return null;

        return beepController._AudioSource;
    }
    
    public void OnValidate()
    {
        if (!initialized)
            OnEnable();
        
        beepController._AudioSource.clip = beepClip;
    }
}
