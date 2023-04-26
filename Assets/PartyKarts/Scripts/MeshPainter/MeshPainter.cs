#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;
using System;

/// <summary>
/// Additional script.
/// For painting on the mesh, like a standard terrain.
/// For mobile optimization.
/// TODO The editor has many flaws. In the future, will be finalized.
/// </summary>
[RequireComponent (typeof (MeshRenderer))]
public class MeshPainter :MonoBehaviour
{

	[SerializeField] Material Material;
	[SerializeField] Texture ControlTexture;
	[SerializeField] int ControlTextureSize = 1024;
	[SerializeField] Vector2 Tile;
	[SerializeField] Texture Texture0;
	[SerializeField] Texture Texture1;
	[SerializeField] Texture Texture2;
	[SerializeField] Texture Texture3;

	[SerializeField, Range(0, 100)] int BrushRadius = 1;
	[SerializeField, Range(0f, 1f)] float Opacity = 1f;

	//Temp fields.
	[SerializeField, HideInInspector] Collider DefaultCollider;
	[SerializeField, HideInInspector] MeshCollider TempCollider;
	[SerializeField, HideInInspector] bool EnabledUpdate;

	[HideInInspector, SerializeField] Texture2D TempControlTexture;
	[HideInInspector] public Texture SelectedTexture;
	[HideInInspector, SerializeField] Color SelectedTextureColor;

	Coroutine InPaintCoroutine;

	MeshRenderer _MeshRenderer;
	MeshRenderer MeshRenderer
	{
		get
		{
			if (_MeshRenderer == null)
			{
				_MeshRenderer = GetComponent<MeshRenderer> ();
			}
			return _MeshRenderer;
		}
	}

	public List<Texture> GetTextures
	{
		get
		{
			var list = new List<Texture> () {
				Texture0,
				Texture1,
				Texture2,
				Texture3
			};
			return list;
		}
	}

	// Update if MeshPainter select or change any field.
	public void UpdateMeshPainter ()
	{

		if (Material == null)
			return;

		if (ControlTexture == null)
		{
			var texture2D = new Texture2D (ControlTextureSize, ControlTextureSize);
			for (int x = 0; x < ControlTextureSize; x++)
			{
				for (int y = 0; y < ControlTextureSize; y++)
				{
					texture2D.SetPixel (x, y, Color.black);
				}
			}
			texture2D.Apply ();
			byte[] bytes = texture2D.EncodeToPNG ();
			UnityEngine.Object.DestroyImmediate (texture2D);

			string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene ().name;
			string path = string.Format ("{0}{1}_{2}{3}.png", ControlTexturePath, sceneName, gameObject.name, ControlTextureName);
			var texturePath = Path.Combine (Application.dataPath, path);
			Debug.LogFormat ("Creating asset at {0}", texturePath);

			if (File.Exists (path))
			{
				Debug.LogErrorFormat ("Texture with name '{0}_{1}.png' already exists", sceneName, gameObject.name, ControlTextureName);
			}
			else
			{
				File.WriteAllBytes (texturePath, bytes);
			}

			AssetDatabase.Refresh ();
			ControlTexture = (Texture)AssetDatabase.LoadAssetAtPath (
			string.Format ("Assets/{0}", path),
			typeof (Texture));
			string assetPath = AssetDatabase.GetAssetPath (ControlTexture);
			TextureImporter A = (TextureImporter)AssetImporter.GetAtPath (assetPath);
			A.isReadable = true;
			AssetDatabase.ImportAsset (assetPath, ImportAssetOptions.ForceUpdate);
		}
		MeshRenderer.materials = new Material[1] { Material };
		var currentControlTexture = Material.GetTexture (ControlTextureName);
		if (currentControlTexture == null || (currentControlTexture != ControlTexture && currentControlTexture != TempControlTexture))
		{
			Material.SetTexture (ControlTextureName, ControlTexture);
		}

		Material.SetTexture (Texture0Name, Texture0);
		Material.SetTexture (Texture1Name, Texture1);
		Material.SetTexture (Texture2Name, Texture2);
		Material.SetTexture (Texture3Name, Texture3);

		Material.SetTextureScale (Texture0Name, Tile);
		Material.SetTextureScale (Texture1Name, Tile);
		Material.SetTextureScale (Texture2Name, Tile);
		Material.SetTextureScale (Texture3Name, Tile);
	}

	public void StartPaint (Event e)
	{
		if (InPaintCoroutine != null)
		{
			return;
		}
		if (DefaultCollider == null)
		{
			DefaultCollider = GetComponent<Collider> ();
		}
		if (DefaultCollider != null)
		{
			DefaultCollider.enabled = false;
		}
		if (TempCollider == null)
		{
			TempCollider = GetComponent<MeshCollider> ();
			if (TempCollider == null)
			{
				TempCollider = gameObject.AddComponent<MeshCollider> ();
			}
		}
		TempCollider.enabled = true;
		EnabledUpdate = true;

		if (SelectedTexture == Texture0)
		{ SelectedTextureColor = new Color (0, 0, 0, 1); }
		if (SelectedTexture == Texture1)
		{ SelectedTextureColor = new Color (1, 0, 0, 0); }
		if (SelectedTexture == Texture2)
		{ SelectedTextureColor = new Color (0, 1, 0, 0); }
		if (SelectedTexture == Texture3)
		{ SelectedTextureColor = new Color (0, 0, 1, 0); }
	}

	public void UpdateInEditor ()
	{
		if (SelectedTexture != null && Selection.activeTransform == transform)
		{
			HandleUtility.AddDefaultControl (GUIUtility.GetControlID (FocusType.Passive));
			var e = Event.current;
			if (e != null && e.type == EventType.MouseDown)
			{
				StartPaint (e);
			}
		}
		if (EnabledUpdate)
		{
			var e = Event.current;
			if (e != null && e.button == 0 && (e.type == EventType.MouseDown || e.type == EventType.MouseDrag))
			{
				Paint (HandleUtility.GUIPointToWorldRay (Event.current.mousePosition));
			}
		}
	}

	public void StopPaint ()
	{
		SelectedTexture = null;
		EnabledUpdate = false;
		if (DefaultCollider != null)
		{
			DefaultCollider.enabled = true;
		}
		if (DefaultCollider == TempCollider)
		{
			TempCollider = null;
		}
		else if (TempCollider != null)
		{
			TempCollider.enabled = false;
			GameObject.DestroyImmediate (TempCollider);
		}
		SaveTexture ();
	}

	public void Paint (Ray mouseRay)
	{
		RaycastHit hit = new RaycastHit ();
		var mask = LayerMask.GetMask (LayerMask.LayerToName (gameObject.layer));
		if (!Physics.Raycast (mouseRay, out hit, 1000.0f, mask))
		{
			return;
		}
		if (TempControlTexture == null)
		{
			TempControlTexture = new Texture2D (ControlTexture.width, ControlTexture.height, TextureFormat.RGBA32, true);
			TempControlTexture.SetPixels ((ControlTexture as Texture2D).GetPixels ());
			Material.SetTexture (ControlTextureName, TempControlTexture);
		}
		var coord = hit.textureCoord;
		var centerX = Mathf.FloorToInt (coord.x *= TempControlTexture.width);
		var centerY = Mathf.FloorToInt (coord.y *= TempControlTexture.height);

		var maxRadiusSqr = BrushRadius * BrushRadius;
		for (int x = -BrushRadius; x < BrushRadius; x++)
		{
			for (int y = -BrushRadius; y < BrushRadius; y++)
			{
				if ((x * x) + (y * y) < maxRadiusSqr)
				{
					var centerPixelColor = SelectedTextureColor * Opacity;
					centerPixelColor += TempControlTexture.GetPixel (centerX + x, centerY + y) * (1 - Opacity);
					TempControlTexture.SetPixel (centerX + x, centerY + y, centerPixelColor);
				}
			}
		}
		TempControlTexture.Apply ();
	}

	void SaveTexture ()
	{
		if (TempControlTexture == null)
		{
			return;
		}
		string assetPath = AssetDatabase.GetAssetPath (ControlTexture);
		string savePath = Path.GetFullPath (assetPath);

		var bytes = TempControlTexture.EncodeToPNG ();
		try
		{
			if (File.Exists (savePath) && bytes != null)
			{
				using (var file = File.Open (savePath, FileMode.Open, FileAccess.Write))
				{
					file.Write (bytes, 0, bytes.Length);
					file.Close ();
				}
			}
			else
			{
				Debug.LogErrorFormat ("Texture with path: [{0}] not found", savePath);
			}
		}
		catch (Exception e)
		{
			Debug.LogErrorFormat ("Exception of save file path:{0}.\nExeption:{1}", savePath, e.StackTrace);
			return;
		}
		TempControlTexture = null;
		AssetDatabase.Refresh ();
		Material.SetTexture (ControlTextureName, ControlTexture);
	}

	const string ControlTexturePath = "FullDrift/MeshPainter/";
	const string ControlTextureName = "_ControlTex";
	const string Texture0Name = "_Tex0";
	const string Texture1Name = "_Tex1";
	const string Texture2Name = "_Tex2";
	const string Texture3Name = "_Tex3";
}

#endif
