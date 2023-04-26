using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEngine.UI;

[CustomEditor(typeof(CustomButton))]
[CanEditMultipleObjects]
public class CustomButtonEditor : UnityEditor.UI.ButtonEditor {

	SerializedProperty SelectedProperty;
	SerializedProperty SoundEnableProperty;
	SerializedProperty ClickSoundProperty;
	SerializedProperty DownSoundProperty;
	SerializedProperty UpSoundProperty;

	protected override void OnEnable () {
		base.OnEnable();

		SelectedProperty = serializedObject.FindProperty ("SelectedOnStart");

		SoundEnableProperty = serializedObject.FindProperty("EnableSounds");

		ClickSoundProperty = serializedObject.FindProperty("OnClickSound");
		DownSoundProperty = serializedObject.FindProperty("OnDownSound");
		UpSoundProperty = serializedObject.FindProperty("OnUpSound");
	}
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		EditorGUI.BeginChangeCheck();

		var btn = target as CustomButton;

		if (btn.navigation.mode != Navigation.Mode.None)
		{
			EditorGUILayout.PropertyField (SelectedProperty);
		}

		EditorGUILayout.PropertyField(SoundEnableProperty);

		if (SoundEnableProperty.boolValue) {
			EditorGUILayout.PropertyField(ClickSoundProperty);
			EditorGUILayout.PropertyField(DownSoundProperty);
			EditorGUILayout.PropertyField(UpSoundProperty);
		}

		if (EditorGUI.EndChangeCheck()) {
			serializedObject.ApplyModifiedProperties();
		}

	}
}