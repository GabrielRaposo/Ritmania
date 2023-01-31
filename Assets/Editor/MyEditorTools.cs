using System.Collections;
using System.Collections.Generic;
using Unity.Plastic.Newtonsoft.Json.Serialization;
using UnityEngine;
using UnityEditor;

public static class MyEditorTools
{
	public static void BaseStringListDisplay(List<string> list) {

		string objToRemove = null;
		
		for (int i = 0; i < list.Count; i++) {
			GUILayout.BeginHorizontal();
			list[i] = EditorGUILayout.TextField(list[i]);

			if (GUILayout.Button("X", GUILayout.MaxWidth(25)))
				objToRemove = list[i];

			GUILayout.EndHorizontal();

		}

		if (objToRemove != null) {
			list.Remove(objToRemove);
			objToRemove = null;
		}

		GUILayout.BeginHorizontal();

		GUILayout.FlexibleSpace();

		if (GUILayout.Button("+", GUILayout.MaxWidth(40))) {
			list.Add("");
		}

		GUILayout.EndHorizontal();

	}

	public static void LimitedStringList(List<string> list, string[] options, bool sortOptions = false)
	{		
		string objToRemove = null;
		
		if(sortOptions)
			System.Array.Sort(options);

		for (int i = 0; i < list.Count; i++) {
			GUILayout.BeginHorizontal();

			if(GetIndexFromArray(list[i], options) == -1)
			{
				GUILayout.BeginHorizontal();
				list[i] = EditorGUILayout.TextField(list[i]);

				GUIStyle style = new GUIStyle();
				style.normal.textColor = Color.red;
				style.fontStyle = FontStyle.Bold;

				GUILayout.Label("!!!", style);
				GUILayout.EndHorizontal();
			}
			else
			{
				list[i] = options[EditorGUILayout.Popup(GetIndexFromArray(list[i], options), options)];
			}

			if (GUILayout.Button("X", GUILayout.MaxWidth(25)))
				objToRemove = list[i];

			GUILayout.EndHorizontal();

		}

		if (objToRemove != null) {
			list.Remove(objToRemove);
			objToRemove = null;
		}

		GUILayout.BeginHorizontal();

		GUILayout.FlexibleSpace();

		if (GUILayout.Button("+", GUILayout.MaxWidth(40))) {
			list.Add(options[0]);
		}

		GUILayout.EndHorizontal();

	}

	public static float CompactedFloatField(string label, float value, bool bold)
	{
		GUILayout.BeginHorizontal();
		GUILayout.Label(label, bold ? EditorStyles.boldLabel : null, GUILayout.ExpandWidth(false));
		value = EditorGUILayout.FloatField(value);
		GUILayout.EndHorizontal();

		return value;
	}
	
	public static string CompactedStringField(string label, string value, bool bold)
	{
		GUILayout.BeginHorizontal();
		
		GUILayout.Label(label);
		value = EditorGUILayout.TextField(value);
		GUILayout.EndHorizontal();

		return value;
	}

	public static int CompactedIntField(string label, int value, GUILayoutOption[] labelOptions = null, GUILayoutOption[] fieldOptions = null)
	{
		GUILayout.BeginHorizontal();
		
		GUILayout.Label(label, labelOptions);
		value = EditorGUILayout.IntField(value, fieldOptions);
		
		GUILayout.EndHorizontal();

		return value;
	}

	public static T CompactedObjectField<T>(string label, T value, GUILayoutOption[] labelOptions = null, GUILayoutOption[] fieldOptions = null) where T : Object
	{
		GUILayout.BeginHorizontal();
		
		GUILayout.Label(label, labelOptions);

		value = (T)EditorGUILayout.ObjectField(value, typeof(T), false, fieldOptions);

		GUILayout.EndHorizontal();

		return value;
	}
	
	public static T WatcherObjectField<T>(string label, T value, Action action, GUILayoutOption[] labelOptions = null, GUILayoutOption[] fieldOptions = null) where T : Object
	{
		var initValue = value;
		
		GUILayout.BeginHorizontal();
		
		GUILayout.Label(label, labelOptions);

		value = (T)EditorGUILayout.ObjectField(value, typeof(T), false, fieldOptions);

		GUILayout.EndHorizontal();
		
		if(initValue!=value)
			action?.Invoke();

		return value;
	} 

	public static string AlignLeftStringField(string label, string value)
	{
		GUILayout.BeginHorizontal();
		
		GUILayout.Label(label);
		value = EditorGUILayout.TextField(value);
		
		GUILayout.FlexibleSpace();
		
		GUILayout.EndHorizontal();

		return value;
	}

	public static string StringAsEnum(string value, string[] options)
	{
		if(GetIndexFromArray(value, options) == -1)
		{
			GUILayout.BeginHorizontal();
			value = EditorGUILayout.TextField(value);

			GUIStyle style = new GUIStyle();
			style.normal.textColor = Color.red;
			style.fontStyle = FontStyle.Bold;

			GUILayout.Label("!!!", style);
			GUILayout.EndHorizontal();				
			
		}
		else
		{
			int i = EditorGUILayout.Popup(GetIndexFromArray(value, options), options);
			value = options[i];
		}

		return value;
	}

	public static void Anotation(string text)
	{
		GUIStyle style = new GUIStyle();

		style.normal.textColor = new Color(0.35f, 0.35f, 0.35f);
		style.fontSize = 9;

		GUILayout.Space(3);

		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		GUILayout.Label(text, style);
		GUILayout.EndHorizontal();
	}

	public static void ColoredLabel(string text, Color color, bool bold)
	{
		GUIStyle style = new GUIStyle();

		style.normal.textColor = color;
		if (bold)
			style.fontStyle = FontStyle.Bold;

		GUILayout.Label(text, style);

	}

	public static int GetIndexFromArray(string s, string[] list, bool fixedValue = false) {

		for (int i = 0; i < list.Length; i++) {
			if (list[i] == s)
				return i;
		}

		if (fixedValue)
			return 0;	

		return -1;
	}

	public static void Line()
	{
		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		GUILayout.Label("--------------------------------------------------------", GUILayout.ExpandWidth(false));
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();
	}
	
	public static string ToTimeDisplay(this float t)
	{
		float min = Mathf.Floor(t / 60f);

		float sec = Mathf.Floor(t - (min * 60f));

		float fraction = (t - Mathf.Floor(t))*1000f;

		//return $"{min:00}:{sec:00}:{fraction:000}";
		return $"{min:00}:{sec:00}:{fraction:000}";
	}

}
