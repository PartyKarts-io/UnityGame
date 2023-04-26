using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameBalance;
using Thirdweb;

#if UNITY_EDITOR
using UnityEditor;
#endif


/// <summary>
/// Components that inherit from this class have unlock logic.
/// </summary>
public class LockedContent: ScriptableObject
{
	[SerializeField, HideInInspector] protected UnlockType Unlock;
	[SerializeField, HideInInspector] protected int Price;                            //Use if UnlockType == UnlockByMoney.
	[SerializeField, HideInInspector] protected TrackPreset CompleteTrackForUnlock;   //Use if UnlockType == UnlockByTrackComleted.

#if UNITY_EDITOR
	[HideInInspector] public bool ShowProperty;                             //For save show in inspector state.
#endif

	public UnlockType GetUnlockType { get { return Unlock; } }
	public int GetPrice { get { return Price; } }
	public TrackPreset GetCompleteTrackForUnlock { get { return CompleteTrackForUnlock; } }

	public bool IsUnlocked
	{
		get
		{
			switch (Unlock)
			{
				case UnlockType.UnlockByMoney:
					return PlayerProfile.ObjectIsBought (this);
				case UnlockType.UnlockByTrackComleted:
					return PlayerProfile.TrackIsComplited (CompleteTrackForUnlock);
				case UnlockType.UnlockByNFT:
                    return Utils.IsWebGLBuild() ? PlayerProfile.OwnsCorrectNFT(this) : true;
               default:
					return true;
			}

		}
	}

	public bool CanUnlock
	{
		get
		{
			if (Unlock == UnlockType.UnlockByMoney)
			{
				return PlayerProfile.Money >= Price;
			}
			return false;
		}
	}

	public bool TryUnlock ()
	{
		if (Unlock == UnlockType.UnlockByMoney && PlayerProfile.Money >= Price)
		{
			PlayerProfile.Money -= Price;
			PlayerProfile.SetObjectAsBought (this);
			return true;
		}
		return false;
	}

	public enum UnlockType
	{
		Unlocked,
		UnlockByMoney,
		UnlockByTrackComleted,
		UnlockByNFT
	}
}


#if UNITY_EDITOR
[CustomEditor (typeof (LockedContent), true)]

public class UnlockContentEditor :Editor
{
	public override void OnInspectorGUI ()
	{
		base.OnInspectorGUI();

		var lockedContent = target as LockedContent;

		lockedContent.ShowProperty = EditorGUILayout.Foldout (lockedContent.ShowProperty, new GUIContent ("Unlock logic"));

		if (lockedContent.ShowProperty)
		{
			EditorGUI.BeginChangeCheck ();

			var unlockProperty = serializedObject.FindProperty ("Unlock");
			var priceProperty = serializedObject.FindProperty ("Price");
			var trackProperty = serializedObject.FindProperty ("CompleteTrackForUnlock");
			var indent = EditorGUI.indentLevel;

			EditorGUI.indentLevel += 1;
			EditorGUILayout.PropertyField (unlockProperty, new GUIContent ("Lock Type"));

			switch (unlockProperty.enumValueIndex)
			{
				case 1:
					EditorGUILayout.PropertyField (priceProperty, new GUIContent ("Price"));
					break;
				case 2:
					EditorGUILayout.PropertyField (trackProperty, new GUIContent ("Complete Track For Unlock"));
					break;
			}
			EditorGUI.indentLevel = indent;

			if (EditorGUI.EndChangeCheck ())
			{
				serializedObject.ApplyModifiedProperties ();
			}
		}
	}
}

#endif
