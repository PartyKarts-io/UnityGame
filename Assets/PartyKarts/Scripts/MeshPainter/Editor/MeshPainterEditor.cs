using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;

/// <summary>
/// Additional script.
/// Editor for MeshPainter.cs
/// </summary>
[CustomEditor(typeof(MeshPainter))]
public class MeshPainterEditor : Editor {

	MeshPainter MeshPainter { get { return (MeshPainter)target; } }
	Texture SelectedTexture {
		get { return MeshPainter.SelectedTexture; }
		set { MeshPainter.SelectedTexture = value; }
	}

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		GUILayout.BeginHorizontal();
		MeshPainter.UpdateMeshPainter();
		var textures = MeshPainter.GetTextures;
		for (int i = 0; i < textures.Count; i++ ){
			if (textures[i] != null) {
				GUIContent buttonContent = new GUIContent(textures[i]);
				bool toogleValue = SelectedTexture == textures[i];
				if (GUILayout.Toggle(toogleValue, buttonContent, GUILayout.Height(100), GUILayout.Width(100))) {
					if (textures[i] != SelectedTexture) {
						MeshPainter.StopPaint();
						SelectedTexture = textures[i];
					}
				}
			}
		}
		GUILayout.EndHorizontal();
	}
	private void OnDisable () {
		if (MeshPainter != null && Selection.activeTransform != MeshPainter.transform) {
			MeshPainter.StopPaint();
		}
	}

	public void OnSceneGUI()
	{
		MeshPainter.UpdateInEditor();
	}
}
