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

        BeatCallsEditor(obj);
        
        GUILayout.Space(5);
        
        GUILayout.BeginHorizontal();//A
        
        if (GUILayout.Button("▶️"))
            audio.Play();

        if (GUILayout.Button("P"))
        {
            audio.Pause();
        }

        GUILayout.EndHorizontal(); //A0

        //Time line
        GUILayout.BeginVertical(GUILayout.Height(65));
        GUILayout.Label(".");
        GUILayout.EndVertical();
        
        var r = GUILayoutUtility.GetLastRect();
        DrawTimeLine(r, obj);
        
        var timelineRect = GUILayoutUtility.GetLastRect();
        
        GUILayout.Label($"{audio.time/audio.clip.length}");
        
        //Calls Buttons
        GUILayout.BeginHorizontal(); //B
        for (int i = 0; i < obj.callTypes.Count; i++)
        {
            int index = i;

            if (GUILayout.Button($"{index}", GUILayout.MaxWidth(17)))
            {
                obj.calls.Add(new Tuple<float, BeatCall>(audio.time, obj.callTypes[index]));
            }
        }
        GUILayout.EndHorizontal(); //B0

        //Controles
        
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

    private void BeatCallsEditor(BeatMap obj)
    {
        if (obj.callTypes == null)
            obj.callTypes = new List<BeatCall>();

        if (obj.calls == null)
            obj.calls = new List<Tuple<float, BeatCall>>();

        for (int i = 0; i < obj.callTypes.Count; i++)
        {
            var call = obj.callTypes[i];
            GUILayout.Label($"{i}:");

            call.awnserCount = EditorGUILayout.IntField("Awnser Count", call.awnserCount);
            call.awnserDistance = EditorGUILayout.FloatField("Distance", call.awnserDistance);
            call.awnsersSpacing = EditorGUILayout.IntField("Spacing", call.awnsersSpacing);
        }
        
        if(GUILayout.Button("Add +"))
            obj.callTypes.Add(new BeatCall());
        
    }

    public void DrawTimeLine(Rect lastRect, BeatMap beatMap)
    {
        int width = Mathf.CeilToInt(lastRect.width);
        int height = Mathf.CeilToInt(lastRect.height);

        int halfHeight = Mathf.CeilToInt(height / 2f);

        var r = lastRect;
        
        Color bgColor = new Color(0.55f, 0.55f, 0.55f);
        Color dividerColor = new Color(0f,0f,0f, 0.4f);
        Color lineColor = Color.red;

        int markerLineSize = 1;
        int dividerLineSize = 2;

        float markerPos = (audio.time / audio.clip.length)*width;
        
        var bgTex = TextureDrawingUtil.GetFill(width, height, bgColor);
        
        var wave = GetSoundWaveTexture(width);

        var dividerTex = TextureDrawingUtil.GetFill(width, dividerLineSize, dividerColor);
        
        GUI.DrawTexture(r, bgTex, ScaleMode.ScaleAndCrop, true);
        GUI.DrawTexture(r, wave, ScaleMode.ScaleAndCrop, true);
        GUI.DrawTexture(new Rect(r.x, Mathf.RoundToInt(r.y+(r.height/2f)-dividerLineSize/2f), width, dividerLineSize), dividerTex, ScaleMode.ScaleAndCrop, true);

        var arrow = TextureDrawingUtil.GetArrow(7, halfHeight, 0.3f, Color.cyan);
        var block = TextureDrawingUtil.GetFill(7, halfHeight, Color.magenta);

        foreach (Tuple<float,BeatCall> beatMapCall in beatMap.calls)
        {
            int x = Mathf.RoundToInt((beatMapCall.Item1 / audio.clip.length)*width) - 3;
            var a = new Rect(r.x+x, r.y, 7, halfHeight);
            GUI.DrawTexture(a, arrow);
        }
        
        int markerHight = height;
        var markerTex = new Texture2D(markerLineSize, markerHight, TextureFormat.RGBA32, false);
        
        for (int i = 0; i < markerLineSize; i++)
        {
            for (int j = 0; j < markerHight; j++)
            {
                markerTex.SetPixel(i,j,lineColor);
            }
        }
        
        markerTex.Apply();
        
        GUI.DrawTexture(new Rect(Mathf.RoundToInt(markerPos)+18, r.y, markerLineSize, markerHight), markerTex, ScaleMode.StretchToFill, true);
        
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
