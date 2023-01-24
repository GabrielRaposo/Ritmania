using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

[CustomEditor(typeof(BeatMapper))]
public class BeatMapperEditor : Editor
{
    public SoundInEditor soundInEditorMain;
    public SoundInEditor soundInEditorBeep;

    private AudioSource audio;

    private Texture2D waveFormTexture;
    private Texture2D middleLineTexture;
    private Texture2D backgroundTexture;
        
    private int pixelsPerSecond = 75;

    private Vector2 timelineScroll;

    private Rect timelineRect;

    //StoredValues
    private AudioClip previousClip;
    private int previousBpm;
    
    private float previousTime;
    private int beatIndex;


    public override bool RequiresConstantRepaint() => true;

    public override void OnInspectorGUI()
    {
        if (soundInEditorMain == null || soundInEditorMain._AudioSource == null || soundInEditorBeep == null)
        {
            soundInEditorMain =
                (SoundInEditor)EditorGUILayout.ObjectField("Sound In Editor", soundInEditorMain, typeof(SoundInEditor), true);
            
            soundInEditorBeep =
                (SoundInEditor)EditorGUILayout.ObjectField("Beed Sound", soundInEditorBeep, typeof(SoundInEditor), true);

            if (soundInEditorMain != null)
            {
                EditorGUILayout.LabelField("AudioSource");
                soundInEditorMain._AudioSource = (AudioSource)EditorGUILayout.ObjectField(soundInEditorMain._AudioSource, typeof(AudioSource), true);    
            }
        }

        if (GUILayout.Button("Reset Textures"))
            ResetTextures();
            
        
        base.OnInspectorGUI();

        BeatMapper obj = (BeatMapper)target;
        
        if(soundInEditorMain == null)
            return;
        
        if(soundInEditorMain._AudioSource == null)
            return;

        audio = soundInEditorMain._AudioSource;

        if (audio.clip != obj.clip)
            audio.clip = obj.clip;

        //Values Check
        if (previousClip != obj.clip || previousBpm != obj.bpm)
            OnChangeClip(obj);


        var time = audio.time;

        if (audio.isPlaying)
        {
            if (time > (beatIndex + 1) * obj.BeatLenght)
            {
                if(soundInEditorBeep!=null)
                    soundInEditorBeep._AudioSource.Play();
                //Debug.Log($"BEAT:({time}) [{beatIndex+1}]");
            }
                
        }
        
        beatIndex = Mathf.FloorToInt(previousTime / obj.BeatLenght);
        previousTime = time;
        
        //BeatCallsEditor(obj);
        
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

    private void ResetTextures()
    {
        waveFormTexture = null;
        middleLineTexture = null;
        backgroundTexture = null;
    }

    private void BeatCallsEditor(BeatMapper obj)
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

    public void DrawTimeLine(Rect r, BeatMapper beatMapper)
    {
        int width = Mathf.CeilToInt(r.width);
        int height = Mathf.CeilToInt(r.height);

        int halfHeight = Mathf.CeilToInt(height / 2f);

        Color bgColor = new Color(0.1f, 0.1f, 0.1f);
        Color lineColor = Color.red;

        int markerLineSize = 1;

        float markerPos = (audio.time / audio.clip.length)*width;

        var bgTex = GetBackgroundTexture(width, height, bgColor);
        
        var wave = GetSoundWaveTexture(width, height);

        var dividerTex = GetMiddleLineTexture(width, height, beatMapper.bpm);
        
        GUI.DrawTexture(r, bgTex, ScaleMode.ScaleAndCrop, true);
        GUI.DrawTexture(r, wave, ScaleMode.ScaleAndCrop, true);
        GUI.DrawTexture(r, dividerTex, ScaleMode.ScaleAndCrop, true);

        // var arrow = TextureDrawingUtil.GetArrow(7, halfHeight, 0.3f, Color.cyan);
        // var block = TextureDrawingUtil.GetFill(7, halfHeight, Color.magenta);

        // foreach (Tuple<float,BeatCall> beatMapCall in beatMapper.calls)
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

    private Texture2D GetSoundWaveTexture(int lenght, int height)
    {
        if (lenght == 1)
            return null;
        
        if (waveFormTexture != null)
            return waveFormTexture;
        
        Debug.Log("Redraw wave texture");
        
        waveFormTexture = AudioTrackPlotter.GetWaveform(Mathf.RoundToInt(lenght), height, audio.clip, Color.clear, Color.yellow - new Color(0f,0f,0f,0.68f));

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
        
        middleLineTexture.ColorBlock(lenght, 1, dividerColor, 0, halfH);

        float nOfBeats = (audio.clip.length / 60f) * bpm*4;
        int beatSize = Mathf.FloorToInt(lenght / nOfBeats);
        
        for (int i = 1; i < nOfBeats; i++)
        {
            if(i%4==0)
                middleLineTexture.ColorBlock(1, halfH, dividerColor, i*beatSize, quarterH);
            else
                middleLineTexture.ColorBlock(1, quarterH, dividerColor*new Color(0.7f,0.7f,0.7f), i*beatSize, quarterH+octH);
        }
        
        return middleLineTexture;
    }

    private Texture2D GetBackgroundTexture(int lenght, int height, Color bgColor)
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

    private void OnChangeClip(BeatMapper obj)
    {
        Debug.Log("On Change Clip");
        beatIndex = 0;
        previousClip = audio.clip;
        previousBpm = obj.bpm;
        ResetTextures();
    }


}
