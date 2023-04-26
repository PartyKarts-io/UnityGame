using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

#endif

/// <summary>
/// The basis of the script is taken from the "Standard Assets" and slightly modified.
/// </summary>
public class WaypointCircuit : MonoBehaviour
{

	public WaypointList m_WaypointList = new WaypointList();
	[SerializeField] private bool m_ShowGizmo = true;
	[SerializeField] private bool m_SmoothRoute = true;
	private int NumPoints;
	private List<Vector3> Points;
	private List<float> Distances;

	public float EditorVisualisationSubsteps = 100;
	public float Length { get; private set; }

	public List<Transform> Waypoints { get { return m_WaypointList.items; } }
	public Transform GetLastPoint { get { return Waypoints[Waypoints.Count - 1]; } }

	//this being here will save GC allocs
	private int P0n;
	private int P1n;
	private int p2n;
	private int p3n;

	private float I;
	private Vector3 P0;
	private Vector3 P1;
	private Vector3 P2;
	private Vector3 P3;

	public void Awake ()
	{
		if (Waypoints.Count > 1)
		{
			CachePositionsAndDistances ();
		}
		NumPoints = Waypoints.Count;
	}


	public RoutePoint GetRoutePoint (float dist)
	{
		// position and direction
		Vector3 p1 = GetRoutePosition (dist);
		Vector3 p2 = GetRoutePosition (dist + 0.1f);
		Vector3 delta = p2 - p1;
		return new RoutePoint (p1, delta.normalized);
	}


	public Vector3 GetRoutePosition (float dist)
	{
		int point = 0;

		if (Length == 0)
		{
			Length = Distances[Distances.Count - 1];
		}

		dist = Mathf.Repeat (dist, Length);

		try {

			while (Distances[point] < dist)
			{
				++point;
			}
		} catch (Exception ex) {
			Debug.LogError (dist);
		}


		// get nearest two points, ensuring points wrap-around start & end of circuit
		P1n = ((point - 1) + NumPoints) % NumPoints;
		p2n = point;

		// found point numbers, now find interpolation value between the two middle points

		I = Mathf.InverseLerp (Distances[P1n], Distances[p2n], dist);

		if (m_SmoothRoute)
		{
			// smooth catmull-rom calculation between the two relevant points


			// get indices for the surrounding 2 points, because
			// four points are required by the catmull-rom function
			P0n = ((point - 2) + NumPoints) % NumPoints;
			p3n = (point + 1) % NumPoints;

			// 2nd point may have been the 'last' point - a dupe of the first,
			// (to give a value of max track distance instead of zero)
			// but now it must be wrapped back to zero if that was the case.
			p2n = p2n % NumPoints;

			P0 = Points[P0n];
			P1 = Points[P1n];
			P2 = Points[p2n];
			P3 = Points[p3n];

			return CatmullRom (P0, P1, P2, P3, I);
		}
		else
		{
			// simple linear lerp between the two points:

			P1n = ((point - 1) + NumPoints) % NumPoints;
			p2n = point;

			return Vector3.Lerp (Points[P1n], Points[p2n], I);
		}
	}


	private Vector3 CatmullRom (Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float i)
	{
		// comments are no use here... it's the catmull-rom equation.
		// Un-magic this, lord vector!
		return 0.5f *
			   ((2 * p1) + (-p0 + p2) * i + (2 * p0 - 5 * p1 + 4 * p2 - p3) * i * i +
				(-p0 + 3 * p1 - 3 * p2 + p3) * i * i * i);
	}


	private void CachePositionsAndDistances ()
	{
		// transfer the position of each point and distances between points to arrays for
		// speed of lookup at runtime
		Points = new List<Vector3> ();
		Distances = new List<float> ();

		float accumulateDistance = 0;
		for (int i = 0; i < Waypoints.Count; ++i)
		{
			var t1 = Waypoints[(i) % Waypoints.Count];
			var t2 = Waypoints[(i + 1) % Waypoints.Count];
			if (t1 != null && t2 != null)
			{
				Vector3 p1 = t1.position;
				Vector3 p2 = t2.position;
				Points.Add (Waypoints[i % Waypoints.Count].position);
				Distances.Add (accumulateDistance);
				accumulateDistance += (p1 - p2).magnitude;
			}
		}

		if (Length == 0)
		{
			Length = Distances[Distances.Count - 1];
		}
	}


	private void OnDrawGizmos ()
	{
		DrawGizmos (false);
	}


	private void OnDrawGizmosSelected ()
	{
		DrawGizmos (true);
	}


	private void DrawGizmos (bool selected)
	{
		if (!m_ShowGizmo)
		{
			return;
		}

		m_WaypointList.circuit = this;
		if (Waypoints.Count > 1)
		{
			NumPoints = Waypoints.Count;

			CachePositionsAndDistances ();
			Length = Distances[Distances.Count - 1];

			Gizmos.color = selected ? Color.yellow : new Color (1, 1, 0, 0.5f);
			Vector3 prev = Waypoints[0].position;
			if (m_SmoothRoute)
			{
				for (float dist = 0; dist < Length; dist += Length / EditorVisualisationSubsteps)
				{
					Vector3 next = GetRoutePosition (dist + 1);
					Gizmos.DrawLine (prev, next);
					prev = next;
				}
				Gizmos.DrawLine (prev, Waypoints[0].position);
			}
			else
			{
				for (int n = 0; n < Waypoints.Count; ++n)
				{
					Vector3 next = Waypoints[(n + 1) % Waypoints.Count].position;
					Gizmos.DrawLine (prev, next);
					prev = next;
				}
			}
		}
	}


	[Serializable]
	public class WaypointList
	{
		public WaypointCircuit circuit;
		public List<Transform> items = new List<Transform>();
	}

	public struct RoutePoint
	{
		public Vector3 position;
		public Vector3 direction;

		public RoutePoint (Vector3 position, Vector3 direction)
		{
			this.position = position;
			this.direction = direction;
		}
	}
}

#if UNITY_EDITOR
[CustomPropertyDrawer (typeof (WaypointCircuit.WaypointList))]
public class WaypointListDrawer :PropertyDrawer
{
	private float lineHeight = 18;
	private float spacing = 4;


	public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
	{
		EditorGUI.BeginProperty (position, label, property);

		float x = position.x;
		float y = position.y;
		float inspectorWidth = position.width;

		// Draw label


		// Don't make child fields be indented
		var indent = EditorGUI.indentLevel;
		EditorGUI.indentLevel = 0;

		var items = property.FindPropertyRelative ("items");
		var titles = new string[] { "Transform", "", "", "" };
		var toolTips = new string[] { "", "Add item after current", "Remove item" };
		var props = new string[] { "transform", "+", "-" };
		var widths = new float[] { .9f, .05f, .05f };
		float lineHeight = 18;
		bool changedLength = false;

		var buttonStyle = new GUIStyle (GUI.skin.button);
		var selectedTransform = new GUIStyle (GUI.skin.textArea);

		if (items.arraySize > 0)
		{
			for (int i = 0; i < items.arraySize; i++)
			{
				var item = items.GetArrayElementAtIndex (i);

				float rowX = x;
				for (int n = 0; n < props.Length; ++n)
				{
					float w = widths[n] * inspectorWidth;

					// Calculate rects
					Rect rect = new Rect (rowX, y, w, lineHeight);
					rowX += w;

					if (n == 0)
					{
						EditorGUI.ObjectField (rect, item.objectReferenceValue, typeof (Transform), true);
					}
					else
					{

						var newcontent = new GUIContent (props[n], toolTips[n]);
						if (GUI.Button (rect, newcontent, buttonStyle))
						{
							switch (props[n])
							{
								case "+":
									CreateEmptyGameObject (property, i);
									changedLength = true;
									break;
								case "-":
									var go = items.GetArrayElementAtIndex (i).objectReferenceValue as Transform;
									GameObject.DestroyImmediate (go.gameObject);
									items.DeleteArrayElementAtIndex (i);
									items.DeleteArrayElementAtIndex (i);
									changedLength = true;
									break;
							}
							UpdateList (property);
						}
					}
				}


				y += lineHeight + spacing;
				if (changedLength)
				{
					break;
				}
			}
		}
		else
		{
			// add button
			var addButtonRect = new Rect ((x + position.width) - widths[widths.Length - 1] * inspectorWidth, y,
										 widths[widths.Length - 1] * inspectorWidth, lineHeight);
			if (GUI.Button (addButtonRect, "+"))
			{
				items.InsertArrayElementAtIndex (items.arraySize);
			}

			y += lineHeight + spacing;
		}

		// add all button
		var addAllButtonRect = new Rect (x, y, inspectorWidth, lineHeight);
		if (GUI.Button (addAllButtonRect, "UpdateList"))
		{
			UpdateList (property);
		}

		// Set indent back to what it was
		EditorGUI.indentLevel = indent;
		EditorGUI.EndProperty ();
	}

	void CreateEmptyGameObject (SerializedProperty property, int index)
	{
		var newItem = new GameObject().transform;
		var circuit = property.FindPropertyRelative ("circuit").objectReferenceValue as WaypointCircuit;

		if (circuit.m_WaypointList.items.Count > index)
		{
			

			if (index + 1 < circuit.m_WaypointList.items.Count)
			{
				var prevItem = circuit.m_WaypointList.items[index];
				var nextItem = circuit.m_WaypointList.items[index + 1];
				newItem.position = (prevItem.position + nextItem.position) / 2;
			}
			else
			{
				var prevItem = circuit.m_WaypointList.items[index];
				newItem.position = prevItem.position;
			}
			
			newItem.SetParent (circuit.transform);
			newItem.SetSiblingIndex (index + 1);
		}
		else
		{
			newItem.SetParent (circuit.transform);
		}
		UpdateList (property);
		Selection.activeTransform = newItem;
		if (SceneView.lastActiveSceneView != null)
		{
			SceneView.lastActiveSceneView.pivot = newItem.position + new Vector3(0, 30, 0);
			SceneView.lastActiveSceneView.Repaint ();
		}
	}

	void UpdateList (SerializedProperty property)
	{
		var circuit = property.FindPropertyRelative ("circuit").objectReferenceValue as WaypointCircuit;
		var children = new Transform[circuit.transform.childCount];
		int n = 0;
		foreach (Transform child in circuit.transform)
		{
			children[n++] = child;
		}
		//Array.Sort(children, new TransformNameComparer());
		circuit.m_WaypointList.items.Clear ();
		for (n = 0; n < children.Length; n++)
		{
			circuit.m_WaypointList.items.Add (children[n]);

			var child = circuit.m_WaypointList.items[n];
			child.name = "Waypoint " + (n+1).ToString ("000");
		}
	}


	public override float GetPropertyHeight (SerializedProperty property, GUIContent label)
	{
		SerializedProperty items = property.FindPropertyRelative ("items");
		float lineAndSpace = lineHeight + spacing;
		return 40 + (items.arraySize * lineAndSpace) + lineAndSpace;
	}
}
#endif
