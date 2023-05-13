#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Michsky.UI.Reach
{
    [CustomEditor(typeof(TimerBar))]
    public class TimerBarEditor : Editor
    {
        private GUISkin customSkin;
        private TimerBar tbTarget;
        private int currentTab;

        private void OnEnable()
        {
            tbTarget = (TimerBar)target;

            if (EditorGUIUtility.isProSkin == true) { customSkin = ReachEditorHandler.GetDarkEditor(customSkin); }
            else { customSkin = ReachEditorHandler.GetLightEditor(customSkin); }
        }

        public override void OnInspectorGUI()
        {
            ReachEditorHandler.DrawComponentHeader(customSkin, "Timer Bar Top Header");

            GUIContent[] toolbarTabs = new GUIContent[3];
            toolbarTabs[0] = new GUIContent("Content");
            toolbarTabs[1] = new GUIContent("Resources");
            toolbarTabs[2] = new GUIContent("Settings");

            currentTab = ReachEditorHandler.DrawTabs(currentTab, toolbarTabs, customSkin);

            if (GUILayout.Button(new GUIContent("Content", "Content"), customSkin.FindStyle("Tab Content")))
                currentTab = 0;
            if (GUILayout.Button(new GUIContent("Resources", "Resources"), customSkin.FindStyle("Tab Resources")))
                currentTab = 1;
            if (GUILayout.Button(new GUIContent("Settings", "Settings"), customSkin.FindStyle("Tab Settings")))
                currentTab = 2;

            GUILayout.EndHorizontal();

            var icon = serializedObject.FindProperty("icon");
            var currentValue = serializedObject.FindProperty("currentValue");
            var timeMultiplier = serializedObject.FindProperty("timeMultiplier");
            var timerValue = serializedObject.FindProperty("timerValue");

            var barImage = serializedObject.FindProperty("barImage");
            var iconObject = serializedObject.FindProperty("iconObject");
            var altIconObject = serializedObject.FindProperty("altIconObject");
            var textObject = serializedObject.FindProperty("textObject");
            var altTextObject = serializedObject.FindProperty("altTextObject");

            var canPlay = serializedObject.FindProperty("canPlay");
            var addPrefix = serializedObject.FindProperty("addPrefix");
            var addSuffix = serializedObject.FindProperty("addSuffix");
            var prefix = serializedObject.FindProperty("prefix");
            var suffix = serializedObject.FindProperty("suffix");
            var decimals = serializedObject.FindProperty("decimals");
            var barDirection = serializedObject.FindProperty("barDirection");
            var timeMode = serializedObject.FindProperty("timeMode");

            var onValueChanged = serializedObject.FindProperty("onValueChanged");

            switch (currentTab)
            {
                case 0:
                    ReachEditorHandler.DrawHeader(customSkin, "Content Header", 6);
                    if (tbTarget.barImage != null) { ReachEditorHandler.DrawProperty(icon, customSkin, "Bar Icon"); }

                    GUILayout.BeginHorizontal(EditorStyles.helpBox);
                    EditorGUILayout.LabelField(new GUIContent("Current Value"), customSkin.FindStyle("Text"), GUILayout.Width(120));
                    currentValue.floatValue = EditorGUILayout.Slider(tbTarget.currentValue, 0, timerValue.floatValue);
                    GUILayout.EndHorizontal();

                    ReachEditorHandler.DrawProperty(timerValue, customSkin, "Timer Value");
                    ReachEditorHandler.DrawProperty(timeMultiplier, customSkin, "Time Multiplier");

                    tbTarget.UpdateUI();

                    ReachEditorHandler.DrawHeader(customSkin, "Events Header", 10);
                    EditorGUILayout.PropertyField(onValueChanged, new GUIContent("On Value Changed"));
                    break;

                case 1:
                    ReachEditorHandler.DrawHeader(customSkin, "Core Header", 6);
                    ReachEditorHandler.DrawProperty(barImage, customSkin, "Bar Image");
                    ReachEditorHandler.DrawProperty(iconObject, customSkin, "Icon Object");
                    ReachEditorHandler.DrawProperty(altIconObject, customSkin, "Alt Icon Object");
                    ReachEditorHandler.DrawProperty(textObject, customSkin, "Text Object");
                    ReachEditorHandler.DrawProperty(altTextObject, customSkin, "Alt Text Object");
                    break;

                case 2:
                    ReachEditorHandler.DrawHeader(customSkin, "Options Header", 6);

                    canPlay.boolValue = ReachEditorHandler.DrawToggle(canPlay.boolValue, customSkin, "Can Play");
                    GUILayout.BeginVertical(EditorStyles.helpBox);
                    GUILayout.Space(-3);
                    addPrefix.boolValue = ReachEditorHandler.DrawTogglePlain(addPrefix.boolValue, customSkin, "Add Prefix");
                    GUILayout.Space(3);

                    if (addPrefix.boolValue == true)
                        ReachEditorHandler.DrawPropertyPlainCW(prefix, customSkin, "Prefix:", 40);

                    GUILayout.EndVertical();
                    GUILayout.BeginVertical(EditorStyles.helpBox);
                    GUILayout.Space(-3);
                    addSuffix.boolValue = ReachEditorHandler.DrawTogglePlain(addSuffix.boolValue, customSkin, "Add Suffix");
                    GUILayout.Space(3);

                    if (addSuffix.boolValue == true)
                        ReachEditorHandler.DrawPropertyPlainCW(suffix, customSkin, "Suffix:", 40);

                    GUILayout.EndVertical();

                    ReachEditorHandler.DrawPropertyCW(decimals, customSkin, "Decimals", 84);
                    ReachEditorHandler.DrawPropertyCW(barDirection, customSkin, "Bar Direction", 84);
                    ReachEditorHandler.DrawPropertyCW(timeMode, customSkin, "Time Mode", 84);
                    break;
            }

            serializedObject.ApplyModifiedProperties();
            if (Application.isPlaying == false) { Repaint(); }
        }
    }
}
#endif