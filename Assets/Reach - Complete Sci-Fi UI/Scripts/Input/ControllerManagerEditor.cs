#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

namespace Michsky.UI.Reach
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(ControllerManager))]
    public class ControllerManagerEditor : Editor
    {
        private ControllerManager cmTarget;
        private GUISkin customSkin;

        private void OnEnable()
        {
            cmTarget = (ControllerManager)target;

            if (EditorGUIUtility.isProSkin == true) { customSkin = ReachEditorHandler.GetDarkEditor(customSkin); }
            else { customSkin = ReachEditorHandler.GetLightEditor(customSkin); }
        }

        public override void OnInspectorGUI()
        {
            var presetManager = serializedObject.FindProperty("presetManager");
            var firstSelected = serializedObject.FindProperty("firstSelected");
            var gamepadObjects = serializedObject.FindProperty("gamepadObjects");
            var keyboardObjects = serializedObject.FindProperty("keyboardObjects");

            var alwaysUpdate = serializedObject.FindProperty("alwaysUpdate");
            var affectCursor = serializedObject.FindProperty("affectCursor");
            var gamepadHotkey = serializedObject.FindProperty("gamepadHotkey");

            ReachEditorHandler.DrawHeader(customSkin, "Core Header", 6);
            ReachEditorHandler.DrawProperty(presetManager, customSkin, "Preset Manager");
            ReachEditorHandler.DrawProperty(firstSelected, customSkin, "First Selected", "UI element to be selected first in the home panel (e.g. Play button).");

            GUILayout.BeginVertical();
            EditorGUI.indentLevel = 1;
            EditorGUILayout.PropertyField(gamepadObjects, new GUIContent("Gamepad Objects"), true);
            EditorGUI.indentLevel = 0;
            GUILayout.EndVertical();

            GUILayout.BeginVertical();
            EditorGUI.indentLevel = 1;
            EditorGUILayout.PropertyField(keyboardObjects, new GUIContent("Keyboard Objects"), true);
            EditorGUI.indentLevel = 0;
            GUILayout.EndVertical();

            ReachEditorHandler.DrawHeader(customSkin, "Options Header", 10);
            alwaysUpdate.boolValue = ReachEditorHandler.DrawToggle(alwaysUpdate.boolValue, customSkin, "Always Update");
            affectCursor.boolValue = ReachEditorHandler.DrawToggle(affectCursor.boolValue, customSkin, "Affect Cursor", "Changes the cursor state depending on the controller state.");
            EditorGUILayout.PropertyField(gamepadHotkey, new GUIContent("Gamepad Hotkey", "Triggers to switch to gamepad when pressed."), true);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif