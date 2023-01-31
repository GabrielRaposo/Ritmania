using System;
using System.Collections;
using System.Collections.Generic;
using Mono.Cecil;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

[CustomEditor(typeof(BeatMapper))]
public class BeatMapperEditor : Editor
{
    public EditorSoundMaster EditorSound;

    private AudioSource mainAudio;
    private AudioSource beepAudio;

    private Texture2D waveFormTexture;
    private Texture2D middleLineTexture;
    private Texture2D backgroundTexture;
    private Texture2D combinedArrowsTexture;
        
    private int pixelsPerSecond = 85;

    private Vector2 timelineScroll;
    private Vector2 callEditorScroll;

    private Rect timelineRect;

    //StoredValues
    private AudioClip previousClip;
    private int previousBpm;
    
    private float previousTime;
    private int beatTempo;
    private int beatCompass;

    private BeatTiming currentBeatCall;

    public override bool RequiresConstantRepaint() => true;

    private float AudioLerp()
    {
        if (mainAudio == null)
            return 0f;

        return mainAudio.time / mainAudio.clip.length;
    }

    public override void OnInspectorGUI()
    {
        if (EditorSound == null)
        {
            EditorSound = (EditorSoundMaster)EditorGUILayout.ObjectField("Sound Controller", EditorSound, typeof(EditorSoundMaster));
            GUILayout.Label("Add Sound Controller");
            return;
        }

        EditorGUILayout.BeginVertical("Box"); //base
        base.OnInspectorGUI();
        EditorGUILayout.EndVertical(); //0base
        
        if (GUILayout.Button("Reset Textures"))
            ResetTextures();
        
        GUILayout.Space(8);
        
        BeatMapper obj = (BeatMapper)target;
        
        EditorSound.SetMapper(obj);

        GUILayout.BeginHorizontal(); //E
        
        GUILayout.Label("Clip:");
        obj.clip = (AudioClip)EditorGUILayout.ObjectField("", obj.clip, typeof(AudioClip));
        GUILayout.Space(8);
        GUILayout.Label("BPM:");
        obj.bpm = EditorGUILayout.IntField("", obj.bpm, GUILayout.MaxWidth(50));
        GUILayout.FlexibleSpace();
        
        GUILayout.EndHorizontal(); //0E

        mainAudio = EditorSound.GetMainSoundSource();
        beepAudio = EditorSound.GetBeepSource();

        if(mainAudio== null)
            return;
        
        if (mainAudio.clip != obj.clip)
            mainAudio.clip = obj.clip;

        //Values Check
        if (previousClip != obj.clip || previousBpm != obj.bpm)
            OnChangeClip(obj);


        //Sound calls
        var time = mainAudio.time;

        if (mainAudio.isPlaying)
        {
            if (time > (beatTempo + 1) * obj.BeatLenght)
            {
                if(beepAudio!=null)
                    beepAudio.Play();
            }
            
            int nextC = beatCompass+1;
            int nextT = beatTempo;

            if (nextC == 4)
            {
                nextT++;
                nextC = 0;
            }
            
            BeatTiming beat = obj.timedCalls.Find(a => a.compass == nextC && a.tempo == nextT);

            if (beat != null)
            {
                if (time > (nextT * obj.BeatLenght) + (nextC * obj.BeatLenght * 0.25f))
                {
                    var sound = EditorSound.GetAudioSource(obj.GetCallFromCode(beat.code).callClip);
                    if(sound!=null)
                        sound.Play();
                }
            }

            List<BeatTiming> answers = obj.timedCalls.FindAll(bt =>
            {
                var call = obj.GetCallFromCode(bt.code);
            
                if (call.answerCount <= 0) return false;

                var list = bt.GetAnswersTiming(obj);

                for (int i = 0; i < list.Count; i++)
                {
                    int t = list[i].Item1;
                    int c = list[i].Item2;
                    
                    if(!(t == nextT && c == nextC))
                        continue;
                    
                    if (time > (t * obj.BeatLenght) + (c * obj.BeatLenght * 0.25f))
                        return true;
                }

                return false;
            
            });

            foreach (BeatTiming answer in answers)
            {
                var sound = EditorSound.GetAudioSource(obj.GetCallFromCode(answer.code).answerClip);
                if(sound!=null)
                    sound.Play();
            }
            
        }
        
        beatTempo = Mathf.FloorToInt(previousTime / obj.BeatLenght);
        beatCompass = Mathf.FloorToInt((time - (beatTempo * obj.BeatLenght)) / (obj.BeatLenght / 4f));
        previousTime = time;
        
        currentBeatCall = obj.timedCalls.Find(
            (BeatTiming a) =>
            {
                return a.compass == beatCompass && a.tempo == beatTempo;
            });
        
        BeatCallsEditor(obj);
        
        GUILayout.Space(5);
        
        //PLAY Controller
        GUILayout.BeginHorizontal(GUILayout.MaxWidth(55));//A
        if (GUILayout.Button(Resources.Load<Texture2D>("Editor/play")))
            mainAudio.Play();

        if (GUILayout.Button(Resources.Load<Texture2D>("Editor/pause")))
            mainAudio.Pause();

        if (GUILayout.Button(Resources.Load<Texture2D>("Editor/stop")))
        {
            mainAudio.Pause();
            mainAudio.time = 0;
        }
        GUILayout.EndHorizontal(); //A0

        //Time line
        int timelineH = 65;
        int timelineW = Mathf.CeilToInt(mainAudio.clip.length * pixelsPerSecond);

        timelineScroll = GUILayout.BeginScrollView(timelineScroll, false, false); //S
        
        timelineRect = GUILayoutUtility.GetRect(timelineW, timelineH);

        if (timelineRect.width >= 1)
        {
            DrawTimeLine(timelineRect, obj);
        }

        GUILayout.EndScrollView(); //0S

        var scrollSpaceRect = GUILayoutUtility.GetLastRect();
        
        //Scroll timeline mark

        var scrollBarRect = new Rect(scrollSpaceRect.x + 13, scrollSpaceRect.y + scrollSpaceRect.height - 12
            , scrollSpaceRect.width-26, 12);

        float dotLerp = Mathf.Lerp(scrollBarRect.x,
            scrollBarRect.position.x + scrollBarRect.width - 13, AudioLerp());
        
        var pointRect = new Rect( new Vector2(dotLerp, scrollBarRect.y), new Vector2(12, 12));
        
        GUI.DrawTexture(pointRect, Resources.Load<Texture2D>("Editor/reddot"));

        GUILayout.Label($"({beatTempo}/{beatCompass}) {time.ToTimeDisplay()}");
        
        //Calls Buttons
        if (!mainAudio.isPlaying)
        {
            GUILayout.BeginHorizontal(); //B
            AddCallsButtons(obj);
            GUILayout.EndHorizontal(); //B0    
        }
        
        
        //Controles
        
        var mousePosScreen = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
        var canvasPosScreen = GUIUtility.GUIToScreenRect(scrollSpaceRect).position;

        bool mouseInsideTimeline = true;

        var relativeMousePos = mousePosScreen-canvasPosScreen;

        if (relativeMousePos.x < 0 || relativeMousePos.y < 0)
            mouseInsideTimeline = false;
        if (relativeMousePos.x > scrollSpaceRect.width || relativeMousePos.y > timelineH )
            mouseInsideTimeline = false;

        relativeMousePos += timelineScroll;
        
        if (mouseInsideTimeline)
        {
            if (Event.current.type is EventType.MouseDown or EventType.MouseDrag)
            {
                //mainAudio.Pause();
                mainAudio.time = (relativeMousePos.x / timelineRect.width) * mainAudio.clip.length;
            }
        
        }
    }

    private void AddCallsButtons(BeatMapper obj)
    {
        if (currentBeatCall != null)
        {
            if (GUILayout.Button("X", GUILayout.Width(27), GUILayout.Height(27)))
            {
                obj.timedCalls.Remove(currentBeatCall);
                RedrawArrows();
            }
            return;
        }
        
        
        for (int i = 0; i < obj.callTypes.Count; i++)
        {
            int index = i;

            var button = GUILayout.Button($"{obj.callTypes[index].name}", GUILayout.Width(95), GUILayout.Height(27));
            if (button)
            {
                obj.timedCalls.Add(new BeatTiming(beatTempo, beatCompass, obj.callTypes[index].Code));
                RedrawArrows();
            }
        }
    }

    private void ResetTextures()
    {
        waveFormTexture = null;
        middleLineTexture = null;
        backgroundTexture = null;
        combinedArrowsTexture = null;
    }

    private void BeatCallsEditor(BeatMapper obj)
    {
        callEditorScroll = GUILayout.BeginScrollView(callEditorScroll);
        
        if (obj.callTypes == null)
            obj.callTypes = new List<BeatCall>();

        if (obj.timedCalls == null)
            obj.timedCalls = new List<BeatTiming>();

        int callToDelete = -1;
        
        GUILayout.BeginHorizontal(); //A
        
        for (int i = 0; i < obj.callTypes.Count; i++)
        {
            GUILayout.BeginVertical("Box", GUILayout.Width(150)); //B
            
            BeatCall call = obj.callTypes[i];
            
            GUILayout.BeginHorizontal(); //C
            GUILayout.Label($"{i}:", GUILayout.Width(25));
            GUILayout.FlexibleSpace();
            call.name = GUILayout.TextField(call.name,GUILayout.Width(140));

            if (GUILayout.Button("X", GUILayout.Width(18)))
                callToDelete = i;
            
            GUILayout.EndHorizontal(); //C0

            call.answerCount = MyEditorTools.CompactedIntField("Count", call.answerCount, fieldOptions:new[] { GUILayout.Width(40) });
            call.answerDistance = MyEditorTools.CompactedIntField("Distance", call.answerDistance, fieldOptions:new[] { GUILayout.Width(40) });
            call.answersSpacing = MyEditorTools.CompactedIntField("Spacing", call.answersSpacing, fieldOptions:new []{GUILayout.Width(40)});

            GUILayout.Space(8);

            call.callClip = MyEditorTools.CompactedObjectField("Call", call.callClip, fieldOptions: new[] { GUILayout.Width(150) });
            call.answerClip = MyEditorTools.CompactedObjectField("Answer", call.answerClip, fieldOptions: new[] { GUILayout.Width(150) });

            call.editorColor = EditorGUILayout.ColorField(call.editorColor);
            call.editorColor = new Color(call.editorColor.r, call.editorColor.g, call.editorColor.b, 1f);

            GUILayout.EndVertical(); //B0
            
            EditorSound.UpdateMapper(obj);
        }

        if (callToDelete > -1)
        {
            RemoveCallType(obj, callToDelete);
        }
            
        
        if(GUILayout.Button("Add +"))
            obj.callTypes.Add(new BeatCall());
        
        GUILayout.EndHorizontal(); //A0
        
        GUILayout.EndScrollView();
        
    }

    public void DrawTimeLine(Rect r, BeatMapper beatMapper)
    {
        int width = Mathf.CeilToInt(r.width);
        int height = Mathf.CeilToInt(r.height);

        int halfHeight = Mathf.CeilToInt(height / 2f);

        Color bgColor = new Color(0.1f, 0.1f, 0.1f);
        Color lineColor = Color.red;

        int markerLineSize = 1;

        float markerPos = AudioLerp()*width;

        var bgTex = GetBackgroundTexture(width, height, bgColor);
        
        var wave = GetSoundWaveTexture(width, height);

        var dividerTex = GetMiddleLineTexture(width, height, beatMapper);

        var arrowsTex = GetArrowsTexture(width, height, beatMapper);
        
        GUI.DrawTexture(r, bgTex, ScaleMode.ScaleAndCrop, true);
        GUI.DrawTexture(r, wave, ScaleMode.ScaleAndCrop, true);
        GUI.DrawTexture(r, dividerTex, ScaleMode.ScaleAndCrop, true);
        GUI.DrawTexture(r, arrowsTex, ScaleMode.ScaleAndCrop, true);

        //TODO desalinho de 1 pixel
        float floatX = ((beatTempo + beatCompass * 0.25f) * beatMapper.BeatLenght) / beatMapper.clip.length * width;
        
        int x = Mathf.CeilToInt(floatX)-7;

        var dot = TextureDrawingUtil.GetNewTintedTexture(Resources.Load<Texture2D>("Editor/dot"), Color.cyan);
        
        Rect tempoMarkerRect = new Rect(x, halfHeight - 7, 13, 13);
        GUI.DrawTexture(tempoMarkerRect, dot);
        
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
        
        waveFormTexture = AudioTrackPlotter.GetWaveform(Mathf.RoundToInt(lenght), height, mainAudio.clip, Color.clear, Color.yellow - new Color(0f,0f,0f,0.68f));

        return waveFormTexture;
    }

    private Texture2D GetMiddleLineTexture(int lenght, int height, BeatMapper obj)
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
        
        middleLineTexture.PaintColorBlock(lenght, 1, dividerColor, 0, halfH);

        float nOfBeats = (mainAudio.clip.length / 60f) * obj.bpm*4;
        
        for (int i = 1; i < nOfBeats; i++)
        {
            int x = Mathf.RoundToInt(((i * (obj.BeatLenght/4f) / mainAudio.clip.length)*lenght));
            
            if(i%4==0)
                middleLineTexture.PaintColorBlock(1, halfH, dividerColor, x, quarterH);
            else
                middleLineTexture.PaintColorBlock(1, quarterH, dividerColor*new Color(0.7f,0.7f,0.7f), x, quarterH+octH);
        }
        
        return middleLineTexture;
    }

    private void RedrawArrows()
    {
        combinedArrowsTexture = null;
    }
    
    private Texture2D GetArrowsTexture(int lenght, int height, BeatMapper obj)
    {
        if (height == 1 || lenght == 1)
            return null;

        if (combinedArrowsTexture != null)
        {
            if (combinedArrowsTexture.height == height && combinedArrowsTexture.width == lenght)
                return combinedArrowsTexture;
        }

        combinedArrowsTexture = TextureDrawingUtil.GetFill(lenght, height, Color.clear);

        int halfHeight = Mathf.FloorToInt(height/2f);

        //TODO jogar texturas de setas para cima
        var arrow = Resources.Load<Texture2D>("Editor/editorArrow");
        var downArrow = Resources.Load<Texture2D>("Editor/editorArrow2");

        if (obj.timedCalls == null)
            obj.timedCalls = new List<BeatTiming>();
        
        foreach (var c in obj.timedCalls)
        {
            if(c == null)
                continue;

            var call = obj.GetCallFromCode(c.code);
            
            if(call == null)
                continue;
            
            var arrow0 = TextureDrawingUtil.GetNewTintedTexture(arrow, call.editorColor);
            var downArrow0 =
                TextureDrawingUtil.GetNewTintedTexture(downArrow,
                    call.editorColor.AdjustSaturation(-0.42f));
            
            int beatLenghtInPixels = Mathf.RoundToInt((obj.BeatLenght * 0.25f / mainAudio.clip.length) * lenght);
            
            int x = Mathf.RoundToInt(((c.tempo+c.compass*0.25f) * obj.BeatLenght / mainAudio.clip.length)*lenght) - 4;
            
            combinedArrowsTexture.OverlayTexture(x, halfHeight, arrow0);

            //TODO propagacao de erro
            
            //int x = Mathf.RoundToInt(((i * (obj.BeatLenght/4f) / mainAudio.clip.length)*lenght));
            
            for (int i = 0; i < call.answerCount; i++)
            {
                int responseX = x + beatLenghtInPixels * call.answersSpacing * i + beatLenghtInPixels*call.answerDistance;
                combinedArrowsTexture.OverlayTexture(responseX,0,downArrow0);
            }
            
        }
        return combinedArrowsTexture;
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
        beatTempo = 0;
        previousClip = mainAudio.clip;
        previousBpm = obj.bpm;
        ResetTextures();
    }

    private void RemoveCallType(BeatMapper obj, int callToDelete)
    {
        obj.RemoveCall(obj.callTypes[callToDelete].Code);
        RedrawArrows();
    }


}
