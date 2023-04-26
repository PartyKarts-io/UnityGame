#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;


/// <summary>
/// Additional script. For easy installation of objects.
/// </summary>
public class CreatePathTool :MonoBehaviour
{

	[SerializeField] List<SegmentPreset> Segments = new List<SegmentPreset>();
	[SerializeField] float CurrentAngle = 0;
	[SerializeField, HideInInspector] List<SegmentPreset> CreatedSegments = new List<SegmentPreset>();

	public void CreateSegment (Object segment)
	{
		CreateSegment (segment as GameObject);
	}

	public void CreateSegment (GameObject segment)
	{
		CheckCreatedSegments ();

		var segmentRef = Segments.Find (s => s.SegmentPrefab == segment);
		var lastWorldPoint = transform.position;

		if (CreatedSegments.Count > 0)
		{
			var lastSegment = CreatedSegments.Last ();
			lastWorldPoint = lastSegment.SegmentPrefab.transform.TransformPoint (GetMostRightUpPoint(lastSegment.SegmentPrefab.GetComponent<MeshFilter>().sharedMesh));
		}

		var currentGO = GameObject.Instantiate (segmentRef.SegmentPrefab, transform);
		currentGO.transform.position = lastWorldPoint;
		//currentGO.transform.localPosition += GetMostRightDownPoint (currentGO.GetComponent<MeshFilter> ().mesh);
		currentGO.transform.localRotation = Quaternion.AngleAxis (CurrentAngle, Vector3.up);

		CurrentAngle += segmentRef.Angle;

		CreatedSegments.Add (
			new SegmentPreset ()
			{
				SegmentPrefab = currentGO ,
				Angle = segmentRef.Angle
			}
		);
		EditorUtility.SetDirty (gameObject);
	}

	Vector3 GetMostRightUpPoint (Mesh mesh)
	{
		var vertices = new List<Vector3> ();
		mesh.GetVertices (vertices);

		if (vertices == null && vertices.Count == 0)
		{
			Debug.LogError ("Vertices list is null or empty");
			return Vector3.zero;
		}

		Vector3 center = new Vector3 ();
		foreach (var vert in vertices)
		{
			center += vert;
		}

		center /= vertices.Count;

		vertices = vertices.Where (v => v.x >= center.x && v.z >= center.z).ToList ();

		Vector3 result = vertices.First ();
		float maxDistance = (result - center).sqrMagnitude;

		foreach (var vert in vertices)
		{
			float distance = (vert - center).sqrMagnitude;
			if (maxDistance < distance)
			{
				maxDistance = distance;
				result = vert;
			}
		}

		return result;
	}

	void CheckCreatedSegments ()
	{
		CreatedSegments.RemoveAll (s => s.SegmentPrefab == null);
	}

	public void DestroyLastSegment ()
	{
		CheckCreatedSegments ();

		if (CreatedSegments.Count > 0)
		{
			var lastSegment = CreatedSegments.Last ();
			CurrentAngle -= lastSegment.Angle;
			DestroyImmediate (lastSegment.SegmentPrefab.gameObject);
			CreatedSegments.RemoveAt (CreatedSegments.Count - 1);
		}
	}

	public void Clear ()
	{
		CreatedSegments.ForEach (s => DestroyImmediate(s.SegmentPrefab.gameObject));
		CreatedSegments.Clear ();
		CurrentAngle = 0;
	}

	[System.Serializable]
	public struct SegmentPreset
	{
		public GameObject SegmentPrefab;
		public float Angle;
	}
}

[CustomEditor (typeof (CreatePathTool))]
public class CreatePathToolEditor :Editor
{
	public override void OnInspectorGUI ()
	{
		base.OnInspectorGUI ();

		var pathTool = target as CreatePathTool;

		GUILayout.Space (20);

		if (GUILayout.Button ("Remove last segment"))
		{
			pathTool.DestroyLastSegment ();
		}

		if (GUILayout.Button ("Clear all"))
		{
			pathTool.Clear ();
		}
	}
}

[CustomPropertyDrawer (typeof (CreatePathTool.SegmentPreset))]
public class LayerEditor :PropertyDrawer
{
	public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
	{

		var prefabProperty = property.FindPropertyRelative ("SegmentPrefab");
		var angleProperty = property.FindPropertyRelative ("Angle");

		float xPosition = position.x;

		Rect rectButton = position;
		rectButton.width = 40;
		rectButton.x = xPosition;
		xPosition += rectButton.width;

		Rect rectAngleProperty = position;
		rectAngleProperty.width *= 0.2f;
		rectAngleProperty.x = xPosition;
		xPosition += rectAngleProperty.width;

		Rect rectPrefabProperty = position;
		rectPrefabProperty.width *= 0.6f;
		rectPrefabProperty.x = xPosition;
		xPosition += rectPrefabProperty.width + 20;

		var content = new GUIContent ();

		EditorGUI.BeginProperty (position, content, property);

		EditorGUI.BeginChangeCheck ();

		var angleValue = EditorGUI.FloatField (rectAngleProperty, angleProperty.floatValue);
		EditorGUI.ObjectField (rectPrefabProperty, prefabProperty, content);

		var buttonContent = new GUIContent (angleValue.ToString());

		if (GUI.Button (rectButton, buttonContent))
		{
			OnCreate (property, prefabProperty.objectReferenceValue);
		}

		if (EditorGUI.EndChangeCheck())
		{
			angleProperty.floatValue = angleValue;
		}

		EditorGUI.EndProperty ();
	}

	void OnCreate (SerializedProperty property, Object obj)
	{
		CreatePathTool pathTool = property.serializedObject.targetObject as CreatePathTool;
		pathTool.CreateSegment (obj);
	}

}

#endif
