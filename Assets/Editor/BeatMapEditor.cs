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
    private Texture2D middleLineTexture;
    private Texture2D backgroundTexture;
        
    private int pixelsPerSecond = 75;

    private Vector2 timelineScroll;

    private Rect timelineRect;

    public override bool RequiresConstantRepaint() => true;

    public override void OnInspectorGUI()
    {
        if (soundInEditor == null || soundInEditor._AudioSource == null)
        {
            soundInEditor =
                (SoundInEditor)EditorGUILayout.ObjectField("Sound In Editor", soundInEditor, typeof(SoundInEditor), true);

            if (soundInEditor != null)
            {
                EditorGUILayout.LabelField("AudioSource");
                soundInEditor._AudioSource = (AudioSource)EditorGUILayout.ObjectField(soundInEditor._AudioSource, typeof(AudioSource), true);    
            }
        }

        if (GUILayout.Button("Reset Textures"))
        {
            waveFormTexture = null;
            middleLineTexture = null;
            backgroundTexture = null;
        }
            
        
        base.OnInspectorGUI();

        BeatMap obj = (BeatMap)target;
        
        if(soundInEditor == null)
            return;
        
        if(soundInEditor._AudioSource == null)
            return;

        audio = soundInEditor._AudioSource;

        if (audio.clip != obj.clip)
            audio.clip = obj.clip;

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
        
        int timelineH = 65;
        int timelineW = Mathf.CeilToInt(audio.clip.length * pixelsPerSecond);

        timelineScroll = GUILayout.BeginScrollView(timelineScroll, false, false); //S
        
        timelineRect = GUILayoutUtility.GetRect(timelineW, timelineH);

        if (timelineRect.width >= 1)
        {
            DrawTimeLine(timelineRect, obj);
        }

        GUILayout.EndScrollView(); //0S

        var scrollSpaceRect = GUILayoutUtility.GetLastRect();
        
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
        
        var mousePosScreen = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
        var canvasPosScreen = GUIUtility.GUIToScreenRect(scrollSpaceRect).position;

        bool mouseInsideTimeline = true;

        //var windowW = GUILayoutUtility.GetLastRect().width;
        
        var relativeMousePos = mousePosScreen-canvasPosScreen;

        if (relativeMousePos.x < 0 || relativeMousePos.y < 0)
            mouseInsideTimeline = false;
        if (relativeMousePos.x > scrollSpaceRect.width || relativeMousePos.y > timelineH )
            mouseInsideTimeline = false;

        relativeMousePos += timelineScroll;
        
        // GUILayout.Label($"last rect: {scrollSpaceRect.position} / {scrollSpaceRect.size}");
        // GUILayout.Label($"canvasPos: {canvasPosScreen}");
        // GUILayout.Label($"mousePos: {mousePosScreen}");
        // GUILayout.Label($"relative mouse: {relativeMousePos}");
        // GUILayout.Label($"timeline Scroll: {timelineScroll.x}");
        // GUILayout.Label($"lerp: {((relativeMousePos.x  / timelineRect.width)).ToString("0.00")}");
        // GUILayout.Label($"inside: {mouseInsideTimeline}");
        
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

    public void DrawTimeLine(Rect r, BeatMap beatMap)
    {
        int width = Mathf.CeilToInt(r.width);
        int height = Mathf.CeilToInt(r.height);

        int halfHeight = Mathf.CeilToInt(height / 2f);

        Color bgColor = new Color(0.1f, 0.1f, 0.1f);
        Color lineColor = Color.red;

        int markerLineSize = 1;

        float markerPos = (audio.time / audio.clip.length)*width;

        var bgTex = GetBackgroundTexture(width, height, bgColor);
        
        var wave = GetSoundWaveTexture(width);

        var dividerTex = GetMiddleLineTexture(width, height, beatMap.bpm);
        
        GUI.DrawTexture(r, bgTex, ScaleMode.ScaleAndCrop, true);
        GUI.DrawTexture(r, wave, ScaleMode.ScaleAndCrop, true);
        GUI.DrawTexture(r, dividerTex, ScaleMode.ScaleAndCrop, true);

        // var arrow = TextureDrawingUtil.GetArrow(7, halfHeight, 0.3f, Color.cyan);
        // var block = TextureDrawingUtil.GetFill(7, halfHeight, Color.magenta);

        // foreach (Tuple<float,BeatCall> beatMapCall in beatMap.calls)
        // {
        //     int x = Mathf.RoundToInt((beatMapCall.Item1 / audio.clip.length)*width) - 3;
        //     var a = new Rect(r.x+x, r.y, 7, halfHeight);
        //     GUI.DrawTexture(a, arrow);
        // }
        
        //BPM division
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
        
        GUI.DrawTexture(new Rect(Mathf.RoundToInt(markerPos), r.y, markerLineSize, markerHight), markerTex, ScaleMode.StretchToFill, true);
        
    }

    private Texture2D GetSoundWaveTexture(int lenght)
    {
        if (lenght == 1)
            return null;
        
        if (waveFormTexture != null)
            return waveFormTexture;
        
        Debug.Log("Redraw wave texture");
        
        waveFormTexture = AudioTrackPlotter.GetWaveform(Mathf.RoundToInt(lenght), 50, audio.clip, Color.clear, Color.yellow - new Color(0f,0f,0f,0.1f));

        return waveFormTexture;
    }

    private Texture2D GetMiddleLineTexture(int lenght, int height, int bpm)
    {
        if (height == 1 || lenght == 1)
            return null;
        
        if (middleLineTexture != null)
        {
            if(middleLineTexture.height == height && middleLineTexture.width == lenght)
                return middleLineTexture;
        }

        Debug.Log("Redraw middle line");
        
        Color dividerColor = new Color(0.0f,0.65f,0.65f, 0.8f);

        middleLineTexture = TextureDrawingUtil.GetFill(lenght, height, Color.clear);

        int halfH = Mathf.RoundToInt(height / 2f);
        int quarterH = Mathf.RoundToInt(height / 4f);
        int octH = Mathf.RoundToInt(height / 8f);
        
        middleLineTexture.ColorBlock(lenght, 2, dividerColor, 20, halfH);

        var beatSpacing = Mathf.FloorToInt((bpm / 60f) * pixelsPerSecond);
        
        for (int i = 0; i < lenght; i+=beatSpacing)
        {
            middleLineTexture.ColorBlock(1, halfH, dividerColor, i, quarterH);
        }
        
        return middleLineTexture;
    }

    public Texture2D GetBackgroundTexture(int lenght, int height, Color bgColor)
    {
        if (height == 1 || lenght == 1)
            return null;
        
        if (backgroundTexture != null)
        {
            if (backgroundTexture.height == height && backgroundTexture.width == lenght)
                return backgroundTexture;
        }
        
        Debug.Log("Redraw BG");
        
        backgroundTexture = TextureDrawingUtil.GetFill(lenght, height, bgColor);

        return backgroundTexture;

    }
    

}
