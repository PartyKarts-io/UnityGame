using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// This script is needed to set up sounds of collisions with objects.
/// </summary>

[CustomEditor(typeof(GameBalance.SoundSettings))]

public class SoundSettingsEditor : Editor {

	bool ListOfLevelsFoldout;

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		ListOfLevelsFoldout = EditorGUILayout.Foldout(ListOfLevelsFoldout, "SoundCollisionLayers");

		if (ListOfLevelsFoldout) {
			EditorGUI.indentLevel += 1;

			var layers = serializedObject.FindProperty("m_Layers");
			var audioClips = serializedObject.FindProperty("m_AudioClips");

			EditorGUI.BeginChangeCheck();
			var sizeProperty = layers.FindPropertyRelative("Array.size");
			EditorGUILayout.PropertyField(sizeProperty);

			if (EditorGUI.EndChangeCheck()) {
				audioClips.arraySize = layers.arraySize;
				serializedObject.ApplyModifiedProperties();
			}
		
			for (int i = 0; i < layers.arraySize; i++) {

				EditorGUI.BeginChangeCheck();
				EditorGUILayout.BeginHorizontal();

				var layer = layers.GetArrayElementAtIndex(i);
				var audioClip = audioClips.GetArrayElementAtIndex(i);
				GUIContent label = new GUIContent("");

				EditorGUILayout.PropertyField(layer, label);

				EditorGUILayout.PropertyField(audioClip, label);

				EditorGUILayout.EndHorizontal();

				if (EditorGUI.EndChangeCheck()) {
					serializedObject.ApplyModifiedProperties();
				}
			}
			EditorGUI.indentLevel -= 1;
		}
	}
}
