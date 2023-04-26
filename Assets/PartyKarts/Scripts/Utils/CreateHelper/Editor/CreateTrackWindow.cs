using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using GameBalance;
using System.IO;
using UnityEditor.SceneManagement;
using System.Linq;

namespace CreateHelper
{
	public class CreateTrackWindow :EditorWindow
	{

		[SerializeField] TrackProperty Property = new TrackProperty();
		[SerializeField] LockedContent.UnlockType Unlock;
		[SerializeField] int Price;
		[SerializeField] TrackPreset CompleteTrackForUnlock;

		Editor Editor;
		CreateHelperSettings CreateHelperSettings;

		[MenuItem ("Window/Arcade Car Controller (ACC)/Create track")]
		public static void ShowWindow ()
		{
			GetWindow<CreateTrackWindow> ("Create track");
		}

		void OnGUI ()
		{
			if (Editor == null)
			{
				CreateHelperSettings = Resources.Load<CreateHelperSettings> ("CreateHelperSettings");
				Property.Regime = CreateHelperSettings.DefaultRegime;

				Editor = Editor.CreateEditor (this);
			}

			ScriptableObject target = this;
			SerializedObject serializedObject = new SerializedObject (target);
			var property = serializedObject.FindProperty ("Property");

			EditorGUILayout.PropertyField (property, new GUIContent ("Track propeties"), true);

			var unlock = serializedObject.FindProperty ("Unlock");
			EditorGUILayout.PropertyField (unlock, new GUIContent ("Unlock"), true);

			if (unlock.enumValueIndex == (int)LockedContent.UnlockType.UnlockByMoney)
			{
				var price = serializedObject.FindProperty ("Price");
				EditorGUILayout.PropertyField (price, new GUIContent ("Price"), true);
			}
			else if (unlock.enumValueIndex == (int)LockedContent.UnlockType.UnlockByTrackComleted)
			{
				var completeTrack = serializedObject.FindProperty ("CompleteTrackForUnlock");
				EditorGUILayout.PropertyField (completeTrack, new GUIContent ("Complete Track For Unlock"), true);
			}

			serializedObject.ApplyModifiedProperties ();

			if (GUILayout.Button ("Create track"))
			{
				var window = (Editor.target as CreateTrackWindow);
				window.OnCreateTrack ();
			}

			if (GUILayout.Button ("Select folder with track assets"))
			{
				var path = CreateHelperSettings.TrackAssetSavePath;
				if (path[path.Length - 1] == '/')
					path = path.Substring (0, path.Length - 1);
				Object obj = AssetDatabase.LoadAssetAtPath(path, typeof(Object));
				Selection.activeObject = obj;
				EditorGUIUtility.PingObject (obj);
			}

			if (GUILayout.Button ("Select folder with track scenes"))
			{
				var path = CreateHelperSettings.TrackSceneSavePath;
				if (path[path.Length - 1] == '/')
					path = path.Substring (0, path.Length - 1);
				Object obj = AssetDatabase.LoadAssetAtPath(path, typeof(Object));
				Selection.activeObject = obj;
				EditorGUIUtility.PingObject (obj);
			}

			if (GUILayout.Button ("Select folder with game controllers"))
			{
				var path = CreateHelperSettings.GameControllerSavePath;
				if (path[path.Length - 1] == '/')
					path = path.Substring (0, path.Length - 1);
				Object obj = AssetDatabase.LoadAssetAtPath(path, typeof(Object));
				Selection.activeObject = obj;
				EditorGUIUtility.PingObject (obj);
			}
		}

		void OnCreateTrack ()
		{
			var scenePath = CreateHelperSettings.TrackSceneSavePath + Property.TrackFileName + ".unity";
			var assetPath = CreateHelperSettings.TrackAssetSavePath + Property.TrackFileName + ".asset";
			var controllerPath = CreateHelperSettings.GameControllerSavePath + "GameController_" + Property.TrackFileName + ".prefab";

			EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo ();

			if (File.Exists (scenePath))
			{
				Debug.LogErrorFormat ("Scene with name: {0}. already exists", scenePath);
				return;
			}

			if (File.Exists (assetPath))
			{
				Debug.LogErrorFormat ("File with name: {0}. already exists", assetPath);
				return;
			}

			if (string.IsNullOrEmpty (Property.TrackFileName))
			{
				Debug.LogErrorFormat ("TrackFileName is empty");
				return;
			}

			if (string.IsNullOrEmpty (Property.TrackInGameName))
			{
				Debug.LogErrorFormat ("TrackInGameName is empty");
				return;
			}

			if (Property.Regime == null)
			{
				Debug.LogErrorFormat ("Regime is null");
				return;
			}

			var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

			bool success;
			var source = (GameObject)PrefabUtility.InstantiatePrefab(CreateHelperSettings.GameControllerRef.gameObject);
			PrefabUtility.UnpackPrefabInstance (source, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
			var gameControllerPrefab = PrefabUtility.SaveAsPrefabAsset(source, controllerPath, out success);

			GameObject.DestroyImmediate (source);

			if (!success)
			{
				Debug.LogErrorFormat ("Error creating prefab");
				return;
			}

			PrefabUtility.InstantiatePrefab (gameControllerPrefab, scene);
			PrefabUtility.InstantiatePrefab(CreateHelperSettings.FXControllerRef, scene);

			EditorSceneManager.SaveScene (scene, scenePath);
			var scenes = EditorBuildSettings.scenes.ToList();

			scenes.Add (new EditorBuildSettingsScene(scenePath, true));

			EditorBuildSettings.scenes = scenes.ToArray ();

			var assetObject = new TrackPreset(Property.TrackInGameName, Property.TrackIcon, Property.TrackFileName, gameControllerPrefab.GetComponent<GameController>(), Property.LapsCount, 
				Property.AIsCount, Property.Regime, Property.MoeyForFirstPlace, Unlock, Price, CompleteTrackForUnlock);

			AssetDatabase.CreateAsset (assetObject, assetPath);

			B.GameSettings.AddAvailableTrack (assetObject);
			B.MultiplayerSettings.AddAvailableTrack (assetObject);

			var camera = GameObject.Find ("Main Camera");
			if (camera != null)
			{
				DestroyImmediate (camera);
			}

			Close ();
		}

		[System.Serializable]
		private class TrackProperty
		{
			public string TrackFileName = "Track";
			public string TrackInGameName = "Track";
			public Sprite TrackIcon;
			public int LapsCount = 3;
			public int AIsCount = 3;
			public RegimeSettings Regime;
			public int MoeyForFirstPlace = 1000;
			public bool OpenCreatedScene = true;
		}
	}
}
