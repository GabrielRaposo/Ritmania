using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BeatMap))]
public class BeatMapEditor : Editor
{
    public SoundInEditor soundInEditor;

    private AudioSource audio;

    private Texture2D waveFormTexture;
    private int waveFormWidth;

    public override bool RequiresConstantRepaint() => true;

    public override void OnInspectorGUI()
    {
        soundInEditor =
            (SoundInEditor)EditorGUILayout.ObjectField("Sound In Editor", soundInEditor, typeof(SoundInEditor), true);
        
        base.OnInspectorGUI();

        BeatMap obj = (BeatMap)target;
        
        if(soundInEditor == null)
            return;
        
        if(soundInEditor._AudioSource == null)
            return;

        audio = soundInEditor._AudioSource;

        if (audio.clip != obj.clip)
            audio.clip = obj.clip;


        EditorGUILayout.LabelField("AudioSource");
        soundInEditor._AudioSource = (AudioSource)EditorGUILayout.ObjectField(soundInEditor._AudioSource, typeof(AudioSource), true);
        
        GUILayout.BeginHorizontal();//A
        
        if (GUILayout.Button("▶️"))
            audio.Play();

        if (GUILayout.Button("P"))
        {
            audio.Pause();
        }

        GUILayout.EndHorizontal(); //A0

        GUILayout.BeginVertical(GUILayout.Height(50));
        GUILayout.Label(".");
        GUILayout.EndVertical();
        
        var r = GUILayoutUtility.GetLastRect();
        DrawTimeLine(r);
        
        
        var timelineRect = GUILayoutUtility.GetLastRect();
        
        GUILayout.Label($"{audio.time/audio.clip.length}");

        var mousePos = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
        var canvasPos = GUIUtility.GUIToScreenRect(timelineRect).position;

        //Controles
        bool mouseInsideTimeline = true;

        var relativeMousePos = mousePos-canvasPos;

        if (relativeMousePos.x < 0 || relativeMousePos.y < 0)
            mouseInsideTimeline = false;
        if (relativeMousePos.x > timelineRect.width || relativeMousePos.y > timelineRect.height)
            mouseInsideTimeline = false;

        if (mouseInsideTimeline)
        {
            if (Event.current.type is EventType.MouseDown or EventType.MouseDrag)
            {
                //audio.Pause();
                audio.time = (relativeMousePos.x / timelineRect.width) * audio.clip.length;
            }

        }
    }

    public void DrawTimeLine(Rect lastRect)
    {
        int x = Mathf.CeilToInt(lastRect.width);
        int y = 50;

        var r = lastRect;
        
        Color bg = new Color(0.55f, 0.55f, 0.55f);
        Color lineColor = Color.red;

        int lineSize = 1;

        float marker = (audio.time / audio.clip.length)*x;
        
        var bgTex = new Texture2D(x, y, TextureFormat.RGBA32, false);

        for (int i = 0; i < x; i++)
        {
            for (int j = 0; j < y; j++)
            {
                bgTex.SetPixel(i,j,bg);
            }
        }
        
        bgTex.Apply();
        var wave = GetSoundWaveTexture(x);
        
        GUI.DrawTexture(r, bgTex, ScaleMode.ScaleAndCrop, true);
        GUI.DrawTexture(r, wave, ScaleMode.ScaleAndCrop, true);

        int markerHight = y;
        var markerTex = new Texture2D(lineSize, markerHight, TextureFormat.RGBA32, false);
        
        for (int i = 0; i < lineSize; i++)
        {
            for (int j = 0; j < markerHight; j++)
            {
                markerTex.SetPixel(i,j,lineColor);
            }
        }
        
        markerTex.Apply();
        
        GUI.DrawTexture(new Rect(Mathf.RoundToInt(marker)+18, r.y, lineSize, markerHight), markerTex, ScaleMode.StretchToFill, true);
        
    }

    private Texture2D GetSoundWaveTexture(int lenght)
    {
        if (waveFormTexture != null && waveFormWidth == lenght)
            return waveFormTexture;
        
        waveFormTexture = AudioTrackPlotter.GetWaveform(Mathf.RoundToInt(lenght), 50, audio.clip, Color.clear, Color.yellow - new Color(0f,0f,0f,0.1f));
        waveFormWidth = lenght;

        return waveFormTexture;

    } 
    

}
