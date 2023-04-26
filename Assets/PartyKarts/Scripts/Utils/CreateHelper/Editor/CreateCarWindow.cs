using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using GameBalance;
using System.IO;
using PG_Atributes;

namespace CreateHelper {
	public class CreateCarWindow :EditorWindow {

		[SerializeField] CarCreateProperty Property = new CarCreateProperty();
		[SerializeField] LockedContent.UnlockType Unlock;
		[SerializeField] int Price;
		[SerializeField] TrackPreset CompleteTrackForUnlock;

		Editor Editor;
		CreateHelperSettings CreateHelperSettings;

		[MenuItem ("Window/Arcade Car Controller (ACC)/Create car")]
		public static void ShowWindow ()
		{
			GetWindow<CreateCarWindow> ("Create Car");
		}

		void OnGUI ()
		{
			if (Editor == null)
			{
				CreateHelperSettings = Resources.Load<CreateHelperSettings>("CreateHelperSettings");
				Property.CarPresetRef = CreateHelperSettings.CarPresetRef;
				Property.ColorPresets = CreateHelperSettings.ColorPresets;

				Editor = Editor.CreateEditor (this);
			}

			ScriptableObject target = this;
			SerializedObject serializedObject = new SerializedObject (target);
			var property = serializedObject.FindProperty ("Property");

			EditorGUILayout.PropertyField (property, new GUIContent ("Car propeties"), true);

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

			if (GUILayout.Button ("Create car"))
			{
				(Editor.target as CreateCarWindow).OnCreateCar ();
			}

			if (GUILayout.Button("Select folder with car assets"))
			{
				var path = CreateHelperSettings.CarAssetSavePath;
				if (path[path.Length - 1] == '/')
					path = path.Substring (0, path.Length - 1);
				Object obj = AssetDatabase.LoadAssetAtPath(path, typeof(Object));
				Selection.activeObject = obj;
				EditorGUIUtility.PingObject (obj);
			}

			if (GUILayout.Button ("Select folder with car prefabs"))
			{
				var path = CreateHelperSettings.CarPrefabSavePath;
				if (path[path.Length - 1] == '/')
					path = path.Substring (0, path.Length - 1);
				Object obj = AssetDatabase.LoadAssetAtPath(path, typeof(Object));
				Selection.activeObject = obj;
				EditorGUIUtility.PingObject (obj);
			}
		}

		void OnCreateCar ()
		{
			var prefabPath = CreateHelperSettings.CarPrefabSavePath + Property.CarFileName + ".prefab";
			var assetPath = CreateHelperSettings.CarAssetSavePath + Property.CarFileName + ".asset";

			if (File.Exists (prefabPath))
			{
				Debug.LogErrorFormat ("File with name: {0}. already exists", prefabPath);
				return;
			}

			if (File.Exists (assetPath))
			{
				Debug.LogErrorFormat ("File with name: {0}. already exists", assetPath);
				return;
			}

			if (string.IsNullOrEmpty (Property.CarFileName))
			{
				Debug.LogErrorFormat ("CarFileName is empty");
				return;
			}

			if (string.IsNullOrEmpty (Property.CarInGameName))
			{
				Debug.LogErrorFormat ("CarInGameName is empty");
				return;
			}

			if (Property.CarPresetRef == null)
			{
				Debug.LogErrorFormat ("CarPresetRef is null");
				return;
			}

			bool success;
			var source = (GameObject)PrefabUtility.InstantiatePrefab(Property.CarPresetRef.CarPrefab.gameObject);
			PrefabUtility.UnpackPrefabInstance (source, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
			var carPrefab = PrefabUtility.SaveAsPrefabAsset(source, prefabPath, out success);

			GameObject.DestroyImmediate (source);

			if (!success)
			{
				Debug.LogErrorFormat ("Error creating prefab");
				return;
			}

			var assetObject = new CarPreset(Property.CarInGameName, null, carPrefab.GetComponent<CarController>(), 
											Property.ColorPresets, Property.CarDescription, Unlock, Price, CompleteTrackForUnlock);

			AssetDatabase.CreateAsset(assetObject, assetPath);

			string[] guids = AssetDatabase.FindAssets(string.Format("t:{0}", typeof(RegimeSettings)));
			for (int i = 0; i < guids.Length; i++)
			{
				assetPath = AssetDatabase.GUIDToAssetPath( guids[i] );
				RegimeSettings regime = AssetDatabase.LoadAssetAtPath<RegimeSettings>( assetPath );
				
				if (regime != null)
				{
					regime.AddAvailableCar (assetObject);
				}
			}

			B.MultiplayerSettings.AddAvailableCar (assetObject);

			Selection.activeObject = carPrefab;
			EditorGUIUtility.PingObject (carPrefab);

			Close ();
		}

		[System.Serializable]
		private class CarCreateProperty
		{
			[Tooltip("For duplicate the prefab of car controller")]
			public CarPreset CarPresetRef;
			public string CarFileName = "Car";
			public string CarInGameName = "Car";
			[TextArea(1, 3)]
			public string CarDescription = "Car description";

			public List<CarColorPreset> ColorPresets = new List<CarColorPreset>();
		}
	}
}
