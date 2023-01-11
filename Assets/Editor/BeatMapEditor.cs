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

        var r = GUILayoutUtility.GetLastRect();
        
        
        GUILayout.Label(GetTimeLineDrawing(Mathf.RoundToInt(r.width)));
        
        var timelineRect = GUILayoutUtility.GetLastRect();
        
        GUILayout.Label($"{audio.time/audio.clip.length}");

        var mousePos = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
        var canvasPos = GUIUtility.GUIToScreenRect(timelineRect).position;

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

    public Texture2D GetTimeLineDrawing(int lenght)
    {
        int x = lenght;
        int y = 50;

        Color bg = new Color(0.7f, 0.7f, 0.7f);
        Color lineColor = Color.red;

        float lineSize = 1;

        float marker = (audio.time / audio.clip.length)*lenght;
        float t1;
        float t2;
        
        var tex = new Texture2D(lenght, y, TextureFormat.RGBA32, false);
        
        var wave =AudioTrackPlotter.GetWaveform(Mathf.RoundToInt(lenght), 50, audio.clip, Color.clear, Color.yellow);

        for (int i = 0; i < x; i++)
        {
            t1 = marker - lineSize;
            t2 = marker + lineSize;
            //Debug.Log($"t1:{t1} / {marker} / t2:{t2}");
            
            for (int j = 0; j < y; j++)
            {
                Color c; 
                
                if(i > t1 && i < t2)
                    c = lineColor;
                else
                    c = bg;

                c += ( wave.GetPixel(i, j) * 0.2f);
                
                tex.SetPixel(i,j,c);
                
                
            }
        }
        
        tex.Apply();
        
        


        return tex;

    }
    

}
