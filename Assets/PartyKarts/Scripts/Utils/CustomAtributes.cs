using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PG_Atributes
{

#region ShowIf attribute

	/// <summary>
	/// Attribute for conditional display of a property.
	/// </summary>
	[System.AttributeUsage (System.AttributeTargets.Field)]
	public class ShowInInspectorIf :PropertyAttribute
	{
		public string ConditionalStr;

		public ShowInInspectorIf (string conditionalStr)
		{
			ConditionalStr = conditionalStr;
		}
	}


#if UNITY_EDITOR

	[CustomPropertyDrawer (typeof (ShowInInspectorIf))]
	public class ShowInInspectorIfDrawer :PropertyDrawer
	{
		public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
		{
			ShowInInspectorIf condAtt = (ShowInInspectorIf)attribute;
			bool enabled = GetConditionalHideAttributeResult(condAtt, property);

			if (enabled)
			{
				EditorGUI.PropertyField (position, property, label, true);
			}
		}

		public override float GetPropertyHeight (SerializedProperty property, GUIContent label)
		{
			ShowInInspectorIf condAtt = (ShowInInspectorIf)attribute;
			bool enabled = GetConditionalHideAttributeResult(condAtt, property);

			if (enabled)
			{
				return EditorGUI.GetPropertyHeight (property, label);
			}
			else
			{
				return -EditorGUIUtility.standardVerticalSpacing;
			}
		}

		private bool GetConditionalHideAttributeResult (ShowInInspectorIf condAtt, SerializedProperty property)
		{
			bool enabled = true;

			string propertyPath = property.propertyPath;
			string conditionPath = propertyPath.Replace(property.name, condAtt.ConditionalStr);
			SerializedProperty sourcePropertyValue = property.serializedObject.FindProperty(conditionPath);

			if (sourcePropertyValue != null)
			{
				enabled = sourcePropertyValue.boolValue;
			}
			else
			{
				Debug.LogErrorFormat ("Field with name [{0}] not found", condAtt.ConditionalStr);
			}

			return enabled;
		}
	}

#endif

#endregion //ShowIf attribute

#region Show if attribute

	/// <summary>
	/// Attribute for conditional display of a property.
	/// </summary>
	[System.AttributeUsage (System.AttributeTargets.Field)]
	public class HideInInspectorIf :PropertyAttribute
	{
		public string ConditionalStr;

		public HideInInspectorIf (string conditionalStr)
		{
			ConditionalStr = conditionalStr;
		}
	}


#if UNITY_EDITOR

	[CustomPropertyDrawer (typeof (HideInInspectorIf))]
	public class HideInInspectorIfDrawer :PropertyDrawer
	{
		public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
		{
			HideInInspectorIf condAtt = (HideInInspectorIf)attribute;
			bool enabled = GetConditionalHideAttributeResult(condAtt, property);

			if (enabled)
			{
				EditorGUI.PropertyField (position, property, label, true);
			}
		}

		public override float GetPropertyHeight (SerializedProperty property, GUIContent label)
		{
			HideInInspectorIf condAtt = (HideInInspectorIf)attribute;
			bool enabled = GetConditionalHideAttributeResult(condAtt, property);

			if (enabled)
			{
				return EditorGUI.GetPropertyHeight (property, label);
			}
			else
			{
				return -EditorGUIUtility.standardVerticalSpacing;
			}
		}

		private bool GetConditionalHideAttributeResult (HideInInspectorIf condAtt, SerializedProperty property)
		{
			bool enabled = true;

			string propertyPath = property.propertyPath;
			string conditionPath = propertyPath.Replace(property.name, condAtt.ConditionalStr);
			SerializedProperty sourcePropertyValue = property.serializedObject.FindProperty(conditionPath);

			if (sourcePropertyValue != null)
			{
				enabled = !sourcePropertyValue.boolValue;
			}
			else
			{
				Debug.LogErrorFormat ("Field with name [{0}] not found", condAtt.ConditionalStr);
			}

			return enabled;
		}
	}

#endif

#endregion //Hide if attribute
}
